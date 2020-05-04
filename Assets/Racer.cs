using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Racer : MonoBehaviour {
    [SerializeField]
    private float m_maxSpeed = 3.0f;

    [SerializeField]
    //private float m_accelerationForce = 1.0f;
    private float m_accelerationBase = 1.0f;

    [SerializeField]
    private float m_jumpForce = 1.0f;

    [SerializeField]
    float m_turnSpeed = 10.0f;

    [SerializeField]
    float m_airControlMult = 0.5f;

    [SerializeField]
    private float m_gravityAccel = 9.81f;

    public float AccelerationBonus = 0.0f;
    public float MaxSpeedBonus = 0.0f;
    public float TurnSpeedBonus = 0.0f;

    public bool IsFinished = false;

    public string TimeToStr(float a_time) {
        var secondsTotal = a_time;
        var minutes = Mathf.FloorToInt( secondsTotal / 60.0f );
        var seconds = secondsTotal - minutes * 60.0f;
        return string.Format( "{0:D2}:{1:00.00}", minutes, seconds );
    }

    public int Lap {  get { return m_lap; } }

    public bool IsGrounded {  get { return m_isGrounded; } }
    public Vector3 Facing {  get {  return Quaternion.Euler( 0.0f, m_rotation, 0.0f ) * Vector3.forward; } }
    public float GravityMult { private get; set; }

    public string AllLapTimeStr {
        get {
            var lapTimeStr = "";
            for ( var i = 0; i < m_lapTimeList.Count; ++i )
                lapTimeStr += string.Format( "{0}. {1}\n", i + 1, TimeToStr( m_lapTimeList[i] ) );
            return lapTimeStr;
        }
    }
    public string TotalTimeStr {  get { return TimeToStr( TotalTime ); } }
    public float TotalTime {
        get {
            var time = 0.0f;
            foreach ( var lapTime in m_lapTimeList ) {
                time += lapTime;
            }
            return time + m_lapTimeElapsed;
        }
    }

    private Rigidbody m_body = null;
    private Vector3 m_force = Vector3.zero;
    private Vector3 m_moveDirection = Vector3.zero;
    private Vector3 m_moveDirectionTarget = Vector3.zero;
    private float m_speed = 0.0f;

    private float m_rotation = 0.0f;

    private bool m_isGrounded = false;

    private List<float> m_lapTimeList = new List<float>();
    private float m_lapTimeElapsed = 0.0f;
    private int m_lap = 0;

    private Checkpoint m_prevCheckpoint = null;

    public void ReturnToCheckpoint() {
        if ( m_prevCheckpoint == null ) m_prevCheckpoint = FindObjectOfType<Checkpoint>();
        transform.position = m_prevCheckpoint.RespawnPoint;
        transform.forward = m_prevCheckpoint.RespawnFacing;
        Stop();
    }

    public void Stop() {
        m_body.velocity = Vector3.zero;
        m_speed = 0.0f;
        m_moveDirection = m_moveDirectionTarget = Vector3.zero;
    }

    private void OnTriggerEnter( Collider other ) {
        var checkpoint = other.GetComponent<Checkpoint>();
        if ( checkpoint == null ) return;

        m_prevCheckpoint = checkpoint;

        if ( checkpoint.IsEnd ) {
            if( m_lap > 0 ) m_lapTimeList.Add( m_lapTimeElapsed );
            ++m_lap;
            m_lapTimeElapsed = 0.0f;

            if ( m_lap > RaceManager.instance.LapCount )
                RaceManager.instance.Finish( this );
        }
    }

    public void Accelerate(Vector3 m_direction) {
        if ( m_direction.magnitude < Mathf.Epsilon || m_isGrounded == false ) return;
        m_moveDirectionTarget = m_direction;
        m_speed += ( m_accelerationBase + AccelerationBonus ) * Time.deltaTime;
    }

    public void Decelerate( float a_multiplier ) {
        if ( m_isGrounded == false ) return;
        m_speed -= ( m_accelerationBase + AccelerationBonus ) * a_multiplier * Time.deltaTime;
    }

    public void Ground() {
        Debug.Log( "Ground" );
        m_body.AddForce( Vector3.down * m_jumpForce );
    }

    public void Jump() {
        Debug.Log( "Jump" );
        m_body.AddForce( Vector3.up * m_jumpForce );
    }

    [SerializeField]
    private float m_turnDecelMult = 0.2f;

    public void Turn(float a_turn) {
        if ( Mathf.Abs( a_turn ) < Mathf.Epsilon ) return;

        if ( m_isGrounded ) {
            Decelerate( m_turnDecelMult );
        } 

        var turnSpeed = a_turn * ( m_turnSpeed + TurnSpeedBonus );
        if ( IsGrounded == false ) turnSpeed *= m_airControlMult;
        m_rotation += turnSpeed * Time.deltaTime;

        m_moveDirectionTarget = Quaternion.Euler( 0.0f, turnSpeed * Time.deltaTime, 0.0f ) * m_moveDirectionTarget;
    }

    private void Awake() {
        m_body = GetComponent<Rigidbody>();
        GravityMult = 1.0f;
    }

    private void FixedUpdate() {
        if ( IsFinished ) return;

        m_body.AddForce( Vector3.down * m_gravityAccel * GravityMult, ForceMode.Acceleration );
    }

    private void Update() {
        if ( IsFinished ) return;

        m_moveDirection = Vector3.Lerp( m_moveDirection, m_moveDirectionTarget, Time.deltaTime );
        m_speed = Mathf.Clamp( m_speed, 0.0f, m_maxSpeed + MaxSpeedBonus );
        m_moveDirection.y = 0.0f;
        m_body.velocity = m_moveDirection * m_speed + Vector3.up * m_body.velocity.y;

        if ( m_lap > 0 ) m_lapTimeElapsed += Time.deltaTime;

        RaycastHit hitInfo;
        m_isGrounded = Physics.Raycast( transform.position, Vector3.down, out hitInfo, m_groundDistance );
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine( transform.position, transform.position + Vector3.down * m_groundDistance );

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere( transform.position + Vector3.down * m_groundDistance, 0.2f );
    }

    [SerializeField]
    private float m_groundDistance = 1.0f;
}

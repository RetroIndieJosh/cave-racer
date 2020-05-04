using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RaceController : MonoBehaviour {
    static public RaceController instance = null;

    [SerializeField]
    private Racer m_racer = null;

    [SerializeField]
    private Vector3 m_followOffset = Vector3.zero;

    [SerializeField]
    private float m_heightAdjustMult = 0.01f;

    [SerializeField]
    private float m_brakeMultiplier = 2.0f;

    [Header("UI")]

    [SerializeField]
    private TextMeshProUGUI m_speedDisplay = null;

    [SerializeField]
    private TextMeshProUGUI m_lapTimeDisplay = null;

    [SerializeField]
    private TextMeshProUGUI m_currentTimeDisplay = null;

    [Header("Debug")]

    [SerializeField]
    private bool m_debug = false;

    public Racer CurrentRacer {  get { return m_racer; } }

    public bool IsPlayer = true;

    private float m_height = 0.0f;
    private float m_rotation = 0.0f;
    private float m_targetHeight = 0.0f;

    private Digger m_digger = null;

    public void NextRacer() {
        IsPlayer = false;

        var step = 0;
        var list = FindObjectsOfType<Racer>();
        while( true) {
            ++step;
            if ( step > 1000 ) return;

            var i = Random.Range( 0, list.Length );
            if ( list[i].IsFinished || list[i] == m_racer ) continue;
            m_racer = list[i];
            return;
        }
    }

    private void Awake() {
        instance = this;
        m_digger = m_racer.GetComponent<Digger>();
    }

    private void Start() {
        m_targetHeight = m_height = m_followOffset.y;
    }

    private void Update() {
        if ( m_racer == null ) return;

        if ( !IsPlayer && Input.GetButtonDown( "Jump" ) )
            NextRacer();

        var heightDiff = m_targetHeight - m_height;
        var shift = heightDiff * m_heightAdjustMult;

        // don't shift down when in air
        if ( shift > 0 || m_racer.IsGrounded ) {
            if ( m_debug ) {
                Debug.LogFormat( "Height: {0} -> {1} / Shift: {2} / Height: {3} -> {4}", m_height, m_targetHeight,
                    shift, m_height, m_height + shift );
            }

            m_height += shift;
            //if ( Mathf.Abs( m_targetHeight - m_height ) < shift )
            //m_height = m_targetHeight;
        }

        transform.position = m_racer.transform.position + m_racer.Facing * m_followOffset.z
            + Vector3.up * m_height;
        transform.LookAt( m_racer.transform.position );

        if( Input.GetButtonDown("Reset Camera")) {
            m_targetHeight = m_height = m_followOffset.y;
        }

        if ( IsPlayer == false ) return;

        var accelerate = Input.GetAxis( "Accelerate" );
        if ( Mathf.Abs( accelerate ) > Mathf.Epsilon )
            RaceManager.instance.HasStarted = true;

        if ( accelerate > Mathf.Epsilon )
            m_racer.Accelerate( transform.forward * accelerate );
        else if ( accelerate < -Mathf.Epsilon )
            m_racer.Decelerate( m_brakeMultiplier * -accelerate );

        var turn = Input.GetAxis( "Horizontal Left" );
        m_racer.Turn( turn );

        RaycastHit hitInfo;
        var hit = Physics.Raycast( transform.position, Vector3.down, out hitInfo );

        if ( hit ) {
            if ( m_debug ) Debug.Log( "Hit down" );
        } else {
            hit = Physics.Raycast( transform.position, Vector3.up, out hitInfo );
            if ( m_debug ) Debug.Log( "Hit up" );
        }

        if ( hit ) {
            m_targetHeight = hitInfo.point.y + m_followOffset.y - m_racer.transform.position.y;
            //Debug.LogFormat( "Target height: {0}", m_targetHeight );
        }

        //CheckDig();
        CheckGravity();
        CheckJump();
        UpdateHud();
    }

    private void CheckJump() {
        if( m_racer.IsGrounded == false || Input.GetButtonDown( "Jump" ) == false )
            return;
        m_racer.Jump();
    }

    private void CheckDig() {
        //if ( m_digger == null ) return;
        //m_digger.IsDigging = Input.GetButton( "Dig" );
        if ( m_racer.IsGrounded == true || Input.GetButtonDown( "Dig" ) == false ) return;
        m_racer.Ground();
    }

    [SerializeField]
    private float m_gravityMultMin = 0.5f;

    [SerializeField]
    private float m_gravityMultMax = 2.0f;

    private void CheckGravity() {
        var vertical = Input.GetAxis( "Vertical Left" );
        if ( Mathf.Abs( vertical ) < Mathf.Epsilon ) return;

        if ( vertical < 0.0f ) {
            var range = 1.0f - m_gravityMultMin;
            m_racer.GravityMult = 1.0f + range * vertical;
        } else {
            var range = m_gravityMultMax - 1.0f;
            m_racer.GravityMult = vertical * range + 1.0f;
        }
    }

    private void UpdateHud() {
        m_currentTimeDisplay.text = m_racer.TotalTimeStr;
        m_lapTimeDisplay.text = m_racer.AllLapTimeStr;

        var speed = m_racer.GetComponent<Rigidbody>().velocity.magnitude * 2.23694f;
        m_speedDisplay.text = string.Format( "{0,3:D3} mph", Mathf.RoundToInt( speed ) );
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Racer))]
public class AiRacer : MonoBehaviour
{
    [SerializeField]
    private Node m_firstNode = null;

    [SerializeField]
    private float m_speedBonusPerLap;

    private Racer m_racer = null;
    private Node m_targetNode = null;

    private void OnTriggerEnter( Collider other ) {
        var node = other.GetComponent<Node>();
        if ( node == null || node != m_targetNode ) return;

        m_targetNode = m_targetNode.Next;
        if ( m_targetNode== null ) m_targetNode = m_firstNode;
    }

    private void Awake() {
        m_racer = GetComponent<Racer>();
        m_targetNode = m_firstNode;
    }

    private bool m_isWavering = false;
    private float m_timeSinceLastWaverCheck = 0.0f;
    private float m_timeSinceWaverStarted = 0.0f;

    [SerializeField]
    private float m_waverTime = 5.0f;

    [SerializeField]
    private float m_waverRollTime = 5.0f;

    [SerializeField]
    private int m_waverChance = 50;

    [SerializeField]
    private float m_waverMax = 1.0f;

    private float m_waverOffset = 0.0f;
    private Vector3 m_targetForward = Vector3.zero;

    private float m_speedMult = 1.0f;

    public void ReturnToCheckpoint() {
        m_targetNode = m_firstNode;
    }

    private void Update() {
        if ( RaceManager.instance.HasStarted == false || m_racer.IsFinished ) return;

        m_racer.MaxSpeedBonus = m_racer.Lap * m_speedBonusPerLap;

        //transform.forward = m_targetNode.transform.position - transform.position;
        //m_racer.Accelerate( transform.forward );
        if ( m_waverOffset > 0 ) {
            m_timeSinceWaverStarted += Time.deltaTime;
            if ( m_timeSinceWaverStarted > m_waverTime ) {
                m_waverOffset = 0.0f;
                m_timeSinceLastWaverCheck = 0.0f;
                m_speedMult = 1.0f;
            }
        } else {
            m_timeSinceLastWaverCheck += Time.deltaTime;
            if ( m_timeSinceLastWaverCheck > m_waverRollTime ) {
                if ( Random.Range( 0, 100 ) < m_waverChance ) {
                    m_speedMult = Random.Range( 0.5f, 1.5f );
                    m_waverOffset = Random.Range( -m_waverMax, m_waverMax );
                    m_timeSinceWaverStarted = 0.0f;
                    //Debug.LogFormat( "{0} is wavering {1}", name, m_waverOffset );
                } else {
                    m_timeSinceLastWaverCheck = 0.0f;
                }
            }
        }
        m_targetForward = m_targetNode.transform.position - transform.position + m_waverOffset * Vector3.one;

        m_racer.Accelerate( m_targetForward.normalized * 0.8f * m_speedMult );
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine( transform.position, transform.position + m_targetForward );

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere( transform.position + m_targetForward, 0.2f );
    }
}

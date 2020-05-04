using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField]
    private Node m_next = null;

    public Node Next {  get { return m_next; } }

    private void OnDrawGizmos() {
        if ( m_next == null ) return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine( transform.position, m_next.transform.position );

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere( transform.position + m_next.transform.position, 0.2f );
    }
}

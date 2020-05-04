using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUnit : MonoBehaviour
{
    [SerializeField]
    private bool m_isDestructible = false;

    private void OnCollisionEnter( Collision collision ) {
        if ( m_isDestructible ) Destroy( this );
    }
}

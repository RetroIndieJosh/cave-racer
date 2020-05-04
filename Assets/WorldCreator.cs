using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCreator : MonoBehaviour
{
    [SerializeField]
    private WorldUnit m_cubePrefab = null;

    [SerializeField]
    private Vector3Int m_worldSize = new Vector3Int( 30, 5, 30 );

    private void Start() {
        for ( int x = 0; x < m_worldSize.x; ++x ) {
            for ( int y = 0; y < m_worldSize.y; ++y ) {
                for ( int z = 0; z < m_worldSize.z; ++z ) {
                    Instantiate( m_cubePrefab, new Vector3( x, y, z ), Quaternion.identity, transform );
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Digger : MonoBehaviour
{
    [SerializeField]
    private float m_digSpeed = 0.5f;

    [SerializeField]
    private float m_digDistance = 0.2f;

    [SerializeField]
    private int digWidth = 4;

    public bool IsDigging = false;

    private void OnCollisionEnter( Collision collision ) {
        if ( IsDigging == false ) return;

        Debug.Log( "Dig collision" );
        var terrain = collision.collider.GetComponent<Terrain>();
        if ( terrain == null ) return;

        foreach ( var contact in collision.contacts ) {
            Dig( terrain, contact.point.x, contact.point.z );
        }
    }

    private void Update() {
        if ( IsDigging == false ) return;

        RaycastHit hitInfo;
        var hit = Physics.Raycast( transform.position, Vector3.down, out hitInfo );

        if ( hit == false || hitInfo.distance > m_digDistance ) return;

        var terrain = hitInfo.collider.GetComponent<Terrain>();
        if ( terrain == null ) return;

        Dig( terrain, hitInfo.point.x, hitInfo.point.z );
    }

    private void Dig(Terrain a_terrain, float a_x, float a_z) {
        var width = a_terrain.terrainData.heightmapWidth;
        var height = a_terrain.terrainData.heightmapHeight;
        float[,] heights = a_terrain.terrainData.GetHeights( 0, 0, width, height );

        var x = Mathf.FloorToInt( ( a_x - a_terrain.transform.position.x ) / a_terrain.terrainData.size.x * width );
        var z = Mathf.FloorToInt( ( a_z - a_terrain.transform.position.z ) / a_terrain.terrainData.size.z * height );

        var halfDigWidth = digWidth / 2;
        for( int z2 = z - halfDigWidth; z2 < z + halfDigWidth; ++z2 ) {
            for ( int x2 = x - halfDigWidth; x2 < x + halfDigWidth; ++x2 ) {
                heights[z2, x2] -= m_digSpeed * Time.deltaTime;
            }
        }

        //Debug.LogFormat( "Dig hit: ({0}, {1}) = {2}", x, z, heights[x, z] );
        a_terrain.terrainData.SetHeights( 0, 0, heights );
    }
}

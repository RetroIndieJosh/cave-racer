using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTerrain : MonoBehaviour
{
    [SerializeField]
    private float m_perlinScale = 0.1f;

    [SerializeField]
    private float m_seed = 0.0f;

    /*
    public int detailScale {  set { m_detailScale = value; } }
    //[SerializeField]
    private float m_detailScale = 5.0f;

    [SerializeField]
    private List<GameObject> m_treePrefabs;

    public GenerateRegion region { set { m_region = value; } }
    private GenerateRegion m_region;

    public float worldSize {  set { m_worldSize = value; } }
    private float m_worldSize;

    private bool m_treesGenerated = false;

    void Start() {

        // HACK for testing individual terrain tiles
        if( m_worldSize == 0 ) {
            m_worldSize = 1000;
        }

        generateLand();

        gameObject.AddComponent<MeshCollider>();

        if ( m_region != null ) {
            var renderer = gameObject.GetComponent<MeshRenderer>();
            renderer.material.SetTexture( "_MainTex", m_region.texture );
        }
    }

    void Update() {
        if ( m_treesGenerated ) return;
        generateTrees();
        m_treesGenerated = true;
    }
    */

    private void Start() {
        var terrain = GetComponent<Terrain>();
        var width = terrain.terrainData.heightmapWidth;
        var height = terrain.terrainData.heightmapHeight;
        float[,] heights = new float[width, height];

        for( int x = 0; x < width; ++x ) {
            for ( int y = 0; y < height; ++y ) {
                if ( x < 5 || x >= width - 5 || y < 5 || y >= height - 5 ) {
                    heights[x, y] = 1.0f;
                    continue;
                }
                //heights[x, y] = h;
                //heights[x, y] = (float)( x + y * width ) / (float)(width * height); 
                heights[x, y] = Mathf.Min( Mathf.PerlinNoise( x * m_perlinScale + m_seed , y * m_perlinScale + m_seed ) * 0.5f, 0.5f );
            }
        }

        terrain.terrainData.SetHeights( 0, 0, heights );
    }

    /*
    private void generateLand() {
        var mesh = this.GetComponent<MeshFilter>().mesh;
        var verts = mesh.vertices;
        for ( int i = 0; i < verts.Length; ++i ) {

            // get world position
            var x = Mathf.Repeat( verts[i].x + transform.position.x, m_worldSize ) / m_detailScale;
            var z = Mathf.Repeat( verts[i].z + transform.position.z, m_worldSize ) / m_detailScale;

            verts[i].y = Mathf.PerlinNoise( x, z ) * m_heightScale;
        }

        mesh.vertices = verts;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    private void generateTrees() {
        if ( m_region == null || m_region.treeChance == 0 || m_treePrefabs.Count == 0 ) return;

        // Cantor pairing function - same trees every time revisited
        var a = Mathf.Repeat( transform.position.x, m_worldSize );
        var b = Mathf.Repeat( transform.position.z, m_worldSize );
        var seed = Mathf.FloorToInt( 0.5f * ( a + b ) * ( a + b + 1 ) + b );
        Random.InitState( seed );

        var count = 0;
        var mesh = this.GetComponent<MeshFilter>().mesh;
        var radius = Mathf.FloorToInt( mesh.bounds.extents.x );
        for ( int x = -radius; x < radius; x += m_region.treeTileSize ) {
            for ( int z = -radius; z < radius; z += m_region.treeTileSize ) {
                if ( Random.Range( 0, 100 ) > m_region.treeChance ) continue; 

                var pos = new Vector3( x + transform.position.x, m_heightScale * 2.0f, z + transform.position.z );
                var ti = Random.Range( 0, m_treePrefabs.Count );

                // TODO optimize by having prefabs with these already in
                var tree = Instantiate( m_treePrefabs[ti], pos, Quaternion.identity, transform );
                tree.GetComponent<DestroyBelowHeight>().minHeight = m_region.waterLevel + 1.0f;
                ++count;
            }
        }

        //Debug.LogFormat( "{0}% chance on {2} size tiles = {1} trees", m_treeChance, count, m_treeTileSize );
    }
    */
}

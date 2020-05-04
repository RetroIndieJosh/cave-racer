using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Experimental.PostProcessing;

class Tile
{
    public GameObject tileObj;
    public float creationTime;

    public Tile(GameObject tileObj, float creationTime) {
        this.tileObj = tileObj;
        this.creationTime = creationTime;
    }
}

public class GenerateInfinite : MonoBehaviour
{
    /*
    [SerializeField]
    private GenerateRegion m_defaultRegion;

    [SerializeField]
    private List<GenerateRegion> m_additionalRegionList;

    [SerializeField]
    private int m_seed = 0;

    [SerializeField]
    private GameObject plane;

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private int m_planeSizeUnits = 10;

    [SerializeField]
    [Tooltip("How many planes the player can see around at any time")]
    private int m_visionRadius = 10;

    [SerializeField]
    private float m_playerViewOffset = 0.0f;

    [SerializeField]
    [Tooltip("Size of the world in planes")]
    private float m_worldSizePlanes = 500;

    [SerializeField]
    private int m_heightScale = 10;

    [SerializeField]
    private int m_detailScale = 10;

    [SerializeField] private GameObject m_waterPrefab;

    [SerializeField]
    private LayerMask m_postProcessLayer;

    private bool loaded = false;

    public float worldSize {  get { return m_worldSizeUnits; } }
    private float m_worldSizeUnits;

    private Vector3 m_startPos;
    private Hashtable m_tiles = new Hashtable();

    public GenerateRegion playerRegion {  get { return m_playerRegion; } }
    private GenerateRegion m_playerRegion;

    // water effects
    private GameObject m_waterTop;
    private GameObject m_waterBottom;
    PostProcessVolume m_Volume;
    DepthOfField effect;

    private float m_regionSizeUnits;
    private int m_regionsAcross;

    public Vector3 worldPos( Vector3 a_pos) {
        return new Vector3( Mathf.Repeat( a_pos.x, m_worldSizeUnits ), a_pos.y, Mathf.Repeat( a_pos.z, m_worldSizeUnits ) );
    }

    GenerateRegion regionAt( Vector3 a_pos ) {
        var x = Mathf.Repeat( a_pos.x, m_worldSizeUnits );
        var z = Mathf.Repeat( a_pos.z, m_worldSizeUnits );

        var regionX = Mathf.FloorToInt( x / m_regionSizeUnits );
        var regionZ = Mathf.FloorToInt( z / m_regionSizeUnits );

        var regionId = regionZ * m_regionsAcross + regionX;
        return regionId == 0 ? m_defaultRegion : m_additionalRegionList[regionId - 1];
    }

    void createTile( Vector3 pos, float updateTime ) {
        var t = Instantiate( plane, pos, Quaternion.identity, transform );

        var generateTerrain = t.GetComponent<GenerateTerrain>();
        generateTerrain.heightScale = m_heightScale;
        generateTerrain.detailScale = m_detailScale;
        generateTerrain.worldSize = m_worldSizeUnits;

        if ( m_additionalRegionList.Count == 0 ) generateTerrain.region = m_defaultRegion;
        else generateTerrain.region = regionAt( pos );

        var tileName = generateTileName( pos );
        t.name = tileName;

        var tile = new Tile( t, updateTime );
        m_tiles.Add( tileName, tile );
    }

    void Start() {
        if( m_defaultRegion.texture == null ) {
            throw new UnityException( "Must have texture for default region of generate infinite." );
        }

        if ( !Utility.isPerfectSquare( m_additionalRegionList.Count + 1 ) ) {
            throw new UnityException( string.Format( "Region count must be perfect square. Have {0} regions defined.", m_additionalRegionList.Count + 1 ) );
        }

        GenerateRegion.s_defaultRegion = m_defaultRegion;

        m_worldSizeUnits = m_worldSizePlanes * m_planeSizeUnits;

        // calculate region sizes
        m_regionsAcross = Mathf.FloorToInt( Mathf.Sqrt( m_additionalRegionList.Count + 1 ) );
        m_regionSizeUnits = m_worldSizeUnits / m_regionsAcross;
        //Debug.LogFormat( "World size {3} units ({4} planes). {0} regions across, {1}x{2} per region.", m_regionsAcross, m_regionSizeUnits, m_regionSizeUnits, m_worldSizeUnits, m_worldSizePlanes );

        // calculate start position (center of map)
        var halfWorldSizeCoords = m_worldSizeUnits * 0.5f;
        m_startPos = new Vector3( halfWorldSizeCoords, 0, halfWorldSizeCoords );

        StartCoroutine( loadTerrain() );

        initWaterEffects();
    }

    void initWaterEffects() {
        effect = ScriptableObject.CreateInstance<DepthOfField>();
        effect.focusDistance.Override( 2 );
        effect.aperture.Override( 2.5f );

        var layerNum = Utility.layerMaskToLayerNum( m_postProcessLayer );
        m_Volume = PostProcessManager.instance.QuickVolume( layerNum, 100f, effect );
    }
    
    // TODO experiment with randomizing detailscales 
    IEnumerator loadTerrain() {
        Random.InitState( m_seed );

        transform.position = m_startPos;

        var updateTime = Time.realtimeSinceStartup;

        for ( int tileX = -m_visionRadius; tileX < m_visionRadius; ++tileX ) {
            for ( int tileZ = -m_visionRadius; tileZ < m_visionRadius; ++tileZ ) {
                var pos = new Vector3( tileX * m_planeSizeUnits + m_startPos.x,
                    0,
                    tileZ * m_planeSizeUnits + m_startPos.z );

                createTile( pos, updateTime );

                var playerPos = player.transform.position;
                player.transform.position = new Vector3( m_startPos.x, 60, m_startPos.z );
            }
            //yield return null;
        }

        m_playerRegion = regionAt( m_startPos );
        if ( m_playerRegion.hasWater ) {
            //var waterPos = new Vector3( m_startPos.x, m_waterHeight, m_startPos.y );
            var waterPos = new Vector3( transform.position.x, m_playerRegion.waterLevel, transform.position.z );
            m_waterTop = Instantiate( m_waterPrefab, waterPos, Quaternion.identity, transform );
            m_waterBottom = Instantiate( m_waterPrefab, waterPos, Quaternion.identity, transform );

            var scale = m_visionRadius * 2 * m_planeSizeUnits;
            m_waterTop.transform.localScale = new Vector3( scale, 1, scale );
            m_waterBottom.transform.localScale = new Vector3( scale, 1, scale );

            m_waterBottom.transform.Rotate( Vector3.forward * 180.0f );

            updateWater();

            // TODO optimize (maybe player should grab this from region? i.e. we just set region?)
            player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().WaterHeight = m_playerRegion.waterLevel;
        }

        loaded = true;

        // for when we don't have the one in the loop
        yield return null;
    }

    void Update() {
        if ( !loaded ) return;

        updateWater();

        var xMove = player.transform.position.x - m_startPos.x;
        var zMove = player.transform.position.z - m_startPos.z;

        // if we didn't move into a new region, don't bother making new tiles
        if ( Mathf.Abs( xMove ) < m_planeSizeUnits && Mathf.Abs( zMove ) < m_planeSizeUnits ) return;

        //
        // loop the player if they went around the world
        //

        var x = player.transform.position.x;
        var z = player.transform.position.z;

        var halfMax = m_worldSizeUnits * 2 * 0.5f;// * 1000;
        //Debug.Log( "Map loop at " + halfMax );
        if( x < -halfMax || x > halfMax || z < -halfMax || z > halfMax ) {
            x = Mathf.Repeat( halfMax + x, halfMax * 2 ) - halfMax;
            z = Mathf.Repeat( halfMax + z, halfMax * 2 ) - halfMax;
            //Debug.Log( "LOOP World size units: " + m_worldSizeUnits );
            player.transform.position = new Vector3( x, player.transform.position.y + 1.0f, z );
        }

        //
        // recreate the tiles on border
        //

        var updateTime = Time.realtimeSinceStartup;

        var playerX = Mathf.FloorToInt( player.transform.position.x / m_planeSizeUnits ) * m_planeSizeUnits;
        var playerZ = Mathf.FloorToInt( player.transform.position.z / m_planeSizeUnits ) * m_planeSizeUnits;

        for ( int tileX = -m_visionRadius; tileX < m_visionRadius; ++tileX ) {
            for ( int tileZ = -m_visionRadius; tileZ < m_visionRadius; ++tileZ ) {
                var pos = new Vector3( tileX * m_planeSizeUnits + playerX,
                    0,
                    tileZ * m_planeSizeUnits + playerZ );
                var tileName = generateTileName( pos );

                if ( m_tiles.ContainsKey( tileName ) ) ( m_tiles[tileName] as Tile ).creationTime = updateTime;
                else createTile( pos, updateTime );
            }
        }

        var newTerrain = new Hashtable();
        foreach( Tile tile in m_tiles.Values ) {
            if( tile.creationTime != updateTime ) {
                Destroy( tile.tileObj );
                continue;
            } 

            newTerrain.Add( tile.tileObj.name, tile );
        }

        m_tiles = newTerrain;
        m_startPos = player.transform.position;

        m_playerRegion = regionAt( m_startPos );

        // snap water prefab to new player x/z
        if ( m_playerRegion.hasWater ) {
            m_waterTop.transform.position = new Vector3( playerX, m_playerRegion.waterLevel, playerZ );
            m_waterBottom.transform.position = new Vector3( playerX, m_playerRegion.waterLevel, playerZ );

            // TODO optimize (maybe player should grab this from region? i.e. we just set region?)
            player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().WaterHeight = m_playerRegion.waterLevel;
        }

    }

    private string generateTileName( Vector3 a_pos ) {
        return "Tile_" + a_pos.x + "_" + a_pos.z;
    }

    private void updateWater() {
        if ( !m_playerRegion.hasWater ) return;

        var waterLevel = m_playerRegion.waterLevel;
        if ( player.transform.position.y + m_playerViewOffset < waterLevel ) {

            // underwater
            m_waterTop.GetComponent<Renderer>().enabled = false;
            m_waterBottom.GetComponent<Renderer>().enabled = true;

            effect.enabled.Override( true );
            var distanceUnder = waterLevel - player.transform.position.y;
            if( effect ) effect.focusDistance.value = 4 - Mathf.Min( distanceUnder * 0.3f, 2 );
        } else {

            // over water
            m_waterTop.GetComponent<Renderer>().enabled = true;
            m_waterBottom.GetComponent<Renderer>().enabled = false;
            if( effect ) effect.enabled.Override( false );
        }
    }

    private void OnDestroy() {
        if ( m_Volume == null ) return;
        RuntimeUtilities.DestroyVolume( m_Volume, true );
    }
    */
}

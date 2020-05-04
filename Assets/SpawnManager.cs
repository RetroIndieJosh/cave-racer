using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance = null;

    private void Awake() {
        instance = this;
    }

    public void SpawnAfter(GameObject a_object, Vector3 a_pos, float a_seconds, Transform a_parent = null) {
        StartCoroutine( SpawnAfterCoroutine( a_object, a_pos, a_seconds, a_parent ) );
    }

    private IEnumerator SpawnAfterCoroutine( GameObject a_object, Vector3 a_pos, float a_seconds, 
        Transform a_parent ) {

        var obj = Instantiate( a_object, a_pos, Quaternion.identity, a_parent );
        obj.SetActive( false );

        yield return new WaitForSeconds( a_seconds );
        obj.SetActive( true );
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Pickup : MonoBehaviour
{
    abstract protected void OnPickup( Racer a_racer );

    private void OnTriggerEnter( Collider other ) {
        var racer = other.GetComponent<Racer>();
        OnPickup( racer );

        var audioSource = GetComponent<AudioSource>();
        if ( audioSource != null ) AudioSource.PlayClipAtPoint( audioSource.clip, transform.position );

        SpawnManager.instance.SpawnAfter( gameObject, transform.position, 30.0f );
        Destroy( gameObject );
    }
}

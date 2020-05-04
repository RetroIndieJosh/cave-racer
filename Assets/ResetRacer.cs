using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetRacer : MonoBehaviour
{
    private void OnTriggerEnter( Collider other ) {
        var racer = other.GetComponent<Racer>();
        if ( racer != null ) racer.ReturnToCheckpoint();

        var aiRacer = other.GetComponent<AiRacer>();
        if ( aiRacer != null ) aiRacer.ReturnToCheckpoint();
    }
}

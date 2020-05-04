using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSpeedPickup : Pickup
{
    [SerializeField]
    private float m_bonus = 1.0f;

    protected override void OnPickup( Racer a_racer ) {
        a_racer.TurnSpeedBonus += m_bonus;
    }
}

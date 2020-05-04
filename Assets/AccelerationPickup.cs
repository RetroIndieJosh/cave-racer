using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerationPickup : Pickup
{
    [SerializeField]
    private float m_accelBonus = 1.0f;

    [SerializeField]
    private float m_speedBonus = 1.0f;

    protected override void OnPickup( Racer a_racer ) {
        a_racer.AccelerationBonus += m_accelBonus;
        a_racer.MaxSpeedBonus += m_speedBonus;
    }
}

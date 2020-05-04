using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    [SerializeField]
    private float m_speed;

    private void Update() {
        transform.Rotate( Vector3.up, m_speed );
    }
}

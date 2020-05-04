using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField]
    private Checkpoint m_nextCheckpoint = null;

    [SerializeField]
    private bool m_isEnd = false;

    [SerializeField]
    private GameObject m_respawnPoint;

    public Vector3 RespawnPoint { get { return m_respawnPoint.transform.position; } }
    public Vector3 RespawnFacing {  get { return m_respawnPoint.transform.forward; } }

    public bool IsEnd { get { return m_isEnd; } }
}

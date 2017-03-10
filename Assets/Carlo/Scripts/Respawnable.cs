using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawnable : MonoBehaviour {

    [SerializeField]
    private string m_respawnTag = "DeathZone";
    private bool m_canRespawn = false;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag + " " + other.name);
        if (other.CompareTag(m_respawnTag))
        {
            m_canRespawn = true;
        }
        Debug.Log("Can respawn? : " + m_canRespawn);
    }

    public bool CanBeRespawned()
    {
        return m_canRespawn;
    }

    public void ResetCanRespawn()
    {
        m_canRespawn = false;
    }
}

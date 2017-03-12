using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour {

    [SerializeField]
    private Respawnable[] m_respawnableObjects;
    [SerializeField]
    private Transform m_respawnLocation;

	void Update ()
    {
		foreach(Respawnable r in m_respawnableObjects)
        {
            if(r.CanBeRespawned())
            {
                r.ResetCanRespawn();
                StartCoroutine(Respawn(r));
            }
        }
	}

    private IEnumerator Respawn(Respawnable r)
    {
        yield return new WaitForSeconds(2);
        r.transform.position = m_respawnLocation.position;
        r.transform.localScale = r.GetDefaultScale();
        r.transform.rotation = r.GetDefaultRotation();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BeaconSurfaceTrigger : MonoBehaviour {

    public bool m_canTeleport = true;
    private WaitForFixedUpdate m_fixedWait;
    private BoxCollider m_testTrigger;

    void Start()
    {
        m_testTrigger = GetComponent<BoxCollider>();
        m_fixedWait = new WaitForFixedUpdate();
        m_testTrigger.isTrigger = true;
        m_testTrigger.size = Vector3.zero;
    }

	void FixedUpdate ()
    {
        m_canTeleport = true;
    }

    IEnumerator OnTriggerStay()
    {
        yield return m_fixedWait;
        m_canTeleport = false;
    }

    public bool CanTeleport()
    {
        return m_canTeleport;
    }
}

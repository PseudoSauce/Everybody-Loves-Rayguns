using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeaconSurface : MonoBehaviour {

    [SerializeField]
    private Transform m_teleportTransform = null;
    private GameObject m_childBeacon = null;
    private BoxCollider m_testTrigger = null;
    private BeaconSurfaceTrigger m_testTriggerParent = null;
    private bool m_canTeleport = true;
    private WaitForFixedUpdate m_fixedWait;

    void Start()
    {
        m_testTriggerParent = GetComponentInChildren<BeaconSurfaceTrigger>();
        m_testTrigger = m_testTriggerParent.GetComponent<BoxCollider>();
        m_testTrigger.isTrigger = true;
        m_testTrigger.size = Vector3.zero;
        m_fixedWait = new WaitForFixedUpdate();
        StartCoroutine(TriggerCheck());
    }

    private IEnumerator TriggerCheck()
    {
        yield return m_fixedWait;
        m_canTeleport = m_testTriggerParent.CanTeleport();
    }

    public void AttachBeacon(GameObject beacon)
    {
        if(m_childBeacon != null)
        {
            Destroy(m_childBeacon); // TODO: Call destroy function on beacon instead of this
            m_childBeacon = beacon;
        }
        else
        {
            m_childBeacon = beacon;
        }
        m_childBeacon.transform.parent = this.transform;
    }

    public void DetachBeacon()
    {
        if(m_childBeacon != null)
        {
            Destroy(m_childBeacon); // TODO: Call destroy function on beacon instead of this
            m_childBeacon.transform.parent = null;
            m_childBeacon = null;
        }
    }

    public Transform GetTeleportTranform()
    {
        return m_teleportTransform;
    }

    public void SetColliderSize(Renderer teleportedObject)
    {
        float boxExtent = teleportedObject.bounds.extents.magnitude * 2;
        m_testTrigger.size = Vector3.one * boxExtent;
        Vector3 boxCenter = m_childBeacon.transform.position;
        boxCenter.z += boxExtent * 0.5f;
        m_testTrigger.center = boxCenter;
    }

    public bool CanTeleport()
    {
        return m_canTeleport;
    }
}

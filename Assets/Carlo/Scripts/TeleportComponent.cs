using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MyTypes;

public class TeleportComponent : Interactable {

    private Beacon m_beaconTarget;
    private bool m_canTeleport = false;

    private MeshFilter m_meshFilter;
    private MeshRenderer m_meshRenderer;
    private Rigidbody m_rigidbody;

    protected override void Init()
    {
        AssignInteractionType(Interaction.TELEPORTING);
        AssignStart(MyStart);
        AssignUpdate(MyUpdate);
    }

    private void MyStart()
    {
        m_meshFilter = GetComponent<MeshFilter>();
        m_meshRenderer = GetComponent<MeshRenderer>();
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void MyUpdate(float deltaTime)
    {
        SendMesh();
    }

    // place your custom logic here for interaction
    protected override void Commit(string msg)
    {
        Debug.Log(this + ": " + msg);

        switch(msg)
        {
            case "HitBegin":
                HitBegin(FindObjectOfType<Beacon>());
                break;
            case "HitEnd":
                HitEnd();
                break;
            case "Teleport":
                Teleport(FindObjectOfType<Beacon>());
                break;
        }
    }

    private void HitBegin(Beacon beacon)
    {
        m_beaconTarget = beacon;
    }

    private void HitEnd()
    {
        m_beaconTarget.StopHologram();
        m_beaconTarget = null;
    }

    private void SendMesh()
    {
        if(m_beaconTarget != null)
        {
            m_beaconTarget.SendMesh(m_meshFilter.mesh, m_meshRenderer.bounds.extents, transform.localScale);
        }
    }

    private void Teleport(Beacon beacon)
    {
        m_beaconTarget = beacon;

        if(m_beaconTarget != null)
        {
            if(m_beaconTarget.CanTeleport())
            {
                transform.position = m_beaconTarget.GetTeleportPosition(m_meshRenderer.bounds.extents);
                transform.rotation = m_beaconTarget.GetRotation();
                m_rigidbody.velocity = Vector3.zero;
            }
        }
    }
}

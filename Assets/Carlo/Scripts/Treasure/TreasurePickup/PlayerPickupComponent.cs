using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickupComponent : MonoBehaviour {
    [SerializeField]
    private Transform m_holdPosition;
    private TreasurePickup m_treasure = null;

    private bool m_isHolding = false;
    private bool m_mousePressed = false;

    [SerializeField]
    private LayerMask m_layerMask;
    public Transform m_raycaster;

	void Update ()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_mousePressed = true;
        }
	}

    void FixedUpdate()
    {
        if (m_mousePressed)
        {
            Debug.Log("pressed button");
            Debug.Log(m_isHolding);
            if (!m_isHolding)
            {
                RaycastHit hitInfo;
                if (Physics.Raycast(m_raycaster.position, m_raycaster.forward, out hitInfo))
                {
                    Debug.Log(hitInfo.collider.name);
                    if (hitInfo.collider.GetComponent<TreasurePickup>())
                    {
                        Debug.Log("Pickup");
                        hitInfo.collider.isTrigger = true;
                        m_treasure = hitInfo.collider.GetComponent<TreasurePickup>();
                        hitInfo.collider.transform.position = m_holdPosition.position;
                        hitInfo.collider.transform.rotation = m_holdPosition.rotation;
                        m_treasure.SetKinematic();
                        m_treasure.tag = "Holden";
                        hitInfo.collider.transform.parent = m_holdPosition;
                        m_isHolding = true;
                    }
                }
            }
            else
            {
                Debug.Log("Dropoff");
                m_treasure.transform.parent = null;
                m_treasure.GetComponent<Collider>().isTrigger = false;
                m_treasure.Throw();
                m_treasure.tag = "PickUp";
                m_isHolding = false;
            }
            m_mousePressed = false;
        }
    }
}

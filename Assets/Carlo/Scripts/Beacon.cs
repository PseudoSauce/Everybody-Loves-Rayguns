using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class Beacon : MonoBehaviour {

    [SerializeField, Tooltip("Speed the beacon will shoot at")]
    private float beaconSpeed = 10.0f;
    [SerializeField, Tooltip("Object that will spawn to test the new area for a suitable teleport destination")]
    private BeaconTestObject m_testObject = null;

    private bool m_hasHit = false;
    private BeaconTestObject m_spawnedTestObject = null;
    private Rigidbody m_rb = null;

	void Start ()
    {
        m_rb = GetComponent<Rigidbody>();
        m_rb.AddForce(transform.forward * beaconSpeed, ForceMode.Impulse);
        StartCoroutine(DestroyAfterTime());
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.collider.CompareTag("TeleportSurface"))
        {
            m_rb.isKinematic = true;
            transform.parent = col.collider.gameObject.transform;
            transform.rotation = Quaternion.identity;
            float angle = Vector3.Angle(transform.forward, col.contacts[0].normal);

            if (Mathf.Abs(col.contacts[0].normal.y) > 0)
            {
                transform.Rotate(Vector3.right, -col.contacts[0].normal.y * angle);
            }
            else if(col.contacts[0].normal.x < 0)
            {
                transform.Rotate(Vector3.up, col.contacts[0].normal.x * angle);
            }
            else
            {
                transform.Rotate(Vector3.up, angle);
            }

            GetComponent<LineRenderer>().SetPosition(1, Vector3.forward * 10);
            m_hasHit = true;
            GetComponent<BoxCollider>().isTrigger = true;
        }
    }

    public bool CanTeleport(Vector3 extent)
    {
        bool result = false;

        if(m_hasHit)
        {
            if(m_spawnedTestObject != null)
            {
                // Destroy old test object
                Destroy(m_spawnedTestObject);
            }

            // First Check if there isn't anything directly in front of the beacon like a wall, ceiling or floor
            if(!Physics.Raycast(transform.position, transform.forward, (transform.forward * (extent.magnitude + extent.magnitude * 0.15f)).magnitude))
            {
                // Then check more specifically using the volume of desired teleport location (beacon test object does this)
                m_spawnedTestObject = Instantiate(m_testObject, transform.position, Quaternion.identity) as BeaconTestObject;
                m_spawnedTestObject.transform.position = transform.position + (transform.forward * (extent.magnitude + extent.magnitude * 0.15f));
                m_spawnedTestObject.transform.parent = transform;

                if (m_spawnedTestObject != null)
                {
                    if (m_spawnedTestObject.CanTeleport(extent))
                    {
                        result = true;
                    }
                }
            }

        }
        
        return result;
    }

    public Vector3 GetTeleportPosition(Vector3 extent)
    {
        return transform.position + (transform.forward * (extent.magnitude + extent.magnitude * 0.15f));
    }

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(2);
        if(!m_hasHit)
        {
            Destroy(this.gameObject);
        }
    }

}

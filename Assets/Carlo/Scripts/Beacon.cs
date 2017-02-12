using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class Beacon : MonoBehaviour {

    [SerializeField, Tooltip("Speed the beacon will shoot at")]
    private float beaconSpeed = 10.0f;
    [SerializeField, Tooltip("Object that will spawn to test the new area for a suitable teleport destination")]
    private BeaconTestObject m_testObject = null;
    private float m_rotationAngle = 0;

    private bool m_hasHit = false;
    [SerializeField]
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
            // Teleport the hologram to be identity
            if (m_spawnedTestObject != null)
            {
                m_spawnedTestObject.transform.localRotation = transform.rotation;
            }

            m_rb.isKinematic = true;
            transform.parent = col.collider.gameObject.transform;
            transform.rotation = Quaternion.identity;
            m_rotationAngle = Vector3.Angle(transform.forward, col.contacts[0].normal);

            if (Mathf.Abs(col.contacts[0].normal.y) > 0)
            {
                transform.Rotate(Vector3.right, -col.contacts[0].normal.y * m_rotationAngle);
            }
            else if(col.contacts[0].normal.x < 0)
            {
                transform.Rotate(Vector3.up, col.contacts[0].normal.x * m_rotationAngle);
            }
            else
            {
                transform.Rotate(Vector3.up, m_rotationAngle);
            }

            GetComponent<LineRenderer>().SetPosition(1, Vector3.forward * 10);
            m_hasHit = true;
            GetComponent<BoxCollider>().isTrigger = true;
        }
    }

    public bool CanTeleport(Vector3 extent)
    {
        bool result = false;

        if (m_spawnedTestObject != null)
        {
            // Destroy old test object
            Destroy(m_spawnedTestObject.gameObject);
        }

        if (m_hasHit)
        {
            // First Check if there isn't anything directly in front of the beacon like a wall, ceiling or floor
            if (!Physics.Raycast(transform.position, transform.forward, (transform.forward * (extent.magnitude + extent.magnitude * 0.15f)).magnitude))
            {
                // Then check more specifically using the volume of desired teleport location (beacon test object does this)
                m_spawnedTestObject = Instantiate(m_testObject, transform.position, Quaternion.identity) as BeaconTestObject;
                //m_spawnedTestObject.transform.Rotate(Vector3.up, m_rotationAngle);
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

    public void SendMesh(Mesh hologramMesh, Vector3 extent, Vector3 scale)
    {
        if(m_spawnedTestObject != null)
        {
            m_spawnedTestObject.ShowHologram(hologramMesh, extent, scale);
        }
        else
        {
            m_spawnedTestObject = Instantiate(m_testObject, transform.position, Quaternion.identity) as BeaconTestObject;
            //m_spawnedTestObject.transform.Rotate(Vector3.up, m_rotationAngle);
            m_spawnedTestObject.transform.position = transform.position + (transform.forward * (extent.magnitude + extent.magnitude * 0.15f));
            m_spawnedTestObject.transform.parent = transform;
            m_spawnedTestObject.ShowHologram(hologramMesh, extent, scale);
        }
    }

    public void StopHologram()
    {
        if (m_spawnedTestObject != null)
        {
            m_spawnedTestObject.StopHologram();
        }
    }

    public Vector3 GetTeleportPosition(Vector3 extent)
    {
        return transform.position + (transform.forward * (extent.magnitude + extent.magnitude * 0.15f));
    }

    public float GetRotationAngle()
    {
        return m_rotationAngle;
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

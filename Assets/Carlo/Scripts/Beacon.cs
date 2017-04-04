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

    [SerializeField, Tooltip("Colour to show that the object can be telported")]
    private Gradient m_goodColor;
    [SerializeField, Tooltip("Colour to show that the object CANNOT be telported")]
    private Gradient m_badColor;
    private LineRenderer m_lineRenderer = null;

    [SerializeField, Tooltip("Minimum distance from the beacon to spawn the object.")]
    private float m_minDistToBeacon = 2.5f;
    private BeaconTestObject m_spawnedTestObject = null;
    private Mesh m_lastMesh = null;
    private Rigidbody m_rb = null;

    public Beacon GetBeacon { get { return this; } }

	void Start ()
    {
        m_rb = GetComponent<Rigidbody>();
        m_rb.AddForce(transform.forward * beaconSpeed, ForceMode.Impulse);
        StartCoroutine(DestroyAfterTime());
        m_lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        // Set the color of the beacon
        if(m_spawnedTestObject != null && m_lineRenderer != null)
        {
            if(m_spawnedTestObject.CanTeleport())
            {
                m_lineRenderer.colorGradient = m_goodColor;
            }
            else
            {
                m_lineRenderer.colorGradient = m_badColor;
            }
        }
    }

    void OnCollisionEnter(Collision col)
    {
        // Stick the beacon to a seurface and rotate it to the correct to face the correct direction
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

    // Can the object be teleported?
    public bool CanTeleport()
    {
        if (m_hasHit)
            return m_spawnedTestObject.CanTeleport();
        else
            return false;
    }

    /// <summary>
    /// Show hologram of object to be teleported
    /// </summary>
    /// <param name="hologramMesh">Pass the mesh of the object to be teleported</param>
    /// <param name="extent">Pass the extent of the object to be teleported</param>
    /// <param name="scale">Pass the scale of the object to be teleported</param>
    public void SendMesh(Mesh hologramMesh, Vector3 extent, Vector3 scale, GameObject objectToTeleport)
    {
        m_lastMesh = hologramMesh;

        if(m_spawnedTestObject != null)
        {
            if (hologramMesh == m_lastMesh)
            {
                // Re-ajust position
                //m_spawnedTestObject.transform.position = transform.position + (transform.forward * (extent.magnitude + extent.magnitude * 0.15f));

                Vector3 newPos = transform.position + (transform.forward * (extent.magnitude + extent.magnitude * 0.15f)); ;

                if (Vector3.Distance(transform.position, newPos) < m_minDistToBeacon)
                {
                    newPos += transform.forward * m_minDistToBeacon;
                }

                m_spawnedTestObject.transform.position = newPos;
            }

            m_spawnedTestObject.ShowHologram(hologramMesh, extent, scale, objectToTeleport);
        }
        else
        {
            m_spawnedTestObject = Instantiate(m_testObject, transform.position, Quaternion.identity) as BeaconTestObject;
            m_spawnedTestObject.transform.position = transform.position + (transform.forward * (extent.magnitude + extent.magnitude * 0.15f));
            m_spawnedTestObject.transform.parent = transform;
            m_spawnedTestObject.ShowHologram(hologramMesh, extent, scale, objectToTeleport);
        }
    }

    // Stop showing the hologram
    public void StopHologram()
    {
        if (m_spawnedTestObject != null)
        {
            m_spawnedTestObject.StopHologram();
        }
    }

    // Returns the teleport position
    public Vector3 GetTeleportPosition(Vector3 extent)
    {
        Vector3 newPos = transform.position + (transform.forward * (extent.magnitude + extent.magnitude * 0.15f)); ;

        if(Vector3.Distance(transform.position, newPos) <  m_minDistToBeacon)
        {
            newPos += transform.forward * m_minDistToBeacon;
        }

        return newPos;
    }

    // Returns the rotation of the test object
    public Quaternion GetRotation()
    {
        if (m_spawnedTestObject != null)
        {
            return m_spawnedTestObject.transform.rotation;
        }
        else
        {
            return Quaternion.identity;
        }
    }

    // Rotate the test object around the X axis by 90 degrees
    public void RotateOnXAxis()
    {
        if (m_spawnedTestObject != null)
        {
            m_spawnedTestObject.transform.Rotate(Vector3.right, 90.0f);
        }
    }

    // Rotate the test object around the Y axis by 90 degrees
    public void RotateOnYAxis()
    {
        if (m_spawnedTestObject != null)
        {
            m_spawnedTestObject.transform.Rotate(Vector3.up, 90.0f);
        }
    }

    // Destroy the beacon after a time if the beacon has not stuck to a wall
    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(2);
        if(!m_hasHit)
        {
            Destroy(this.gameObject);
        }
    }

}

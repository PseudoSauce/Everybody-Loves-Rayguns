using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class Beacon : MonoBehaviour {

    [SerializeField]
    private float beaconSpeed = 10.0f;
    [SerializeField]
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
        }
    }

    public bool CanTeleport(float bounds)
    {
        if(m_hasHit)
        {
            m_spawnedTestObject = Instantiate(m_testObject, transform.position, Quaternion.identity) as BeaconTestObject;
            m_spawnedTestObject.transform.position = transform.position + (transform.forward * bounds);
            m_spawnedTestObject.SetColliderSize(Vector3.one * Mathf.Ceil(bounds));
            m_spawnedTestObject.transform.parent = transform;
            if (m_spawnedTestObject != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
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

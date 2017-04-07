using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TreasurePickup : MonoBehaviour {
    private Rigidbody m_rb;

    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        m_rb.isKinematic = true;
    }
    
    public void SetKinematic()
    {
        m_rb.isKinematic = true;
    }

    public void Throw()
    {
        m_rb.isKinematic = false;
        m_rb.AddForce(transform.forward * 10, ForceMode.Impulse);
    }
}

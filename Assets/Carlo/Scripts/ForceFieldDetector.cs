using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFieldDetector : MonoBehaviour
{

    private Raygun m_raygun;

    void Start()
    {
        m_raygun = GetComponentInChildren<Raygun>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ForceField"))
        {
            Debug.Log("Force Field");
            m_raygun.EnterLevelEntranceTrigger(other.GetComponent<LevelStart>().GetRotationMarker());
        }
    }
}

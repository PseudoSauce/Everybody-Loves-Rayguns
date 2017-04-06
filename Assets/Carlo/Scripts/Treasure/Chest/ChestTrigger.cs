using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Collider), typeof(Rigidbody))]
public class ChestTrigger : MonoBehaviour {

    private Animator m_animator;

	void Start ()
    {
        m_animator = GetComponent<Animator>();
	}
    
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            m_animator.SetTrigger("Open");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            m_animator.SetTrigger("Close");
        }
    }
}

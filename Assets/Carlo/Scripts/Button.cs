using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Collider), typeof(Rigidbody))]
public class Button : ActivatorObject {


    [SerializeField]
    private string m_animationBool = "";
    private Animator m_animator;

    void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Interactable>())
        {
            PressButton();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Interactable>())
        {
            UnPressButton();
        }
    }

    public void PressButton()
    {
        base.Activate();
        m_animator.SetBool(m_animationBool, true);
    }

    public void UnPressButton()
    {
        base.Deactivate();
        m_animator.SetBool(m_animationBool, false);
    }

}

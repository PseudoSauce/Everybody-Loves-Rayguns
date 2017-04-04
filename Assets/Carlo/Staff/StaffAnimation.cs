using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MyTypes;

public class StaffAnimation : MonoBehaviour {

    private Animator m_animator;

    [SerializeField]
    private string m_shrinkMode = "ShrinkMode";
    [SerializeField]
    private string m_shrink = "Shrinking";
    [SerializeField]
    private string m_grow = "Growing";
    [SerializeField]
    private string m_teleportMode = "TeleportMode";
    [SerializeField]
    private string m_throwBeacon = "ThrowBeacon";
    [SerializeField]
    private string m_teleport = "Teleport";
    [SerializeField]
    private string m_walk = "Walking";


    void Start ()
    {
        m_animator = GetComponent<Animator>();
	}

    public void SwitchToMode(GunMode newMode)
    {
        switch(newMode)
        {
            case GunMode.Scaler:
                m_animator.SetBool(m_walk, false);
                m_animator.SetBool(m_shrinkMode, true);
                m_animator.SetBool(m_teleportMode, false);
                break;
            case GunMode.Teleporter:
                m_animator.SetBool(m_walk, false);
                m_animator.SetBool(m_shrinkMode, false);
                m_animator.SetBool(m_teleportMode, true);
                m_animator.SetBool(m_shrink, false);
                m_animator.SetBool(m_grow, false);
                break;
        }
    }

    public void Shrink(bool b)
    {
        m_animator.SetBool(m_walk, false);
        m_animator.SetBool(m_shrink, b);
    }

    public void Grow(bool b)
    {
        m_animator.SetBool(m_walk, false);
        m_animator.SetBool(m_grow, b);
    }

    public void ThrowBeacon()
    {
        m_animator.SetBool(m_walk, false);
        m_animator.SetTrigger(m_throwBeacon);
    }

    public void Teleport()
    {
        m_animator.SetBool(m_walk, false);
        m_animator.SetTrigger(m_teleport);
    }
}

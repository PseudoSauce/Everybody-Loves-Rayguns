using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatableObject : MonoBehaviour {
    [SerializeField]
    protected bool m_isOneShot = false;
    [SerializeField]
    protected bool m_activateAfterTime = false;
    [SerializeField]
    protected float m_activateTime = 2.0f;

    protected Coroutine m_timedCoroutine;
    protected bool m_isActive = false;

    virtual public void ActivateObject() { }
    virtual public void DeactivateObject() { }

    virtual public void Call(string method, params object[] list) { }

}

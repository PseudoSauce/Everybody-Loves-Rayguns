using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatableObject : MonoBehaviour {
    [SerializeField]
    protected bool m_isOneShot = false;

    virtual public void ActivateObject() { }
    virtual public void DeactivateObject() { }

    virtual public void Call(string method, params object[] list) { }

}

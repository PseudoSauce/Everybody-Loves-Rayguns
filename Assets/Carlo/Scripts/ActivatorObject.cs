using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatorObject : MonoBehaviour {

    [SerializeField]
    protected List<ActivatableObject> m_activatableObjects;

    virtual protected void Activate()
    {
        foreach(ActivatableObject a in m_activatableObjects)
        {
            a.ActivateObject();
        }
    }

    virtual protected void Deactivate()
    {
        foreach (ActivatableObject a in m_activatableObjects)
        {
            a.DeactivateObject();
        }
    }

}

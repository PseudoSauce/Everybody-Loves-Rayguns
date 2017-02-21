using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatableObject : MonoBehaviour {

    virtual public void ActivateObject() { }
    virtual public void DeactivateObject() { }

    virtual public void Call(string method, params object[] list) { }

}

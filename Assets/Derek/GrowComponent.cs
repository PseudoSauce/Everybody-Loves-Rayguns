using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MyTypes;

//[------------------------------------]
// Works as a toggle for growing. It receives
// a message to start growing, and a message to stop.
//[------------------------------------]
public class GrowComponent : Interactable {
    private bool m_Growing;

    protected override void Init()
    {
        AssignInteractionType(Interaction.GROWING);
        AssignStart(MyStart);
        AssignUpdate(MyUpdate);
    }

    private void MyStart()
    {
        Debug.Log("GrowComponent: Starting...");
    }

    private void MyUpdate(float deltaTime)
    {
        gameObject.transform.Translate(Vector3.forward*deltaTime*20);
    }

    // place your custom logic here for interaction
    protected override void Commit(string msg)
    {
        Debug.Log(this + ": " + msg);
    }
}

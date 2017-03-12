using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(InputState))]
public abstract class AbstractBehaviour : MonoBehaviour
{

    public Buttons[] inputButtons;

    protected InputState inputState;

    protected virtual void Awake()
    {
        inputState = GetComponent<InputState>();
    }
}

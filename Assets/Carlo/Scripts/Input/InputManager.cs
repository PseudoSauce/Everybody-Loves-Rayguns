using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Buttons
{
    Accept, // A
    Cancel, // B
    Pause, // Start

    PrimaryFire, // Right trigger
    SecondaryFire, // Left trigger
    SwapWeaponRight, // Right trigger
    SwapWeaponLeft, // Left d-pad
    ToggleWeapon,   // Tab

    RotateX, // Right bumper
    RotateY, // Left bumper
}

public enum Condition
{
    GreaterThan,
    LessThan
}

[System.Serializable]
public class InputAxisState
{
    public string axisName;
    public float offValue;
    public Buttons button;
    public Condition condition;

    public bool value
    {
        get
        {
            var val = Input.GetAxis(axisName);

            switch (condition)
            {
                case Condition.GreaterThan:
                    {
                        if(!onDown)
                            onDown = true;
                        return val > offValue;
                    }
                case Condition.LessThan:
                    return val < offValue;
            }

            return false;
        }
    }

    private bool onDown;

    public bool downValue
    {
        get
        {
            return onDown;
        }
    }

    public IEnumerator OnButtonDown()
    {
        yield return new WaitForEndOfFrame();
        onDown = false;
    }
}

public class InputManager : MonoBehaviour
{

    public InputAxisState[] inputs;
    public InputState[] inputStates;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        foreach (var state in inputStates)
        {
            foreach (var input in inputs)
            {
                state.SetButtonValue(input.button, input.value, input.downValue);
                
                if(input.downValue == true)
                {
                    StartCoroutine(input.OnButtonDown());
                }
            }
        }
    }
}

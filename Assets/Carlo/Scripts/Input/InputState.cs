using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonState
{
    public bool value;
    public float holdTime = 0;
    public bool onHold;
}

public class InputState : MonoBehaviour
{
    private Dictionary<Buttons, ButtonState> buttonStates = new Dictionary<Buttons, ButtonState>();

    public void SetButtonValue(Buttons key, bool value, bool onHold)
    {
        if (!buttonStates.ContainsKey(key))
            buttonStates.Add(key, new ButtonState());

        var state = buttonStates[key];

        if (state.value && !value)
        {
            //Debug.Log("Button " + key + " released " + state.holdTime);
            state.holdTime = 0;
        }
        else if (state.value && value)
        {
            state.holdTime += Time.deltaTime;
            //Debug.Log("Button " + key + " down " + state.holdTime);
        }

        state.value = value;

        state.onHold = onHold;
    }

    public bool GetButtonDown(Buttons key)
    {
        if (buttonStates.ContainsKey(key))
        {
            if (buttonStates[key].value == true && buttonStates[key].holdTime < 0.22f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
            return false;
    }

    public bool GetButtonHold(Buttons key)
    {
        if (buttonStates.ContainsKey(key))
            return buttonStates[key].value;
        else
            return false;
    }

    public float GetButtonHoldTime(Buttons key)
    {
        if (buttonStates.ContainsKey(key))
            return buttonStates[key].holdTime;
        else
            return 0;
    }
}

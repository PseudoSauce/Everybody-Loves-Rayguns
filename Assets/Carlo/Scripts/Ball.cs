using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

    [SerializeField]
    private BallColours m_colour = BallColours.Red;

    public BallColours GetColour()
    {
        return m_colour;
    }
}

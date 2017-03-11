using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallReceptor : MonoBehaviour {

    [SerializeField]
    private BallColours m_colour = BallColours.Red;
    private bool m_isComplete = false;

    void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Ball>())
        {
            if(m_colour == other.GetComponent<Ball>().GetColour())
            {
                m_isComplete = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Ball>())
        {
            if (m_colour == other.GetComponent<Ball>().GetColour())
            {
                m_isComplete = false;
            }
        }
    }

    public bool IsComplete()
    {
        return m_isComplete;
    }
}

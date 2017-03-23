using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookAtTrigger : MonoBehaviour {
    // Store all the camera moves here
    [SerializeField, Tooltip("The moves the camera will make in the order it will make them")]
    private CameraMove[] m_cameraMoves;
    // Has the trigger already been used
    private bool m_isDone = false;

    // Get all the camera moves from this trigger
    public CameraMove[] GetCameraMoves()
    {
        m_isDone = true;
        return m_cameraMoves;
    }

    // Have the camera moves been executed already?
    public bool IsDone()
    {
        return m_isDone;
    }
}

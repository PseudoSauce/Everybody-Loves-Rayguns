using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookAtTrigger : MonoBehaviour {

    [SerializeField]
    private CameraMove[] m_cameraMoves;

    public CameraMove[] GetCameraMoves()
    {
        return m_cameraMoves;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStart : MonoBehaviour {
    
    [SerializeField]
    private Transform m_levelRotationMarker = null;

    public Transform GetRotationMarker()
    {
        return m_levelRotationMarker;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeaconTestObject : MonoBehaviour {

    [SerializeField]
    [Range(0, 1.0f)]
    [Tooltip("Percentage of the extent added to the extent when drawing rays. Padding to make sure the object will fit in the new space.")]
    private float m_extentPercentage = 0.05f;
    private bool m_canDrawRays = false;
    private float m_longestExtent;
    private Vector3 [] m_directions = new Vector3[] { Vector3.up, Vector3.left, Vector3.right, Vector3.forward, Vector3.back, Vector3.down };

    public bool CanTeleport(Vector3 extent)
    {
        m_longestExtent = extent.x;
        if (extent.y > m_longestExtent)
            m_longestExtent = extent.y;
        if (extent.z > m_longestExtent)
            m_longestExtent = extent.z;
        // Add extra percentage to the length of the bounds to have a little breathing room
        m_longestExtent = m_longestExtent + m_longestExtent * m_extentPercentage;

        m_canDrawRays = true;

        bool result = true;

        for (int i = 0; i < m_directions.Length; i++)
        {
            Ray ray = new Ray(transform.position, m_directions[i]);
            Ray inverseRay = new Ray(transform.position + (m_directions[i] * m_longestExtent), -m_directions[i]);
            if (Physics.Raycast(ray, m_longestExtent))
            {
                result = false;
                break;
            }

            // Check inverse rays in case the test object is in a wall
            // Though the check in beacon should prevent the need of this check
            // Kept here just in case
            if (Physics.Raycast(inverseRay, m_longestExtent))
            {
                result = false;
                break;
            }

        }

        return result;
    }

    void Update()
    {
        // For debugging
        if (m_canDrawRays)
        {
            for (int i = 0; i < m_directions.Length; i++)
            {
                Debug.DrawRay(transform.position, m_directions[i] * m_longestExtent, Color.red);
                Debug.DrawRay(transform.position + (m_directions[i] * m_longestExtent), -m_directions[i] * m_longestExtent, Color.blue);
            }
        }
    }
}

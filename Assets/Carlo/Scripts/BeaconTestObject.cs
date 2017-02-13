using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeaconTestObject : MonoBehaviour {

    // Area/Volume Check
    [SerializeField]
    [Range(0, 1.0f)]
    [Tooltip("Percentage of the extent added to the extent when drawing rays. Padding to make sure the object will fit in the new space.")]
    private float m_extentPercentage = 0.05f;
    private bool m_canDrawRays = false;
    private float m_longestExtent;
    private Vector3 [] m_directions = new Vector3[] { Vector3.up, Vector3.down,  Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

    // Hologram
    [SerializeField]
    private Color m_goodColour = new Color(0.15f, 0.1f, 0.9f, 0.15f);
    [SerializeField]
    private Color m_badColour = new Color(0.9f, 0.2f, 0.0f, 0.15f);
    private MeshFilter m_mesh;
    private MeshRenderer m_meshRenderer;

    void Start()
    {
        m_mesh = GetComponent<MeshFilter>();
        m_meshRenderer = GetComponent<MeshRenderer>();
    }

    float[] extents;
    public bool CanTeleport(Vector3 extent)
    {
        //m_longestExtent = extent.x;
        //if (extent.y > m_longestExtent)
        //    m_longestExtent = extent.y;
        //if (extent.z > m_longestExtent)
        //    m_longestExtent = extent.z;
        //// Add extra percentage to the length of the bounds to have a little breathing room
        //m_longestExtent = m_longestExtent + m_longestExtent * m_extentPercentage;

        float extentX = extent.y + extent.y * m_extentPercentage;
        float extentY = extent.x + extent.x * m_extentPercentage;
        float extentZ = extent.z + extent.z * m_extentPercentage;

        extents = new float[]{ extentX, extentX, extentY, extentY, extentZ, extentZ };

        m_canDrawRays = true;

        bool result = true;

        for (int i = 0; i < m_directions.Length; i++)
        {
            Ray ray = new Ray(transform.position, m_directions[i]);
            Ray inverseRay = new Ray(transform.position + (m_directions[i] * extents[i]), -m_directions[i]);
            if (Physics.Raycast(ray, extents[i]))
            {
                result = false;
                break;
            }

            // Check inverse rays in case the test object is in a wall
            // Though the check in beacon should prevent the need of this check
            // Kept here just in case
            if (Physics.Raycast(inverseRay, extents[i]))
            {
                result = false;
                break;
            }

        }

        return result;
    }

    public void ShowHologram(Mesh hologramMesh, Vector3 extent, Vector3 scale)
    {
        if(m_mesh != null)
        {
            if(CanTeleport(extent))
            {
                m_meshRenderer.material.color = m_goodColour;
            }
            else
            {
                m_meshRenderer.material.color = m_badColour;
            }
            m_mesh.mesh = hologramMesh;
            transform.localScale = scale * 5.0f;
        }
    }

    public void StopHologram()
    {
        transform.localScale = Vector3.zero;
    }

    void Update()
    {
        // For debugging
        if (m_canDrawRays)
        {
            for (int i = 0; i < m_directions.Length; i++)
            {
                Debug.DrawRay(transform.position, m_directions[i] * extents[i], Color.red);
                //Debug.DrawRay(transform.position + (m_directions[i] * extents[i]), -m_directions[i] * extents[i], Color.blue);
            }
        }
    }
}

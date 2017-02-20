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
    private Vector3[] m_directions;
    private bool m_canTeleport = false;
    private float[] m_extents;
    private float m_extentX, m_extentY, m_extentZ;

    // Hologram
    [SerializeField, Tooltip("Colour to show that the object can be telported")]
    private Color m_goodColour = new Color(0.15f, 0.1f, 0.9f, 0.15f);
    [SerializeField, Tooltip("Colour to show that the object CANNOT be telported")]
    private Color m_badColour = new Color(0.9f, 0.2f, 0.0f, 0.15f);
    private MeshFilter m_mesh;
    private MeshRenderer m_meshRenderer;

    void Start()
    {
        m_mesh = GetComponent<MeshFilter>();
        m_meshRenderer = GetComponent<MeshRenderer>();
        m_directions = new Vector3[] { transform.up, transform.right, transform.forward,
                                       -transform.up, -transform.right, -transform.forward };
    }
   
    void Update()
    {
        // For debugging
        if (m_canDrawRays)
        {
            for (int i = 0; i < m_directions.Length; i++)
            {
                // Show the rays being  shot ot to check the volume of the new location
                // Regular rays
                Debug.DrawRay(transform.position, m_directions[i] * m_extents[i], Color.red);
                // Inverse Rays
                //Debug.DrawRay(transform.position + (m_directions[i] * extents[i]), -m_directions[i] * extents[i], Color.blue);
            }
        }
    }

    // Pass in the extent of object ot teleport.  This will check if it is possible to teleport.
    private bool CanTeleport(Vector3 extent, GameObject objectToTeleport)
    {
        m_canDrawRays = true;   // For debug draw rays
        bool result = true;

        // Set the extent of the check
        m_extentX = extent.y + extent.y * m_extentPercentage;
        m_extentY = extent.x + extent.x * m_extentPercentage;
        m_extentZ = extent.z + extent.z * m_extentPercentage;

        m_extents = new float[]{ m_extentX, m_extentY, m_extentZ,
                                 m_extentX, m_extentY, m_extentZ };
        m_directions = new Vector3[] { transform.up, transform.right, transform.forward,
                                       -transform.up, -transform.right, -transform.forward };

        RaycastHit hitInfo;
        
        // Cast rays and inverse rays from the centre of the test object
        for (int i = 0; i < m_directions.Length; i++)
        {
            Ray ray = new Ray(transform.position, m_directions[i]);
            Ray inverseRay = new Ray(transform.position + (m_directions[i] * m_extents[i]), -m_directions[i]);

            // Regular rayast check
            if (Physics.Raycast(ray, out hitInfo, m_extents[i]))    // This might cause issues
            {
                if (hitInfo.collider.gameObject != objectToTeleport)
                {
                    result = false;
                    break;
                }
            }
            // Check inverse rays in case the test object is in a wall
            // Though the check in beacon should prevent the need of this check
            // Kept here just in case
            if (Physics.Raycast(inverseRay, out hitInfo, m_extents[i]))
            {
                if (hitInfo.collider.gameObject != objectToTeleport)    // This might cause issues
                {
                    result = false;
                    break;
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Show the hologram of potential teleport
    /// </summary>
    /// <param name="hologramMesh">Pass the mesh of the object to be teleported</param>
    /// <param name="extent">Pass the extent of the object to be teleported</param>
    /// <param name="scale">Pass the scale of the object to be teleported</param>
    public void ShowHologram(Mesh hologramMesh, Vector3 extent, Vector3 scale, GameObject objectToTeleport)
    {
        if(m_mesh != null)
        {
            // Changes the colour of the hologram based on if the object can be teleported or not
            // Also set m_canTeleport to true or false
            if(CanTeleport(extent, objectToTeleport))
            {
                m_meshRenderer.material.color = m_goodColour;
                m_canTeleport = true;
            }
            else
            {
                m_meshRenderer.material.color = m_badColour;
                m_canTeleport = false;
            }
            // Set the hologram mesh to the object to be teleported
            m_mesh.mesh = hologramMesh;
            // Set the scale of the hologram
            // (1 / transform.localScale.x) reverses any scale chage the beacon has imposed on the test object
            //transform.localScale = transform.localScale.x != 0 ? scale * (1 / transform.localScale.x) : Vector3.zero;
            transform.localScale = scale * 5.0f;
        }
    }

    // Stop displaying the hologram of potential teleport
    public void StopHologram()
    {
        transform.localScale = Vector3.zero;
        transform.rotation = Quaternion.identity;
        m_canTeleport = false;
    }

    // Can the object be teleported?
    public bool CanTeleport()
    {
        return m_canTeleport;
    }
}

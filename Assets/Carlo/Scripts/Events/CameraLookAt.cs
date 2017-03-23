using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

[System.Serializable]
public struct CameraMove
{
    [Tooltip("Use Rotation?")]
    public bool useRotation;
    [Tooltip("Target to rotate camera to")]
    public Transform lookAtPosition;
    [Tooltip("Use Zoom?")]
    public bool useZoom;
    [Tooltip("Target zoom FOV (field of view)")]
    public float zoomFOV;
    [Tooltip("Speed of the move (for both rotate and zoom)")]
    public float moveSpeed;
    [Tooltip("Wait time between before moving to the next move")]
    public float waitTime;
    [Tooltip("Is the move complete?")]
    public bool isMoveOver;
}

public class CameraLookAt : MonoBehaviour
{
    // Camera move variables
    [SerializeField, Tooltip("Tag used by cutscene triggers")]
    private string m_cutSceneTrigger = "CutScene";
    [SerializeField, Tooltip("Empty game object that is used for rotation")]
    private Transform m_targetingDestination;
    [SerializeField, Tooltip("Camera that will be used for the cutscene")]
    private Camera m_cutSceneCamera;

    // Store camera moves that will be done
    private CameraMove[] m_cameraMoves;
    // Current camera move
    private int m_currentMove = 0;

    // Stores values and references to the actual player camera
    private GameObject m_playerCamera;
    private RigidbodyFirstPersonController m_playerController;
    private Quaternion m_playerCamRotation;
    private float m_playerCamFOV;

    void Start ()
    {
        // Grab references
        m_playerCamera = Camera.main.gameObject;
        m_playerController = GetComponent<RigidbodyFirstPersonController>();

        // Shut off flags
        m_cutSceneCamera.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == m_cutSceneTrigger)
        {
            CameraLookAtTrigger trigger = other.GetComponent<CameraLookAtTrigger>();
            if (trigger != null)
            {
                if (!trigger.IsDone())
                {
                    // Set moves
                    SetCameraMoves(trigger.GetCameraMoves());
                    if (m_cameraMoves.Length > 0)
                    {
                        // Start moves
                        StartCameraMoves();
                    }
                }
            }
            else
            {
                Debug.LogWarning("No cutscene set at cutscene trigger: " + other.name + " or cut scene trigger tag not set properly");
            }
        }
    }

    // Copy the camera moves from the trigger
    private void SetCameraMoves(CameraMove[] moveArray)
    {
        m_cameraMoves = moveArray;
        for (int i = 0; i < m_cameraMoves.Length; i++)
        {
            m_cameraMoves[i].isMoveOver = false;
        }
    }

    // Start the camera moves going
    public void StartCameraMoves()
    {
        // Save current settings
        SaveCameraSettings();
        // Turn on cutscene camera
        m_cutSceneCamera.gameObject.SetActive(true);
        // Disable the player
        m_playerCamera.SetActive(false);
        m_playerController.enabled = false;
        // Start camera move loop
        StartCoroutine(PlayAllCameraMoves());
    }

    // Save the player's current camera setting so that we can return to the original camera
    private void SaveCameraSettings()
    {
        // Save settings
        m_playerCamRotation = m_playerCamera.transform.rotation;
        m_playerCamFOV = m_playerCamera.GetComponent<Camera>().fieldOfView;
        // Set starting position of the cutscene camera
        m_cutSceneCamera.transform.rotation = m_playerCamRotation;
        m_cutSceneCamera.fieldOfView = m_playerCamFOV;
    }

    // Camera move loop
    private IEnumerator PlayAllCameraMoves()
    {
        for(int i = 0; i < m_cameraMoves.Length; i++)
        {
            if(i == m_currentMove)
            {
                // Do the current camera move
                if (m_cameraMoves[i].useRotation)
                {
                    // Look at target
                    m_targetingDestination.LookAt(m_cameraMoves[i].lookAtPosition);
                    m_cutSceneCamera.transform.rotation = Quaternion.Lerp(m_cutSceneCamera.transform.rotation, m_targetingDestination.rotation, m_cameraMoves[i].moveSpeed * Time.deltaTime);
                }

                if (m_cameraMoves[i].useZoom)
                {
                    // Zoom in
                    float newFOV = Mathf.Lerp(m_cutSceneCamera.fieldOfView, m_cameraMoves[i].zoomFOV, m_cameraMoves[i].moveSpeed * Time.deltaTime);
                    m_cutSceneCamera.fieldOfView = newFOV;
                }

                // Move is over
                if (m_cameraMoves[i].useRotation)
                {
                    if (Quaternion.Angle(m_cutSceneCamera.transform.rotation, m_targetingDestination.rotation) < 2)
                    {
                        m_cameraMoves[i].isMoveOver = true;
                    }
                }
                else if(m_cameraMoves[i].useZoom)
                {
                    if(Mathf.Abs(m_cutSceneCamera.fieldOfView - m_cameraMoves[i].zoomFOV) < 2)
                    {
                        Debug.Log(m_cutSceneCamera.fieldOfView);
                        m_cameraMoves[i].isMoveOver = true;
                    }
                }
                else
                {
                    m_cameraMoves[i].isMoveOver = true;
                }

                // Check if move is finished or not
                if (m_cameraMoves[i].isMoveOver)
                {
                    m_currentMove++;
                    yield return new WaitForSeconds(m_cameraMoves[i].waitTime);
                }
                else
                {
                    break;
                }
            }
        }

        // Check if all moves are finished
        if(m_currentMove < m_cameraMoves.Length)
        {
            yield return new WaitForEndOfFrame();
            StartCoroutine(PlayAllCameraMoves());
        }
        else
        {
            // Return camera to proper position and FOV
            // Rotate back
            m_cutSceneCamera.transform.rotation = Quaternion.Lerp(m_cutSceneCamera.transform.rotation, m_playerCamRotation, 2 * Time.deltaTime);
            // Zoom out
            float newFOV = Mathf.Lerp(m_cutSceneCamera.fieldOfView, m_playerCamFOV, 2 * Time.deltaTime);
            m_cutSceneCamera.fieldOfView = newFOV;

            //Debug.Log(Quaternion.Angle(m_cutSceneCamera.transform.rotation, m_playerCamRotation));

            // Returned
            if (Quaternion.Angle(m_cutSceneCamera.transform.rotation, m_playerCamRotation) > 2)
            {
                yield return new WaitForEndOfFrame();
                StartCoroutine(PlayAllCameraMoves());
            }
            else
            {
                // Return control to the player
                m_cutSceneCamera.gameObject.SetActive(false);
                m_playerCamera.SetActive(true);
                m_playerController.enabled = true;
                StopAllCoroutines();
            }
        }
    }
}

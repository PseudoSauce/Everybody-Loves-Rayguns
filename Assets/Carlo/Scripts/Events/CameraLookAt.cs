using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CameraMove
{
    public bool rotateTo;
    public Transform lookAtPosition;
    public bool zoomIn;
    public float zoomFOV;
    public float moveSpeed;
    public float waitTime;
    public bool isMoveOver;
}

public class CameraLookAt : MonoBehaviour
{
    // Camera moves
    [SerializeField]
    private string m_cutSceneTrigger = "CutScene";
    [SerializeField]
    private Transform m_targetingDestination;
    [SerializeField]
    private CameraMove[] m_cameraMoves;

    private GameObject m_playerCamera;
    [SerializeField]
    private Camera m_cutSceneCamera;
    private int m_currentMove = 0;
    private Quaternion m_playerCamRotation;
    private float m_playerCamFOV;

    void Start ()
    {
        m_playerCamera = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Camera>().gameObject;
        m_cutSceneCamera.gameObject.SetActive(false);
        for (int i = 0; i < m_cameraMoves.Length; i++)
        {
            m_cameraMoves[i].isMoveOver = false;
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            StartCameraMoves();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == m_cutSceneTrigger)
        {
            if (other.GetComponent<CameraLookAtTrigger>() != null)
            {
                SetCameraMoves(other.GetComponent<CameraLookAtTrigger>().GetCameraMoves());
                Debug.Log(m_cameraMoves);
                if (m_cameraMoves.Length > 0)
                {
                    StartCoroutine(PlayAllCameraMoves());
                }
            }
            else
            {
                Debug.LogWarning("No cutscene set at cutscene trigger: " + other.name + " or cut scene trigger tag not set properly");
            }
        }
    }

    public void SetCameraMoves(CameraMove[] moveArray)
    {
        m_cameraMoves = moveArray;
        for (int i = 0; i < m_cameraMoves.Length; i++)
        {
            m_cameraMoves[i].isMoveOver = false;
        }
    }

    public void StartCameraMoves()
    {
        SaveCameraSettings();
        m_cutSceneCamera.gameObject.SetActive(true);
        m_playerCamera.SetActive(false);
        StartCoroutine(PlayAllCameraMoves());
    }

    private void SaveCameraSettings()
    {
        m_playerCamRotation = m_playerCamera.transform.rotation;
        m_playerCamFOV = m_playerCamera.GetComponent<Camera>().fieldOfView;
        m_cutSceneCamera.transform.rotation = m_playerCamRotation;
        m_cutSceneCamera.fieldOfView = m_playerCamFOV;
    }

    private IEnumerator PlayAllCameraMoves()
    {
        for(int i = 0; i < m_cameraMoves.Length; i++)
        {
            if(i == m_currentMove)
            {
                // Do the current camera move
                if (m_cameraMoves[i].rotateTo)
                {
                    // Look at target
                    m_targetingDestination.LookAt(m_cameraMoves[i].lookAtPosition);
                    m_cutSceneCamera.transform.rotation = Quaternion.Lerp(m_cutSceneCamera.transform.rotation, m_targetingDestination.rotation, m_cameraMoves[i].moveSpeed * Time.deltaTime);
                }

                if (m_cameraMoves[i].zoomIn)
                {
                    // Zoom in
                    float newFOV = Mathf.Lerp(m_cutSceneCamera.fieldOfView, m_cameraMoves[i].zoomFOV, m_cameraMoves[i].moveSpeed * Time.deltaTime);
                    m_cutSceneCamera.fieldOfView = newFOV;
                }

                // Move is over
                if (m_cameraMoves[i].rotateTo)
                {
                    if (Quaternion.Angle(m_cutSceneCamera.transform.rotation, m_targetingDestination.rotation) < 2)
                    {
                        m_cameraMoves[i].isMoveOver = true;
                    }
                }
                else if(m_cameraMoves[i].zoomIn)
                {
                    if(Mathf.Abs(m_cutSceneCamera.fieldOfView - m_cameraMoves[i].zoomFOV) < 1)
                    {
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

            Debug.Log(Quaternion.Angle(m_cutSceneCamera.transform.rotation, m_playerCamRotation));

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
            }
        }
    }
}

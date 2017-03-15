using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MyTypes;
using System;

public class Raygun : MonoBehaviour {
    #region Variables
    // Raygun //
    private GunMode m_currentGunMode = GunMode.Scaler;

    [Tooltip("How far the weapon should shoot")]
    public float weaponShootRange = 25;
    [Tooltip("The end of the gun [Empty gameobject]")]
    public Transform gunEnd;
    private Camera fpsCam;      // There should be a better transform reference to use instead of camera, it makes it so if multiple guns are present, there won't be enough transform to instantiate the beacon
    // Raygun //

    // Teleport // 
    // TODO: BEACON NEEDS TO BE OBJECT POOLED!!!
    public Beacon beaconProjectile = null;
    private Beacon beacon = null;
    private GameObject lastObject = null;
    // Teleport //

    // Sizer //
    private bool stuck = false;//check if you haved hooked to an object
    private bool canFire = true;

    private LineRenderer laserLine;
    private GameObject currentHit;
    private Color currentColor;
    //where the beam ends [it starts from the center of the screen] 
    private Vector3 endPos;
    [SerializeField]
    [Tooltip("Layer to consider for the beam")]
    private LayerMask beamMask;

    [SerializeField]
    [Tooltip("Layer to consider for the fit checkers")]
    private LayerMask fitterMask;
    // Sizer //

    // Debuging //
    public Text displayText = null;
    public Text inst1 = null;
    public Text inst2 = null;
    // Debuging //
    #endregion Variables

    #region Monobehaviour
    void Start ()
    {
        laserLine = GetComponent<LineRenderer>();
        fpsCam = GetComponentInParent<Camera>();
        if (!fpsCam) {
            fpsCam = FindObjectOfType<Camera>();
        }
    }
	
	void Update ()
    {
        ChangeGunMode();

        if (displayText) {
            switch (m_currentGunMode) {
                case GunMode.Teleporter:
                    TeleportBeamInput();
                    displayText.text = "Teleport-Beam";
                    inst1.text = "Teleport object";
                    inst2.text = "Deploy teleport beacon";
                    break;
                case GunMode.Scaler:
                    ScalerBeamInput();
                    StopDisplayingHologram();
                    displayText.text = "Sizer-beam";
                    inst1.text = "Grow object";
                    inst2.text = "Shrink beacon";
                    break;
                default:
                    Debug.LogWarning("Current Gun Mode not set properly.");
                    displayText.text = "Current Gun Mode not set properly.";
                    break;
            }
        } else {
            var disText = new GameObject("DisplayText", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            disText.transform.SetParent(FindObjectOfType<Canvas>().transform);
            disText.GetComponent<RectTransform>().anchoredPosition = Vector2.one;
            displayText = disText.GetComponent<Text>();

            switch (m_currentGunMode) {
                case GunMode.Teleporter:
                    TeleportBeamInput();
                    displayText.text = "Currently in: Teleport Mode";
                    break;
                case GunMode.Scaler:
                    ScalerBeamInput();
                    StopDisplayingHologram();
                    displayText.text = "Currently in: Scaler Mode";
                    break;
                default:
                    Debug.LogWarning("Current Gun Mode not set properly.");
                    displayText.text = "Current Gun Mode not set properly.";
                    break;
            }
        }
    }

    void FixedUpdate()
    {
        switch(m_currentGunMode)
        {
            case GunMode.Teleporter:
                TeleportBeam();
                break;
            case GunMode.Scaler:

                break;
            default:
                Debug.LogWarning("Current Gun Mode not set properly.");
                break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("ForceField"))
        {
            DestroyBeacon();
        }
    }
    #endregion Monobehaviour

    #region Teleport Beam
    private void TeleportBeamInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            // TODO: NEEDS TO BE OBJECT POOLED!!!
            if (beacon != null)
            {
                Destroy(beacon.gameObject);    // Create cool destruction animation call that before destruction
            }

            beacon = Instantiate(beaconProjectile, fpsCam.transform.position, fpsCam.transform.rotation) as Beacon;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            beacon.RotateOnXAxis();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            beacon.RotateOnYAxis();
        }
    }

    private void TeleportBeam()
    {
        RaycastHit hitInfo;
        InteractMessage msg;

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hitInfo, weaponShootRange))
        {
            if (hitInfo.collider.GetComponent<Interactable>())
            {
                if (beacon != null)
                {
                    lastObject = hitInfo.collider.gameObject;
                    msg = new InteractMessage(Interaction.TELEPORTING, "HitBegin", beacon);

                    lastObject.SendMessage("Interact", msg);

                    // Input
                    if (Input.GetMouseButtonDown(0))
                    {
                        msg = new InteractMessage(Interaction.TELEPORTING, "Teleport", beacon);
                        lastObject.SendMessage("Interact", msg);
                    }
                }
            }
            else
            {
                StopDisplayingHologram();
            }
        }
    }

    private void StopDisplayingHologram()
    {
        InteractMessage msg;
        if (beacon != null)
        {
            if (lastObject != null)
            {
                msg = new InteractMessage(Interaction.TELEPORTING, "HitEnd");
                lastObject.SendMessage("Interact", msg);
                lastObject = null;
            }
        }
    }

    private void DestroyBeacon()
    {
        if (beacon != null)
        {
            Destroy(beacon.gameObject);
            beacon = null;
        }
    }
    #endregion Teleport Beam

    #region Scaler Beam
    bool scaling = false;
    private void ScalerBeamInput()
    {
        if (canFire)
        {
            if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2"))
                scaling = true;
            if (Input.GetButtonUp("Fire1") || Input.GetButtonUp("Fire2"))
                scaling = false;

            if (Input.GetButton("Fire1"))
            {
                if(scaling)
                    shootRay("growing");
            }
            if (Input.GetButton("Fire2"))
            {
                if (scaling)
                    shootRay("shrinking");
            }
        }
        if (Input.GetButtonUp("Fire1") || Input.GetButtonUp("Fire2") || !canFire || !scaling)
        {
            laserLine.enabled = false;
            if (stuck || !canFire)
            {
                InteractMessage sendMsg = new InteractMessage(Interaction.SCALING, "STOPGROW");
                currentHit.SendMessage("Interact", sendMsg);
                sendMsg.msg = "STOPSHRINK";
                currentHit.SendMessage("Interact", sendMsg);

                currentHit.GetComponent<Renderer>().material.color = currentColor;
                stuck = false;
                currentHit = null;
                canFire = true;
            }
        }
    }

    private void shootRay(string whatAmIDoing)
    {
        Vector3 camCenter = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
        Vector3 rayOrigin = camCenter;
        RaycastHit normalhit;
        //lines 'resting' position
        laserLine.SetPosition(0, gunEnd.position);
        laserLine.SetPosition(1, camCenter);
        if (!stuck)
        {
            if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out normalhit, weaponShootRange))
            {
                Debug.DrawRay(rayOrigin, fpsCam.transform.forward * weaponShootRange, Color.red);
                if (normalhit.transform.gameObject.GetComponent<ScaleComponent>())
                {
                    stuck = true;
                    currentHit = normalhit.transform.gameObject;
                    //first set the raycast to the pos of object [lock it]
                    currentColor = currentHit.GetComponent<Renderer>().material.color;
                    currentHit.GetComponent<Renderer>().material.color = new Color(0, 0, 255);
                    laserLine.enabled = true;
                }
            }
            else
            {
                print("too far");
            }
        }
        else
        {
            //if you are hooked to an object, draw line to that object
            endPos = currentHit.transform.GetComponent<Renderer>().bounds.center;
            laserLine.SetPosition(1, endPos);
            this.transform.Rotate(new Vector3(0, 0, 60 * Time.deltaTime));
            Debug.DrawLine(rayOrigin, endPos, Color.green);
            if (Physics.Linecast(rayOrigin, endPos, out normalhit, beamMask.value))
            {
                if (normalhit.collider.gameObject != currentHit)
                {
                    print(normalhit.collider.gameObject);
                    print("blocked");
                    canFire = false;
                }
                else
                {
                    //send scale msg to obj
                    InteractMessage sendMsg = new InteractMessage(Interaction.SCALING, "");
                    if (whatAmIDoing == "growing")
                    {
                        sendMsg.msg = "GROW";
                        currentHit.SendMessage("Interact", sendMsg);
                    }
                    else if (whatAmIDoing == "shrinking")
                    {
                        sendMsg.msg = "SHRINK";
                        currentHit.SendMessage("Interact", sendMsg);
                    }
                }
            }
        }
    }
    #endregion Scaler Beam

    #region Raygun
    private void ChangeGunMode()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            m_currentGunMode = GunMode.Teleporter;
            scaling = false;
        }

        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            m_currentGunMode = GunMode.Scaler;
        }

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            m_currentGunMode++;

            if (m_currentGunMode >= GunMode.ModeCount)
            {
                m_currentGunMode = 0;
            }

            if (m_currentGunMode == GunMode.Teleporter)
            {
                if (canFire && currentHit != null)
                    scaling = false;
            }
        }
    }

    public void ChangeGunMode(GunMode mode)
    {
        m_currentGunMode = mode;
    }
    #endregion Raygun

}

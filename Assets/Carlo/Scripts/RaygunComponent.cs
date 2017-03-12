using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MyTypes;
using System;

public enum GunMode
{
    Teleporter = 0,
    Scaler,
    ModeCount
}

public class RaygunComponent : MonoBehaviour {
    #region Variables
    // Raygun //
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
    [Tooltip("Layer to ignore for the beam")]
    private LayerMask beamMask;

    [SerializeField]
    [Tooltip("Layer to ignore for the fit checkers")]
    private LayerMask fitterMask;
    // Sizer //
    #endregion Variables

    #region Monobehaviour
    void Start()
    {
        laserLine = GetComponent<LineRenderer>();
        fpsCam = GetComponentInParent<Camera>();
        if (!fpsCam)
        {
            fpsCam = FindObjectOfType<Camera>();
        }
    }
    #endregion Monobehaviour

    // Gun Modes:
    #region Teleport Beam
    // Primary fire
    public void TeleportBeam()
    {
        if (beacon != null)
        {
            RaycastHit hitInfo;
            InteractMessage msg;

            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hitInfo, weaponShootRange))
            {
                if (hitInfo.collider.GetComponent<Interactable>())
                {
                    lastObject = hitInfo.collider.gameObject;

                    msg = new InteractMessage(Interaction.TELEPORTING, "Teleport", beacon);
                    lastObject.SendMessage("Interact", msg);
                }
            }
        }
        else
        {
            Debug.Log("Beacon needs to be fired first before teleporting an object.");
        }
    }

    // Secondary fire
    public void DeployBeacon()
    {
        if (beacon != null)
        {
            Destroy(beacon.gameObject);    // Create cool destruction animation call that before destruction
        }

        beacon = Instantiate(beaconProjectile, fpsCam.transform.position, fpsCam.transform.rotation) as Beacon;
    }

    public void RotateHologram(bool onX)
    {
        if (beacon != null)
        {
            if (onX)
            {
                beacon.RotateOnXAxis();
            }
            else
            {
                beacon.RotateOnYAxis();
            }
        }
    }

    public void DisplayHologram()
    {
        if (beacon != null)
        {
            RaycastHit hitInfo;
            InteractMessage msg;

            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hitInfo, weaponShootRange))
            {
                if (hitInfo.collider.GetComponent<Interactable>())
                {
                    lastObject = hitInfo.collider.gameObject;
                    msg = new InteractMessage(Interaction.TELEPORTING, "HitBegin", beacon);

                    lastObject.SendMessage("Interact", msg);
                }
                else
                {
                    StopDisplayingHologram();
                }
            }
        }
    }

    private void StopDisplayingHologram()
    {
        if (beacon != null)
        {
            if (lastObject != null)
            {
                InteractMessage msg;
                msg = new InteractMessage(Interaction.TELEPORTING, "HitEnd");
                lastObject.SendMessage("Interact", msg);
                lastObject = null;
            }
        }
    }

    // Still unused feature (for later)
    public void DestroyBeacon()
    {
        if (beacon != null)
        {
            Destroy(beacon.gameObject);
            beacon = null;
        }
    }
    #endregion Teleport Beam

    #region Scaler Beam
    public void ShootRay(string rayType)
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
            if (Physics.Linecast(rayOrigin, endPos, out normalhit, beamMask))
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
                    if (rayType == "growing")
                    {
                        sendMsg.msg = "GROW";
                        currentHit.SendMessage("Interact", sendMsg);
                    }
                    else if (rayType == "shrinking")
                    {
                        sendMsg.msg = "SHRINK";
                        currentHit.SendMessage("Interact", sendMsg);
                    }
                }
            }
        }
    }

    public void StopScaling()
    {
        if (!canFire)
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
    #endregion Scaler Beam
}

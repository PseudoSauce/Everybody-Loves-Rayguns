using UnityEngine;
using System.Collections;

using MyTypes;
public class ShootCast : MonoBehaviour {
    [Tooltip("How far the weapon should shoot")]
    public float weaponShootRange = 25;
    [Tooltip("The end of the gun [Empty gameobject]")]
    public Transform gunEnd;

    private bool stuck = false;//check if you haved hooked to an object
    private bool canFire = true;

    private Camera fpsCam;
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

    void Start() {
        laserLine = GetComponent<LineRenderer>();
        fpsCam = GetComponentInParent<Camera>();
    }

    void Update() {
        if (canFire) {
            if (Input.GetButton("Fire1")) {
                shootRay("growing");
            }
            if (Input.GetButton("Fire2")) {
                shootRay("shrinking");
            }
        }
        if (Input.GetButtonUp("Fire1") || Input.GetButtonUp("Fire2") || !canFire) {
            laserLine.enabled = false;
            if (stuck || !canFire) {
                InteractMessage sendMsg;
                sendMsg.interaction = Interaction.SCALING;
                sendMsg.msg = "STOPGROW";
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

    void shootRay(string whatAmIDoing) {
        Vector3 camCenter = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
        Vector3 rayOrigin = camCenter;
        RaycastHit normalhit;
        //lines 'resting' position
        laserLine.SetPosition(0, gunEnd.position);
        laserLine.SetPosition(1, camCenter);
        if (!stuck) {
            if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out normalhit, weaponShootRange)) {
                Debug.DrawRay(rayOrigin, fpsCam.transform.forward * weaponShootRange, Color.red);
                if (normalhit.transform.gameObject.CompareTag("Sizeable")) {
                    stuck = true;
                    currentHit = normalhit.transform.gameObject;
                    //first set the raycast to the pos of object [lock it]
                    currentColor = currentHit.GetComponent<Renderer>().material.color;
                    currentHit.GetComponent<Renderer>().material.color = new Color(0, 0, 255);
                    laserLine.enabled = true;
                }
            } else {
                print("too far");
            }
        } else {
            //if you are hooked to an object, draw line to that object
            endPos = currentHit.transform.GetComponent<Renderer>().bounds.center;
            laserLine.SetPosition(1, endPos);
            this.transform.Rotate(new Vector3(0, 0, 60 * Time.deltaTime));
            Debug.DrawLine(rayOrigin, endPos, Color.green);
            if (Physics.Linecast(rayOrigin, endPos, out normalhit, beamMask)) {
                if (normalhit.collider.gameObject != currentHit) {
                    print(normalhit.collider.gameObject);
                    print("blocked");
                    canFire = false;
                } else {
                    //send scale msg to obj
                    InteractMessage sendMsg;
                    sendMsg.interaction = Interaction.SCALING;
                    if (whatAmIDoing == "growing") {
                        sendMsg.msg = "GROW";
                        currentHit.SendMessage("Interact", sendMsg);
                    } else if (whatAmIDoing == "shrinking") {
                        sendMsg.msg = "SHRINK";
                        currentHit.SendMessage("Interact", sendMsg);
                    }
                }
            }
        }
    }
}
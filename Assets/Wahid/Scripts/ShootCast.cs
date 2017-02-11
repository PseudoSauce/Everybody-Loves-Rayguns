using UnityEngine;
using System.Collections;

public class ShootCast : MonoBehaviour {
    public float weaponShootRange;
    public Transform gunEnd;

    private bool stuck = false;//check if you haved hooked to an object
    private bool canFire = true;
    private Camera fpsCam;
    private LineRenderer laserLine;
    private GameObject currentHit;
    private Color currentColor;
    private Vector3 endPos;
    [SerializeField]
    private LayerMask mask1;

    public delegate void Scaler(GameObject param);

    void Start() {
        laserLine = GetComponent<LineRenderer>();
        fpsCam = GetComponentInParent<Camera>();
    }

    void Update() {
        if (canFire) {
            if (Input.GetButton("Fire1")) {
                shootRay(growObject);
            }
            if (Input.GetButton("Fire2")) {
                shootRay(shrinkObject);
            }
        }
        if (Input.GetButtonUp("Fire1") || Input.GetButtonUp("Fire2") || !canFire) {
            laserLine.enabled = false;
            if (stuck || !canFire) {
                currentHit.GetComponent<Renderer>().material.color = currentColor;
                stuck = false;
                currentHit = null;
                canFire = true;
            }
        }
    }

    void shootRay(Scaler scalerDel) {
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
            Debug.DrawLine(rayOrigin, currentHit.transform.position, Color.green);
            if (Physics.Linecast(rayOrigin, endPos, out normalhit, mask1)) {
                if (normalhit.collider.gameObject != currentHit) {
                    print(normalhit.collider.gameObject);
                    print("blocked");
                    canFire = false;
                } else {
                    scalerDel(currentHit);
                }
            }
        }
    }

    void growObject(GameObject otherObject) {
        otherObject.transform.localScale += new Vector3(0.01f, 0.01f, 0.01f);
    }

    void shrinkObject(GameObject otherObject) {
        otherObject.transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
    }
}
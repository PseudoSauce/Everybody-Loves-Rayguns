using UnityEngine;
using System.Collections;

public class ShootCast : MonoBehaviour {
    [Tooltip("How far the weapon should shoot")]
    public float weaponShootRange = 25;
    [Tooltip("The end of the gun [Empty gameobject]")]
    public Transform gunEnd;

    private bool stuck = false;//check if you haved hooked to an object
    private bool canFire = true;

    //volume check for object
    private float extentPercentage = 0.05f;
    private Vector3[] directions;
    private float[] extents;
    private float extentX, extentY, extentZ;


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

    public delegate void Scaler(GameObject param);

    void Start() {
        laserLine = GetComponent<LineRenderer>();
        fpsCam = GetComponentInParent<Camera>();
        //for the raycast detector
        //directions = new Vector3[5];
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
            Debug.DrawLine(rayOrigin, endPos, Color.green);
            if (Physics.Linecast(rayOrigin, endPos, out normalhit, beamMask)) {
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

    private bool canFit(Vector3 extent, GameObject otherObject) {
        // Set the extent of the check
        extentX = extent.y + extent.y * extentPercentage;
        extentY = extent.x + extent.x * extentPercentage;
        extentZ = extent.z + extent.z * extentPercentage;

        extents = new float[]{ extentX, extentY, extentZ,
                               extentX, extentY, extentZ };
        //dont need -up since it is the floor
        directions = new Vector3[] { otherObject.transform.up, otherObject.transform.right, otherObject.transform.forward,
            -otherObject.transform.up, -otherObject.transform.right, -otherObject.transform.forward };
        //initial you can fit
        bool canIFit = true;
        //each bool for the raycasts stored here
        bool[] results = new bool[directions.Length];
        // Cast rays from the centre of the test object
        for (int i = 0; i < directions.Length; i++) {
            Ray ray = new Ray(otherObject.transform.position, directions[i]);
            Debug.DrawRay(otherObject.transform.position, directions[i] * extents[i], Color.red);
            // Regular rayast check
            RaycastHit hit;
            //ignore layer 10: Floor
            if (Physics.Raycast(ray, out hit, extents[i], beamMask)) {
                print(hit.collider.gameObject.name);
                //if you hit, we assume you cannot fit conditionally
                canIFit = false;
                results[i] = true;
            } else {
                print("hitting nothing");
                results[i] = false;
            }
        }
        if (!canIFit) {
            //if you cannot fit, check to see if all of the values stored are of the same time, either all true or all false
            canIFit = areTheseBooleansThisState(results, true, 2);//check all raycasts from before
            //negated to reflect meaning of 'fitting'
        }
        return canIFit;
    }

    bool areTheseBooleansThisState(bool[] array, bool state, int howMany) {
        bool answer = true;
        int checkedNum = 0;
        for (int i = 0; i < array.Length; i++) {
            if (array[i] == state) {
                checkedNum++;
            } else if (checkedNum == howMany) {
                answer = false;
                break;
            }
        }
        return answer;
    }

    void growObject(GameObject otherObject) {
        Rigidbody oRb = otherObject.GetComponent<Rigidbody>();
        if (oRb.mass < 100 && canFit(otherObject.GetComponent<MeshRenderer>().bounds.extents, otherObject)) {
            otherObject.transform.localScale += new Vector3(0.01f, 0.01f, 0.01f);
            otherObject.GetComponent<Rigidbody>().mass += 0.1f;
        } else {
            print("cant grow this object");
        }
    }

    void shrinkObject(GameObject otherObject) {
        otherObject.transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
        Rigidbody oRb = otherObject.GetComponent<Rigidbody>();
        if (oRb.mass > 1) {
            oRb.mass -= 0.1f;
        }
    }
}
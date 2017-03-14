using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MyTypes;

[AddComponentMenu("Custom Components/ScaleComponent")]
[RequireComponent(typeof(Rigidbody))]
public class ScaleComponent : Interactable {
    private float origScaleFactor;
    private float scaleFactor = 0.05f;
    Vector3 localScaleOrig;
    //volume check for object
    private float extentPercentage = 0.05f;
    private Vector3[] directions;
    private float[] extents;
    private float extentX, extentY, extentZ;
    [SerializeField]
    [Tooltip("Layer to ignore for the beam")]
    private LayerMask beamMask;
    //init
    bool growing;
    bool shrinking;


    public delegate void Scaler(GameObject param);

    protected override void Init() {
        AssignInteractionType(Interaction.SCALING);
        AssignStart(MyStart);
        AssignUpdate(MyUpdate);
    }

    private void MyStart() {
        origScaleFactor = scaleFactor;
        localScaleOrig = transform.localScale;
        Debug.Log("GrowComponent: Starting...");
    }

    private void MyUpdate(float deltaTime) {
        if (growing) {
            Debug.Log("growing...");
            growObject(3);
        } else if (shrinking) {
            Debug.Log("shrinking...");
            shrinkObject();
        } else {
            scaleFactor = origScaleFactor;
        }
    }

    void shrinkObject() {
        Rigidbody oRb = GetComponent<Rigidbody>();
        //extra check for different shapes
        Vector3 desiredScale = new Vector3((localScaleOrig.x % localScaleOrig.x) + 1f
                , (localScaleOrig.y % localScaleOrig.y) + 1f);
        if (transform.localScale != desiredScale) {
            scaleFactor += 0.1f;
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3((localScaleOrig.x % localScaleOrig.x) + 1f
                , (localScaleOrig.y % localScaleOrig.y) + 1f,
                (localScaleOrig.z % localScaleOrig.z) + 1f), scaleFactor * Time.deltaTime);
            if (oRb.mass > 2f) {
                oRb.mass -= scaleFactor;
            }
        }
    }


    void growObject(float scaleTo) {

        // Vector3 scale = Vector3.Lerp(transform.localScale, 20, Mathf.SmoothStep(0.0, 1.0, Time.deltaTime));
        //float scale = Mathf.sin(Time.time * (.5f * 2 * Mathf.PI) + 1f) / 2f;
        Rigidbody oRb = GetComponent<Rigidbody>();
        float lgScale = smallestVecIndice(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        if (canFit(GetComponent<MeshRenderer>().bounds.extents)) {
            scaleFactor += 0.1f;
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(localScaleOrig.x + scaleTo, //
                localScaleOrig.y + scaleTo, localScaleOrig.z + scaleTo), scaleFactor * Time.deltaTime);
            if (oRb.mass < 100f) {//arbirary clamp
                oRb.mass += scaleFactor;
            }
        } else {
            print("cant grow this object");
        }
    }

    float smallestVecIndice(float x, float y, float z) {
        return (x < y) ? ((x < z) ? x : z) : ((y < z) ? y : z);
    }
    float largestVecIndice(float x, float y, float z) {
        return (x > y) ? ((x > z) ? x : z) : ((y > z) ? y : z);
    }

    private bool canFit(Vector3 extent) {
        // Set the extent of the check
        extentX = extent.y + extent.y * extentPercentage;
        extentY = extent.x + extent.x * extentPercentage;
        extentZ = extent.z + extent.z * extentPercentage;

        extents = new float[]{ extentX, extentY, extentZ,
                               extentX, extentY, extentZ };

        directions = new Vector3[] { transform.up, transform.right, transform.forward,
            -transform.up, -transform.right, -transform.forward };
        //initially you can fit
        bool canIFit = true;
        //each bool for the raycasts stored here
        bool[] results = new bool[directions.Length];
        // Cast rays from the centre of the test object
        for (int i = 0; i < directions.Length; i++) {
            Ray ray = new Ray(transform.position, directions[i]);
            Debug.DrawRay(transform.position, directions[i] * extents[i], Color.red);
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

    // place your custom logic here for interaction
    protected override void Commit(InteractMessage msg) {
        Debug.Log(this + ": " + msg);

        switch (msg.msg) {
            case "GROW":
                growing = true;
                break;
            case "STOPGROW":
                growing = false;
                break;
            case "SHRINK":
                shrinking = true;
                break;
            case "STOPSHRINK":
                shrinking = false;
                break;
            default:
                break;
        }
    }
}

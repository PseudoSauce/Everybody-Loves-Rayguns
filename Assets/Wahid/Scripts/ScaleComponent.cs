using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MyTypes;

public class ScaleComponent : Interactable {
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
        Debug.Log("GrowComponent: Starting...");
    }

    private void MyUpdate(float deltaTime) {
        if (growing) {
            Debug.Log("growing...");
            growObject();
        } else if (shrinking) {
            Debug.Log("shrinking...");
            shrinkObject();
        }
    }

    void shrinkObject() {
        Rigidbody oRb = GetComponent<Rigidbody>();
        if (oRb.mass > 1) {
            transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
            oRb.mass -= 0.1f;
        }
    }

    void growObject() {
        Rigidbody oRb = GetComponent<Rigidbody>();
        if (oRb.mass < 100 && canFit(GetComponent<MeshRenderer>().bounds.extents)) {
            transform.localScale += new Vector3(0.01f, 0.01f, 0.01f);
            GetComponent<Rigidbody>().mass += 0.1f;
        } else {
            print("cant grow this object");
        }
    }

    private bool canFit(Vector3 extent) {
        // Set the extent of the check
        extentX = extent.y + extent.y * extentPercentage;
        extentY = extent.x + extent.x * extentPercentage;
        extentZ = extent.z + extent.z * extentPercentage;

        extents = new float[]{ extentX, extentY, extentZ,
                               extentX, extentY, extentZ };
        //dont need -up since it is the floor
        directions = new Vector3[] { transform.up, transform.right, transform.forward,
            -transform.up, -transform.right, -transform.forward };
        //initial you can fit
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
    protected override void Commit(string msg) {
        Debug.Log(this + ": " + msg);

        switch (msg) {
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

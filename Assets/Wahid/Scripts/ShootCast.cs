using UnityEngine;
using System.Collections;

public class ShootCast : MonoBehaviour {
    public int sizeRatio = 1;
    public float weaponRange;
    public Transform gunEnd;

    private Camera fpsCam;
    private WaitForSeconds coolDown = new WaitForSeconds(0.5f);
    private LineRenderer laserLine;
    private float nextFire;
    private Shader highLight;
    private GameObject currentHit;
    private Color currentColor;
    private bool stuck = false;//see if raycast should be set to dir of obj
    private bool canFire = true;
    private const int mask = 1 << 9;//this is the layer mask to ignore (ignores player)

    void Start() {
        laserLine = GetComponent<LineRenderer>();
        fpsCam = GetComponentInParent<Camera>();
        highLight = Shader.Find("Self-Illumin/Outlined Diffuse");
    }

    void Update() {
        if (canFire) {
            if (Input.GetButton("Fire1")) {
                Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
                RaycastHit normalhit;
                laserLine.SetPosition(0, gunEnd.position);

                if (!stuck) {
                    if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out normalhit, weaponRange)) {
                        Debug.DrawRay(rayOrigin, fpsCam.transform.forward * weaponRange, Color.red);
                        if (normalhit.transform.gameObject.CompareTag("Sizeable")) {
                            stuck = true;
                            currentHit = normalhit.transform.gameObject;
                            //first set the raycast to the pos of object [lock it]
                            currentColor = currentHit.GetComponent<Renderer>().material.color;
                            currentHit.GetComponent<Renderer>().material.color = new Color(100, 140, 0);
                            laserLine.enabled = true;
                        }
                    }
                } else {
                    //if you have stuck, draw line to that object
                    laserLine.SetPosition(1, currentHit.transform.position);
                    if (Physics.Linecast(rayOrigin, currentHit.transform.position, out normalhit, mask)) {
                        print(normalhit.collider.gameObject);
                        Debug.DrawLine(rayOrigin, currentHit.transform.position, Color.green);
                        //if (normalhit.transform.gameObject.CompareTag(currentHit.gameObject.tag)) {
                        print("blocked");
                        canFire = false;
                        //if there is something blocking between those points, exit
                        //}
                        //    float dist = Vector3.Distance(stuckHit.transform.position, currentHit.transform.position);
                        //if (Mathf.Abs(dist) > objectPadding) {
                        //    currentHit.GetComponent<Renderer>().material.color = new Color(255, 0, 0);
                        //} else {
                        //    currentHit.GetComponent<Renderer>().material.color = new Color(100, 140, 0);
                        //}
                    }
                }
            }
        } else if (Input.GetButtonUp("Fire1") || !canFire) {
            laserLine.enabled = false;
            if (stuck) {
                currentHit.GetComponent<Renderer>().material.color = currentColor;
                stuck = false;
                currentHit = null;
            }
            if (!canFire) {
                StartCoroutine(coolEffect());
            }
        }
    }
    private IEnumerator coolEffect() {
        yield return coolDown;
        currentHit = null;
        stuck = false;
        canFire = true;
    }
}
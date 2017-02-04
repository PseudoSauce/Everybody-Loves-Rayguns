using UnityEngine;
using System.Collections;

public class ShootCast : MonoBehaviour {
    public int gunDamage = 1;
    public float fireRate = 0.25f;
    public float weaponRange;
    public Transform gunEnd;
    public float objectPadding = 2; //this is for the shooting zone

    private Camera fpsCam;
    private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);
    private LineRenderer laserLine;
    private float nextFire;
    private Shader highLight;
    private GameObject currentHit;
    private Color currentColor;
    private bool firstHit = true;

    void Start() {
        laserLine = GetComponent<LineRenderer>();
        fpsCam = GetComponentInParent<Camera>();
        highLight = Shader.Find("Self-Illumin/Outlined Diffuse");
    }

    void Update() {
        if (Input.GetButton("Fire1") && Time.time > nextFire) {
            nextFire = Time.time + fireRate;
            Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
            RaycastHit hit;
            laserLine.SetPosition(0, gunEnd.position);

            if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, weaponRange)) {
                //print("hit point is : " + hit.point);
                if (hit.transform.gameObject.CompareTag("Sizeable")) {
                    //first set the raycast to the pos of object [lock it]
                    laserLine.SetPosition(1, hit.point);
                    if (firstHit) {
                        currentHit = hit.transform.gameObject;
                        currentColor = currentHit.GetComponent<Renderer>().material.color;

                        currentHit.GetComponent<Renderer>().material.color = new Color(100, 140, 0);
                        laserLine.enabled = true;
                        firstHit = false;
                        weaponRange = Vector3.Distance(fpsCam.transform.position, currentHit.transform.position);
                    }
                    //print("object pos is : " + Mathf.Round(currentHit.transform.position.x));
                    //calculating the distance between the hit and the object, then limiting your hit distance
                    print(hit.transform.position);
                    print("curr" + currentHit.transform.position);
                    float dist = Vector3.Distance(hit.transform.position, currentHit.transform.position);
                    print("distance x between hit and obj: " + dist);
                    if (Mathf.Abs(dist) > objectPadding) {
                        currentHit.GetComponent<Renderer>().material.color = new Color(255, 0, 0);
                    } else {
                        currentHit.GetComponent<Renderer>().material.color = new Color(100, 140, 0);
                    }
                }
            } else {
                laserLine.SetPosition(1, rayOrigin + (fpsCam.transform.forward * weaponRange));
            }
        } else if (Input.GetButtonUp("Fire1")) {
            if (!firstHit) {
                laserLine.enabled = false;
                currentHit.GetComponent<Renderer>().material.color = currentColor;
                currentHit = null;
                firstHit = true;
            }
        }
    }
}
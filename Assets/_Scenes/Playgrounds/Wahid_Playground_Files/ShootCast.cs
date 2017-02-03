using UnityEngine;
using System.Collections;

public class ShootCast : MonoBehaviour {
    public int gunDamage = 1;
    public float fireRate = 0.25f;
    public float weaponRange = 50f;
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
                laserLine.SetPosition(1, hit.point);
                print("hit point is : " + hit.point);


                if (firstHit) {
                    currentHit = hit.collider.gameObject;
                    currentColor = currentHit.GetComponent<Renderer>().material.color;

                    currentHit.GetComponent<Renderer>().material.color = new Color(100, 140, 0);
                    laserLine.enabled = true;
                    firstHit = false;
                }
                print("object pos is : " + Mathf.Round(currentHit.transform.position.x));
                float currX = currentHit.transform.position.x;
                //hit.point.x > currX + objectPadding && 
                if (hit.point.x > currX - objectPadding && hit.point.x > currX + objectPadding) {
                    currentHit.GetComponent<Renderer>().material.color = new Color(255, 0, 0);
                } else {
                    currentHit.GetComponent<Renderer>().material.color = new Color(100, 140, 0);
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
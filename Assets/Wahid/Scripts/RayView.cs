using UnityEngine;
using System.Collections;

public class RayView : MonoBehaviour {
    private float weaponRange;                     
    private Camera fpsCam;                               

    void Start() {
        weaponRange = GetComponent<ShootCast>().weaponRange;
        print("weapon range" + weaponRange);
        fpsCam = GetComponentInParent<Camera>();
    }

    void Update() {
        Vector3 lineOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
        Debug.DrawRay(lineOrigin, fpsCam.transform.forward * weaponRange, Color.red);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTypes;
//[ExecuteInEditMode]
public class TurretScript : MonoBehaviour {
    //NOTE: increase the FOV and camera frustums for increased effect
    [Tooltip("Layer to ignore for the beam")]
    [SerializeField]
    private LayerMask beamMask;
    [Range(0, 50)]
    public float turretRotSpeed = 2.0f;
    [Range(0, 360)]
    public float maxRotation = 45.0f;
    public Transform player;
    public float turretShootRange = 11.0f;

    private Transform gunEnd;
    private Collider playerColl;
    private Camera cam;
    private Plane[] planes;
    private LineRenderer laserLine;

    private bool stuck = false;//check if you haved hooked to an object

    private Plane[] startingPlanes;

    void Start() {
        gunEnd = GetComponentInChildren<Transform>();
        laserLine = GetComponent<LineRenderer>();
        playerColl = player.GetComponent<Collider>();
        cam = GetComponent<Camera>();
        cam.farClipPlane = turretShootRange;
        planes = GeometryUtility.CalculateFrustumPlanes(cam);
        startingPlanes = planes;
    }

    void Update() {
        Scan();
    }

    void Scan() {
        //recalc planes
        planes = GeometryUtility.CalculateFrustumPlanes(cam);
        if (GeometryUtility.TestPlanesAABB(planes, playerColl.bounds)) {
            Detected();
        } else {
            FireBeam(false);
            transform.rotation = Quaternion.Euler(0f, maxRotation * Mathf.Sin(Time.time * turretRotSpeed), 0f);
            GetComponentInParent<Renderer>().material.color = Color.green;
        }
    }

    void Detected() {
        if (GeometryUtility.TestPlanesAABB(startingPlanes, playerColl.bounds)) {
            print("player detected");
            GetComponentInParent<Renderer>().material.color = Color.red;
            transform.LookAt(player);
            FireBeam(true);
        } else {
            FireBeam(false);
        }
    }

    void FireBeam(bool inRange) {
        if (inRange) {
            RaycastHit normalhit;
            Vector3 rayOrigin = gunEnd.position;
            if (Physics.Linecast(rayOrigin, player.position, out normalhit, beamMask.value)) {
                if (normalhit.collider.gameObject != player.gameObject) {
                    print(normalhit.collider.gameObject);
                    print("blocked");
                    FireBeam(false);
                } else {
                    laserLine.SetPosition(0, gunEnd.position);
                    laserLine.SetPosition(1, player.position);
                    laserLine.enabled = true;

                    InteractMessage msg;
                    msg = new InteractMessage(Interaction.DEATH, "SENDHITS");
                    player.SendMessage("Interact", msg);
                }
            }
        } else {
            laserLine.SetPosition(0, gunEnd.position);
            laserLine.SetPosition(1, gunEnd.position);
            laserLine.enabled = false;
            InteractMessage msg;
            msg = new InteractMessage(Interaction.DEATH, "STOPHITS");
            player.SendMessage("Interact", msg);
        }
    }

    //TODO: try something later....
    T CopyComponent<T>(T original, GameObject destination) where T : Component {
        System.Type type = original.GetType();
        var dst = destination.GetComponent(type) as T;
        if (!dst) dst = destination.AddComponent(type) as T;
        var fields = type.GetFields();
        foreach (var field in fields) {
            if (field.IsStatic) continue;
            field.SetValue(dst, field.GetValue(original));
        }
        var props = type.GetProperties();
        foreach (var prop in props) {
            if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name") continue;
            prop.SetValue(dst, prop.GetValue(original, null), null);
        }
        return dst as T;
    }
}
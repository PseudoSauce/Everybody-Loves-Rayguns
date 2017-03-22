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

    private Renderer turretBodyColor;
    private Transform gunEnd;
    private Collider playerColl;
    private Camera cam;
    private Plane[] planes;
    private LineRenderer laserLine;
    private Transform playerHeadPosition;
    private Vector3 playerHeadCorrected;

    private bool locked = false;//when you are locked to player
    private Quaternion lockRotation;//the rotation of the turret at point of lock

    private bool stuck = false;//check if you haved hooked to an object

    private Plane[] startingPlanes;

    void Start() {
        playerHeadPosition = player.GetComponentInChildren<Camera>().transform;
        turretBodyColor = GetComponentInParent<Renderer>();
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
            locked = false;
            FireBeam(false);
            transform.rotation = Quaternion.Euler(0f, maxRotation * Mathf.Sin(Time.time * turretRotSpeed), 0f);
            turretBodyColor.material.color = Color.green;
        }
    }

    void Detected() {
        if (!locked) {
            //first store init pos from whence you entered 
            lockRotation = transform.rotation;
            locked = true;
        }
        Quaternion lowLim = lockRotation * Quaternion.Euler(0, lockRotation.y - 90, 0);
        Quaternion highLim = lockRotation * Quaternion.Euler(0, lockRotation.y + 90, 0);
        if (!(transform.rotation.y > lowLim.y && transform.rotation.y < highLim.y)) {
            FireBeam(false);
        } else {
            turretBodyColor.material.color = Color.red;
            transform.LookAt(player);
            FireBeam(true);
        }
    }

    void FireBeam(bool inRange) {
        if (inRange) {
            RaycastHit normalhit;
            Vector3 rayOrigin = gunEnd.position;
            if (Physics.Linecast(rayOrigin, playerHeadPosition.position, out normalhit, beamMask.value)) {
                if (normalhit.collider.gameObject != player.gameObject) {
                    print(normalhit.collider.gameObject);
                    turretBodyColor.material.color = Color.yellow;
                    print("blocked");
                    FireBeam(false);
                } else {
                    laserLine.SetPosition(0, gunEnd.position);
                    playerHeadCorrected = playerHeadPosition.position - new Vector3(0, 0.2f, 0);
                    laserLine.SetPosition(1, playerHeadCorrected);
                    laserLine.enabled = true;

                    InteractMessage msg;
                    msg = new InteractMessage(Interaction.DEATH, "SENDHITS");
                    player.SendMessage("Interact", msg);
                }
            }
        } else {
            transform.rotation = Quaternion.Euler(0f, maxRotation * Mathf.Sin(Time.time * turretRotSpeed), 0f);
            laserLine.SetPosition(0, gunEnd.position);
            laserLine.SetPosition(1, gunEnd.position);
            laserLine.enabled = false;
            //InteractMessage msg;
            //msg = new InteractMessage(Interaction.DEATH, "STOPHITS");
            //player.SendMessage("Interact", msg);
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
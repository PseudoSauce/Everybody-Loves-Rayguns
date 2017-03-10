using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTypes;
//[ExecuteInEditMode]
public class TurretScript : MonoBehaviour {
    [Range(0, 50)]
    public float turretRotSpeed = 2.0f;
    [Range(0, 360)]
    public float maxRotation = 45.0f;
    public Transform player;
    public Transform gunEnd;
    public float turretShootRange = 11.0f;

    private Collider playerColl;
    private Camera cam;
    private Plane[] planes;
    private LineRenderer laserLine;

    private bool stuck = false;//check if you haved hooked to an object

    private Plane[] startingPlanes;

    void Start() {
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
            transform.LookAt(player);
            FireBeam(true);
        }
        print("player detected");
        GetComponentInParent<Renderer>().material.color = Color.red;
    }

    void FireBeam(bool inRange) {
        if (inRange) {
            laserLine.SetPosition(0, gunEnd.position);
            laserLine.SetPosition(1, player.position);
            laserLine.enabled = true;

            InteractMessage msg;
            msg = new InteractMessage(Interaction.DEATH, "SENDHITS");
            player.SendMessage("Interact", msg);
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
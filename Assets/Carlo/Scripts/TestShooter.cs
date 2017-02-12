using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShooter : MonoBehaviour {

    public GameObject projectile;
    public LineRenderer ray;
    public Transform gun;
    private GameObject lastProjectile = null;
    private Coroutine lastLaser = null;
    private bool isShooting = false;
    private Vector3 firstPoint, secondPoint;

    void Start()
    {
        ray.SetPosition(0, transform.position);
        ray.SetPosition(1, transform.position);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (lastProjectile != null)
            {
                Destroy(lastProjectile);    // Create cool destruction animation call that before destruction
            }

            lastProjectile = Instantiate(projectile, Camera.main.transform.position, Camera.main.transform.rotation) as GameObject;
        }

        if(isShooting)
        {
            firstPoint = Vector3.Lerp(firstPoint, secondPoint, Time.deltaTime);

            ray.SetPosition(0, firstPoint);
            ray.SetPosition(1, firstPoint + ((secondPoint - firstPoint).normalized * 25.0f));
        }
        else
        {
            ray.SetPosition(0, transform.position);
            ray.SetPosition(1, transform.position);
        }
    }

    void FixedUpdate()
    {
        // How to teleport an object
        RaycastHit hitInfo;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 300.0f))
        {
            if (lastProjectile != null)
            {
                if (hitInfo.collider.CompareTag("Teleportable"))
                {
                    lastProjectile.GetComponent<Beacon>().SendMesh(hitInfo.collider.GetComponent<MeshFilter>().mesh,
                                                                    hitInfo.collider.GetComponent<MeshRenderer>().bounds.extents,
                                                                    hitInfo.transform.localScale);
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (lastProjectile.GetComponent<Beacon>().CanTeleport(hitInfo.collider.GetComponent<MeshRenderer>().bounds.extents))
                        {
                            hitInfo.collider.gameObject.transform.position = lastProjectile.GetComponent<Beacon>().GetTeleportPosition(hitInfo.collider.GetComponent<MeshRenderer>().bounds.extents);
                            hitInfo.collider.gameObject.transform.rotation = Quaternion.identity;
                            //hitInfo.collider.gameObject.transform.Rotate(Vector3.up, lastProjectile.GetComponent<Beacon>().GetRotationAngle());
                            hitInfo.collider.GetComponent<Rigidbody>().velocity = Vector3.zero;
                        }
                    }
                }
                else
                {
                    lastProjectile.GetComponent<Beacon>().StopHologram();
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (lastLaser != null)
                {
                    StopCoroutine(lastLaser);
                }
                lastLaser = StartCoroutine(FireLaser(gun.position + (hitInfo.point - gun.position).normalized * 50.0f));
            }
        }
        else
        {
            if (lastProjectile != null)
            {
                lastProjectile.GetComponent<Beacon>().StopHologram();
            }
        }
    }


    private IEnumerator FireLaser(Vector3 _secondPoint)
    {
        firstPoint = gun.position;
        secondPoint = _secondPoint;
        isShooting = true;

        yield return new WaitForSeconds(0.35f);

        isShooting = false;
    }
}

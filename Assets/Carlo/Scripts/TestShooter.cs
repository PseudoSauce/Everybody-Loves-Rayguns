using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShooter : MonoBehaviour {

    public GameObject projectile;
    private GameObject lastProjectile = null;

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
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // How to teleport an object
            if (lastProjectile != null)
            { 
                RaycastHit hitInfo;

                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 300.0f))
                {
                    if (hitInfo.collider.CompareTag("Teleportable"))
                    {
                        if (lastProjectile.GetComponent<Beacon>().CanTeleport(hitInfo.collider.GetComponent<MeshRenderer>().bounds.extents))
                        {
                            hitInfo.collider.gameObject.transform.position = lastProjectile.GetComponent<Beacon>().GetTeleportPosition(hitInfo.collider.GetComponent<MeshRenderer>().bounds.extents);
                            hitInfo.collider.gameObject.transform.rotation = Quaternion.identity;
                            hitInfo.collider.GetComponent<Rigidbody>().velocity = Vector3.zero;
                        }
                    }
                }
            }
        }
    }
}

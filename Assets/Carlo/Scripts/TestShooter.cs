using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MyTypes;

public class TestShooter : MonoBehaviour {

    public GameObject projectile;
    public Transform gun;
    private GameObject beacon = null;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (beacon != null)
            {
                Destroy(beacon);    // Create cool destruction animation call that before destruction
            }

            beacon = Instantiate(projectile, Camera.main.transform.position, Camera.main.transform.rotation) as GameObject;
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            beacon.GetComponent<Beacon>().RotateOnXAxis();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            beacon.GetComponent<Beacon>().RotateOnYAxis();
        }
    }

    void FixedUpdate()
    {
        // How to teleport an object
        RaycastHit hitInfo;
        InteractMessage msg;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 300.0f))
        {
            if (hitInfo.collider.GetComponent<Interactable>())
            {
                if (beacon != null)
                {
                    msg = new InteractMessage(Interaction.TELEPORTING, "HitBegin");
                    hitInfo.collider.gameObject.SendMessage("Interact", msg);

                    if (Input.GetMouseButtonDown(0))
                    {
                        msg = new InteractMessage(Interaction.TELEPORTING, "Teleport");
                        hitInfo.collider.gameObject.SendMessage("Interact", msg);
                    }
                }
            }
            else
            {
                if (beacon != null)
                {
                    msg = new InteractMessage(Interaction.TELEPORTING, "HitEnd");
                    hitInfo.collider.gameObject.SendMessage("Interact", msg);
                }
            }
        }

    }

}

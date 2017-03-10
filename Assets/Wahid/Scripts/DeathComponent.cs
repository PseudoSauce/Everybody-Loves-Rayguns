using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MyTypes;

[AddComponentMenu("Custom Components/DeathComponent")]
[RequireComponent(typeof(Rigidbody))]
public class DeathComponent : Interactable {
    [Tooltip("Simply your health")]
    public int tempHitpoints = 100;
    [Tooltip("This is how fast you heal or die")]
    public float drainTimeStep = 0.1f;
    private int origHitpoints;
    private bool beingHit = false;
    private bool quickDeath = false;
    private float nextFire = 0;

    protected override void Init() {
        AssignInteractionType(Interaction.DEATH);
        AssignStart(MyStart);
        AssignUpdate(MyUpdate);
    }

    private void MyStart() {
        Debug.Log("DeathComponent: Starting...");
        origHitpoints = tempHitpoints;
    }

    private void MyUpdate(float deltaTime) {
        if (beingHit) {
            StartDeath(true);
        } else if (quickDeath) {
            StartDeath(false);
        } else {
            Heal();
        }
    }

    void StartDeath(bool normalDeath) {
        if (!normalDeath) {
            Destroy(this.gameObject);
        }
        if (Time.time > nextFire) {
            nextFire = Time.time + drainTimeStep;
            tempHitpoints--;
            print("health: " + tempHitpoints);
            if (tempHitpoints < 0) {
                Destroy(this.gameObject);
            }
        }
    }

    void Heal() {
        if (tempHitpoints < origHitpoints - 10) {
            //ala COD, same ratio as ROF
            if (Time.time > nextFire) {
                nextFire = Time.time + drainTimeStep;
                tempHitpoints++;
                print("health: " + tempHitpoints);
            }
        }
    }
    // place your custom logic here for interaction
    protected override void Commit(InteractMessage msg) {
        //optional time step for the beam hp drainage 
        //object[] objects = new object[msg.msgData.Count];
        //msg.msgData.CopyTo(objects, 0);
        //if (objects.Length > 0)
        //    if (objects[0].GetType() == typeof(float)) {
        //        drainTimeStep = (int)objects[0];
        //    }
        Debug.Log(this + ": " + msg);
        switch (msg.msg) {
            case "SENDHITS":
                beingHit = true;
                break;
            case "STOPHITS":
                beingHit = false;
                break;
            case "INSTAKILL":
                //short circuit death proc
                quickDeath = true;
                break;
        }
    }
}
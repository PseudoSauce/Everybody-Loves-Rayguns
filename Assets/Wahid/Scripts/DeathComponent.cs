using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;

using MyTypes;
//TODO: this is now too specific to the player, make generic compo later
[AddComponentMenu("Custom Components/DeathComponent")]
[RequireComponent(typeof(Rigidbody))]
public class DeathComponent : Interactable {
    [Tooltip("Simply your health")]
    public int tempHitpoints = 100;
    [Tooltip("This is how fast you heal or die")]
    public float drainTimeStep = 0.1f;
    [Tooltip("This is how many points you lose in one hit")]
    private int drainagePoints = 1;
    private int origHitpoints;
    private bool beingHit = false;
    private bool quickDeath = false;
    private float nextFire = 0;
    private float depletionRation = 0;

    [SerializeField]
    private Image m_healthBar;
    [SerializeField]
    private Text m_deathMessage;
    [SerializeField]
    private string m_bulletTag = "Bullet";
    [SerializeField]
    private string m_deathTag = "DeathZone";
    [SerializeField]
    private string m_spawnPointTag = "SpawnPoint";

    private Transform m_lastSpawnPoint;
    StoreTransform saveTrans;
    private bool m_respawning = false;
    public bool isDead = false;

    //handle multiple hits
    ArrayList uniqueInvokers = new ArrayList();
    private bool isFirstHit = true;
    private static int concurrencyCount = 0;

    //quick shader effect only applicable to player
    //add animation curve later
    Vingette deathVig;

    protected override void Init() {
        AssignInteractionType(Interaction.DEATH);
        AssignStart(MyStart);
        AssignUpdate(MyUpdate);
    }

    private void MyStart() {
        deathVig = Camera.main.gameObject.GetComponent<Vingette>();
        saveTrans = this.gameObject.transform.Save().Position();
        m_lastSpawnPoint = null;
        print("last spawn point" + m_lastSpawnPoint);
        Debug.Log("DeathComponent: Starting...");
        origHitpoints = tempHitpoints;
    }

    private void MyUpdate(float deltaTime) {
        if (!isDead && !m_respawning) {
            if (beingHit) {
                StartDeath(true);
            } else if (quickDeath) {
                quickDeath = false;
                StartDeath(false);
            } else {
                Heal();
            }
        }
    }

    void StartDeath(bool normalDeath) {
        if (!normalDeath && !m_respawning) {
            m_healthBar.fillAmount = 0;
            m_deathMessage.text = "You Died!";
            isDead = true;

            StartCoroutine(Respawn());
        } else if (Time.time > nextFire && !m_respawning) {
            SwitchVignette(true);
            nextFire = Time.time + drainTimeStep;
            tempHitpoints -= drainagePoints;
            m_healthBar.fillAmount = (float)tempHitpoints / (float)origHitpoints;
            print("health: " + tempHitpoints);
            if (tempHitpoints <= 0) {
                m_healthBar.fillAmount = 0;
                m_deathMessage.text = "You Died!";
                isDead = true;
                StartCoroutine(Respawn());
            } else {
                m_deathMessage.text = "";
            }
        }
        uniqueInvokers.Clear();
        beingHit = false;
    }

    void Heal() {
        if (tempHitpoints <= origHitpoints - depletionRation) {
            //ala COD, same ratio as ROF
            if (Time.time > nextFire) {
                SwitchVignette(false);
                nextFire = Time.time + drainTimeStep;
                tempHitpoints++;
                depletionRation++;
                m_healthBar.fillAmount = (float)tempHitpoints / (float)origHitpoints;
                print("health: " + tempHitpoints);
            }
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag(m_deathTag) && !m_respawning) {
            print(this + ": NEED A FLAG HERE TO PREVENT THESE FROM CALLING TWICE (OR MORE)");
            quickDeath = true;
        }

        if (other.CompareTag(m_spawnPointTag))
            m_lastSpawnPoint = other.gameObject.transform;
    }
    // place your custom logic here for interaction
    protected override void Commit(InteractMessage msg) {
        //object[] objects = new object[msg.msgData.Count];
        //poll unique invoker
        foreach (object[] id in msg.msgData) {
            int inID = (int)id[0];
            if (!uniqueInvokers.Contains(inID)) {
                uniqueInvokers.Add(inID);
            }
        }
        drainagePoints = uniqueInvokers.Count;
        print(drainagePoints);
        //int incomingmsgs = (int)((ArrayList)msg.msgData[0])[0]);
        //for (int i = 0; i < msg.msgData.Count; i++) {
        //    uniqueInvokers.Add msg.msgData[i];
        //}

        //int objectUID = (int)msg.msgData.FirstOrDefault<object>();
        //int objectsUID = 
        //print(objectUID);

        //msg.msgData.CopyTo(objects, 0);
        //check if we have it 
        //MULTIPLE; time step for the beam hp drainage 


        //if (objects.Length > 0)
        //    print("invokers: ") + objects[isFirstHit];
        //    //if (objects[0].GetType() == typeof(float)) {
        //    //    drainTimeStep = (int)objects[0];
        //    //}
        Debug.Log(this + ": " + msg.msg);
        switch (msg.msg) {
            case "SENDHITS":
                beingHit = true;
                break;
            case "INSTAKILL":
                //short circuit death proc
                quickDeath = true;
                break;
        }
    }
    private IEnumerator Respawn() {
        print("RESPAWNING");
        m_respawning = true;
        yield return new WaitForSeconds(2);
        if (m_lastSpawnPoint != null) {
            transform.position = m_lastSpawnPoint.position;
        } else {
            this.transform.Load(saveTrans).Position();
            //transform.position;
        }
        //reset everything
        isDead = false;
        tempHitpoints = origHitpoints;
        m_deathMessage.text = "";
        depletionRation = 0;
        m_respawning = false;
        deathVig.ResetVignette();
    }

    void SwitchVignette(bool switcher) {
        if (tempHitpoints < origHitpoints / 2) {
            float normalVigDeplete = Remap(drainagePoints, 0, origHitpoints / 2, 0, 1);
            deathVig.ConvergeVignette(switcher, normalVigDeplete);
        }
    }

    // TODO add this to a universal extension method...
    float Remap(float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
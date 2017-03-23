using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
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
    private bool isDead = false;

    protected override void Init() {
        AssignInteractionType(Interaction.DEATH);
        AssignStart(MyStart);
        AssignUpdate(MyUpdate);
    }

    private void MyStart() {
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
            nextFire = Time.time + drainTimeStep;
            tempHitpoints--;
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
        beingHit = false;
    }

    void Heal() {
        print("healing");
        if (tempHitpoints <= origHitpoints - depletionRation) {
            //ala COD, same ratio as ROF
            if (Time.time > nextFire) {
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
        //optional time step for the beam hp drainage 
        //object[] objects = new object[msg.msgData.Count];
        //msg.msgData.CopyTo(objects, 0);
        //if (objects.Length > 0)
        //    if (objects[0].GetType() == typeof(float)) {
        //        drainTimeStep = (int)objects[0];
        //    }
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
    }
}
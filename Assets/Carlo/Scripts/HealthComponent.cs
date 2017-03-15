using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthComponent : MonoBehaviour {

    //[SerializeField]
    private int m_maxHealth = 3;
    private int m_health;
    //[SerializeField]
    //private Image m_healthBar;
    [SerializeField]
    private Text m_deathMessage;
    //[SerializeField]
    //private string m_bulletTag = "Bullet";
    [SerializeField]
    private string m_deathTag = "DeathZone";
    [SerializeField]
    private string m_spawnPointTag= "SpawnPoint";

    private Transform m_lastSpawnPoint;
    private bool m_respawning = false;

    void Start()
    {
        m_health = m_maxHealth;
    }

	void Update ()
    {
        if (!m_respawning)
        {
            if (IsDead())
            {
                m_respawning = true;
                StartCoroutine(Respawn());
            }
        }

        //m_healthBar.fillAmount = (float)m_health / (float)m_maxHealth;
	}

    //void OnCollisionEnter(Collision col)
    //{
    //    if(col.collider.CompareTag(m_bulletTag))
    //    {
    //        m_health--;
    //        if (m_health < 0)
    //            m_health = 0;
    //    }
    //}

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(m_deathTag))
            m_health = 0;
        if (other.CompareTag(m_spawnPointTag))
            m_lastSpawnPoint = other.gameObject.transform;
    }

    private bool IsDead()
    {
        if(m_health <= 0)
        {
            m_deathMessage.text = "You Died!";
            return true;
        }
        else
        {
            m_deathMessage.text = "";
            return false;
        }
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(2);

        transform.position = m_lastSpawnPoint.position;
        //transform.rotation = m_lastSpawnPoint.rotation;
        m_respawning = false;
        m_health = m_maxHealth;
    }
}

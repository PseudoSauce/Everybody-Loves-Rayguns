using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreScript : MonoBehaviour {
    private int index = 0;
    private Transform[] allChildren;
    private List<ParticleSystem> childrenEffects;
    public GameObject bob;

    void Awake() {
        childrenEffects = new List<ParticleSystem>();
        allChildren = GetComponentsInChildren<Transform>();

        foreach (Transform child in allChildren) {
            if (child.CompareTag("Particle")) {
                childrenEffects.Add(child.GetComponent<ParticleSystem>());
            }
        }
    }

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.I)) {
            index++;
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("PickUp")) {
            Destroy(other);
            if (index < childrenEffects.Count) {
                childrenEffects[index].Play();
                index++;
            } else {
                StartCoroutine(subsequentSpawn());
            }
        }
    }

    private IEnumerator subsequentSpawn() {
        yield return new WaitForSeconds(3.0f);
        Instantiate(bob);
        Destroy(this.gameObject);
    }
}

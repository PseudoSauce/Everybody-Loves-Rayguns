using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ExplosionScript : MonoBehaviour {
    private ParticleSystem particles;

    private void Awake()
    {
        particles = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (particles.isStopped)
        {
            Destroy(gameObject);
        }
    }
}

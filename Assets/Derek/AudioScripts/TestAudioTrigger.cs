using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAudioTrigger : MonoBehaviour {
    [SerializeField]
    string musicName;

    [SerializeField]
    string phaserSoundName;

    AudioManager manager;

	// Use this for initialization
	void Start () {
        manager = GetComponent<AudioManager>();

        manager.PlaySound(musicName, true);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space))
        {
            manager.PlaySoundConcurrent(phaserSoundName, false);
        }
	}
}

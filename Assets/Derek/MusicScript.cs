using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicScript : MonoBehaviour {
    [SerializeField]
    private string m_MusicName;

    private AudioManager m_Manager;

    private void Awake()
    {
        m_Manager = GetComponent<AudioManager>();
    }

	// Use this for initialization
	void Start () {
		m_Manager.PlaySound(m_MusicName, true);
	}
}

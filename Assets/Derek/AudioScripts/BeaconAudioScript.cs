using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeaconAudioScript : MonoBehaviour {
    [SerializeField, Range(0.0f, 1.0f)]
    private float m_Volume = 1.0f;

    private AudioManager m_AudioManager;
    private List<string> m_BeaconNotes;

    private int m_NoteIndex = 0;

    bool m_HasPlayedNote = true;

    private void Awake()
    {
        m_AudioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        m_BeaconNotes = new List<string>();
    }

    private void Start()
    {
        LoadNotes();        
    }

    public void PlayNextNote()
    {
        if (m_BeaconNotes.Count > 0)
        {
            m_HasPlayedNote = m_AudioManager.PlaySoundConcurrent(m_BeaconNotes[m_NoteIndex], false, m_Volume);
            m_NoteIndex = m_HasPlayedNote ? m_NoteIndex + 1 : m_NoteIndex;

            if (m_NoteIndex >= m_BeaconNotes.Count)
            {
                m_NoteIndex = 0;
            }
        }

    }

    private void LoadNotes()
    {
        var note1 = Resources.Load("Audio/beacon-notes/beacon1") as AudioClip;
        var note2 = Resources.Load("Audio/beacon-notes/beacon2") as AudioClip;
        var note3 = Resources.Load("Audio/beacon-notes/beacon3") as AudioClip;
        var note4 = Resources.Load("Audio/beacon-notes/beacon4") as AudioClip;

        if (note1 != null)
        {
            m_BeaconNotes.Insert(0, note1.name);
            m_AudioManager.PushSound(note1);
        }
        if (note2)
        {
            m_BeaconNotes.Insert(1, note2.name);
            m_AudioManager.PushSound(note2);
        }
        if (note3)
        {
            m_BeaconNotes.Insert(2, note3.name);
            m_AudioManager.PushSound(note3);
        }
        if (note4)
        {
            m_BeaconNotes.Insert(3, note4.name);
            m_AudioManager.PushSound(note4);
        }
    }
}

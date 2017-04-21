using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour {
    [SerializeField]
    private AudioClip [] m_Clips;

    private AudioSource m_MainSource;
    private Dictionary<string, AudioClip> m_ClipDic;

    private List<AudioSource> m_Playing;

    private void Awake()
    {
        m_MainSource = GetComponent<AudioSource>();

        m_ClipDic = new Dictionary<string, AudioClip>();

        m_Playing = new List<AudioSource>();

        foreach (var clip in m_Clips)
        {
            m_ClipDic.Add(clip.name, clip);
        }
    }

    void Update()
    {
        foreach(var src in m_Playing)
        {
            if (!src.isPlaying)
            {
                Destroy(src);
            }
        }

        m_Playing.Clear();
    }

    public void PushSound(AudioClip clip)
    {
        if (clip != null && !m_ClipDic.ContainsKey(clip.name))
        {
            print(clip.name);
            m_ClipDic.Add(clip.name, clip);
        }
    }

	public void PlaySound(string soundName, bool loop)
    {
        AudioClip clip = null;

        if (m_ClipDic.TryGetValue(soundName, out clip))
        {
            while (m_MainSource.isPlaying)
            {            
            }

            m_MainSource.clip = clip;  
            
            m_MainSource.loop = loop;
             
            m_MainSource.Play();         
        }
    }

    public bool PlaySoundConcurrent(string soundName, bool loop, bool layerSound = false)
    {
        AudioClip clip = null;

        if (m_ClipDic.TryGetValue(soundName, out clip))
        {
            // look for any free sources
            var sources = GetComponents<AudioSource>();

            AudioSource source = null;

            foreach(var src in sources)
            {
                if (!layerSound && src.clip.name == soundName && src.isPlaying)
                {
                    return true;
                }
                if (!src.isPlaying && source == null)
                {
                    source = src;

                    if (layerSound)
                    {
                        break;
                    }
                }
            }

            if (source == null)
            {
                source = gameObject.AddComponent<AudioSource>();
                m_Playing.Add(source);
            }

            source.loop = loop;
            source.clip = clip;
            source.Play();

            return true;
        }

        return false;
    }

    public void PauseSound(bool b)
    {
        if (m_MainSource.isPlaying)
        {
            if (b)
            {
                m_MainSource.Pause();
            }
            else
            {
                m_MainSource.UnPause();
            }
        }
    }

    public void PauseSoundsConcurrent(string soundName, bool b)
    {
        // look for any free sources
        var sources = GetComponents<AudioSource>();

        foreach (var src in sources)
        {
            if (src.clip.name == name && src != m_MainSource && src.isPlaying)
            {
                if (b)
                {
                    src.Pause();
                }
                else
                {
                    src.UnPause();
                }
            }
        }
    }

    public void StopSound()
    {
        if (m_MainSource.isPlaying)
        {
            m_MainSource.Stop();
        }
    }

    public void StopSoundsConcurrent(string name)
    {
        // look for any free sources
        var sources = GetComponents<AudioSource>();

        foreach (var src in sources)
        {
            if (src.clip.name == name && src != m_MainSource && src.isPlaying)
            {
                src.Stop();
            }
        }
    }
}

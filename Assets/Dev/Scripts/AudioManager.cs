using System;
using UnityEngine;

[Serializable]
public class Sound
{
    public AudioClip Clip;
    public string Name;

    [Range(0f, 1f)] public float Volume;

    [Range(.1f, 3f)] public float Pitch;

    public bool RandomizePitch = false;

    public bool Loop;

    [HideInInspector] public AudioSource Source;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public Sound[] Sounds;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        foreach (var sound in Sounds)
        {
            sound.Source = gameObject.AddComponent<AudioSource>();
            DontDestroyOnLoad(sound.Source);
            sound.Source.clip = sound.Clip;
            sound.Source.volume = sound.Volume;
            sound.Source.pitch = sound.Pitch;
            sound.Source.loop = sound.Loop;
        }
    }

    private void Start()
    {
        Instance.Play("main-theme");
        Instance.Play("cave-droplets");
    }

    public void Play(string soundName)
    {
        Debug.Log("Sounds size is " + Sounds.Length);
        var sound = Array.Find(Sounds, sound => sound.Name == soundName);
        if (sound == null || sound.Source == null)
        {
            Debug.LogWarning("AudioManager: Cannot find sound " + soundName + ": " + sound);
            if (sound != null)
            {
                Debug.Log("source is " + sound.Source);
            }
            return;
        }

        if (sound.RandomizePitch)
        {
            sound.Source.pitch = UnityEngine.Random.Range(.8f, 1.2f);
        }

        sound.Source.Play();
    }
}

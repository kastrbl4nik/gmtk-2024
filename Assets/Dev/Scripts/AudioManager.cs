using System;
using UnityEngine;

[Serializable]
public class Sound
{
    public AudioClip Clip;
    public string Name;

    [Range(0f, 1f)] public float Volume;

    [Range(.1f, 3f)] public float Pitch;

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
            sound.Source.clip = sound.Clip;
            sound.Source.volume = sound.Volume;
            sound.Source.pitch = sound.Pitch;
            sound.Source.loop = sound.Loop;
        }
    }

    public void Play(string name, float maxPitch = float.NaN, float minPitch = float.NaN)
    {
        var sound = Array.Find(Sounds, sound => sound.Name == name);
        if (sound == null)
        {
            Debug.LogWarning("AudioManager: Cannot find sound " + name);
            return;
        }

        if (float.IsNaN(maxPitch) || float.IsNaN(minPitch))
        {
            sound.Source.pitch = sound.Pitch;
        }
        else
        {
            sound.Source.pitch = sound.Pitch + UnityEngine.Random.Range(minPitch, maxPitch);
        }

        sound.Source.Play();
    }
}

using UnityEngine.Audio;
using UnityEngine;
using System.Diagnostics.CodeAnalysis;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f,1f)]
    public float volume;
    [Range(0.1f,3f)]
    public float pitch;
    [Range(0f, 0.5f)]
    public float pitchVariation = 0f;

    [Range(0f,1f)]
    public float spacialBlend;
    [Range(0f, 5f)]
    public float doppler = 1f;
    public float maxDist = 2000f;

    [Range(0f, 1.1f)]
    public float reverb = 1f;


    [HideInInspector]
    public AudioSource source;

    public bool loop = false;
    public bool playOnAwake = false;

    [HideInInspector]
    public float startPitch;


}

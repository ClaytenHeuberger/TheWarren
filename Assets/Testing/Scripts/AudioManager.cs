using System.Collections;
using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    [SerializeField] private GameObject audioSourceObj;

    float startPitch;
    void Awake()
    {
        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;

            s.source.spatialBlend = s.spacialBlend;
            s.source.dopplerLevel = s.doppler;
            s.source.maxDistance = s.maxDist;
            s.source.loop = s.loop;
            s.source.playOnAwake = s.playOnAwake;

            s.startPitch = s.pitch;
        }

    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        float pitchVar = UnityEngine.Random.Range(-s.pitchVariation, s.pitchVariation);
        float tempPitch = s.source.pitch;
        s.source.pitch = s.startPitch + pitchVar;

        s.source.PlayOneShot(s.clip);

    }

    public void PlayAtPos(string name, Vector3 pos, float pitchVariation, float spatialBlend)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        GameObject sourceObj = Instantiate(audioSourceObj, pos, Quaternion.identity);
        AudioSource source = sourceObj.GetComponent<AudioSource>();
        source.clip = s.source.clip;
        source.volume = s.source.volume;
        source.pitch = s.source.pitch;
        source.spatialBlend = spatialBlend;
        source.dopplerLevel = s.source.dopplerLevel;
        source.maxDistance = s.source.maxDistance;
        source.loop = s.source.loop;
        
        float randomPitch = UnityEngine.Random.Range(s.source.pitch - pitchVariation, s.source.pitch + pitchVariation);
        source.pitch = randomPitch;
        source.Play();
        StartCoroutine(KillSource(s.clip.length, sourceObj));

    }

    IEnumerator KillSource(float clipLength, GameObject toKill)
    {

        yield return new WaitForSeconds(clipLength);
        Destroy(toKill);
    }
}

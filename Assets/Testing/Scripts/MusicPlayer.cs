using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Layouts;
public class MusicPlayer : MonoBehaviour
{

    #region Attributes

    // All Audio Clips that can be used as music tracks.
    public List<AudioClip> musicClips = new List<AudioClip>();
    [Range(0.0f, 1.5f)]
    public List<float> volumes = new List<float>();

    public MusicQueue musicQueue;

    // 2D music player.
    public static AudioSource music;

    // Audio currently playing.
    AudioClip currentTrack;

    // Length of the current track; used to know when to play next
    private float length;

    private Coroutine musicLoop;

    private AudioSource musicSource;

    private GameHandler gameHandler;

    #endregion

    void Start()
    {
        musicQueue = new MusicQueue(musicClips);

        musicSource = GetComponent<AudioSource>(); 
        gameHandler = FindObjectOfType<GameHandler>();

        //StartMusic();
    }

    #region Audio Playing

    public void PlayMusicClip(AudioClip music)
    {
        musicSource.Stop();
        musicSource.clip = music;
        musicSource.Play();

        //GetComponent<AudioVisualization>().ResetBandHighest();
    }

    public void StopMusic()
    {
        if (musicLoop != null)
            StopCoroutine(musicLoop);

        music.Stop();
    }

    public void StartMusic()
    {
        musicLoop = StartCoroutine(musicQueue.LoopMusic(this, 0, PlayMusicClip, volumes));
    }

    #endregion
}

// Queue of multiple audioclips that automatically randomizes and may repeat with given delay
public class MusicQueue
{
    List<AudioClip> clips;
    
    public MusicQueue(List<AudioClip> clips)
    {
        this.clips = clips;
    }

    public IEnumerator LoopMusic(MonoBehaviour player, float delay, System.Action<AudioClip> playFunction, List<float> volumes)
    {
        while (true)
        {
            yield return player.StartCoroutine(Run(RandomizeList(clips), delay, playFunction, player, volumes));
        }
    }

    // Runs all music clips, then repeats if desired
    public IEnumerator Run(List<AudioClip> tracks,
        float delay, System.Action<AudioClip> playFunction, MonoBehaviour player, List<float> volumes)
    {
        // Run all clips
        foreach (AudioClip clip in tracks)
        {
            // play
            playFunction(clip);

            int index = tracks.IndexOf(clip);
            player.GetComponent<AudioSource>().volume = volumes[index];
            
            // Wait until the clip is done, and delay between clips is over
            yield return new WaitForSeconds(clip.length + delay);
        }
    }

    public List<AudioClip> RandomizeList(List<AudioClip> list)
    {
        List<AudioClip> copy = new List<AudioClip>(list);

        int n = copy.Count;

        // what we do here is grab any random track,
        // then set the last track in the copy to be that track,
        // then we remove the last track from the list of tracks we need to change.

        // basically, we move from largest index to smallest,
        // setting the current index to a random clip from the smallest index
        // and up to the largest index that has not been set
        while (n > 1)
        {
            n--;

            // exclusive int range, add one since we remove one earlier
            int k = Random.Range(0, n + 1);

            // store temporary value
            AudioClip value = copy[k];

            // swap without overwrite
            copy[k] = copy[n];
            copy[n] = value;
        }

        return copy;
    }
}

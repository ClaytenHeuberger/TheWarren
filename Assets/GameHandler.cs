using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    public Image colorOverlay;
    public TextMeshProUGUI promptText;
    

    [HideInInspector]
    public bool started = false;

    bool temp = false;

    MusicPlayer musicPlayer;

    private void Start()
    {
        musicPlayer = FindObjectOfType<MusicPlayer>();
    }

    private void Update()
    {
        if(started && !temp)
        {
            musicPlayer.StartMusic();
            temp = true;
        }
    }
}

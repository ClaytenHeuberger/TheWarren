using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayPrompt : MonoBehaviour
{
    TextMeshProUGUI text;
    [SerializeField] float speed = 1.0f;
    [SerializeField] float factor = 0.7f;
    [SerializeField] float minVolume = 0.2f;
    [SerializeField] float maxVolume = 0.8f;

    float state = 0;
    AudioSource source;
    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        source = GetComponent<AudioSource>();
    }

    private void Update()
    {
        state = Mathf.Sin(Time.time * speed);

        Color tempCol = text.color;

        tempCol.a = Mathf.Pow(state, factor);

        text.color = tempCol;
        source.volume = Mathf.Lerp(minVolume, maxVolume, Mathf.Pow(state, factor));


    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class thrusterSourceScript : MonoBehaviour
{
    [SerializeField] GameObject Player;
    [SerializeField] private float volumeVariation;
    [SerializeField] private float pitchVariation;
    AudioSource source;

    Rigidbody rb;
    private void Start()
    {
        source = GetComponent<AudioSource>();
        rb = Player.GetComponent<Rigidbody>();
    }
    void Update()
    {
        if(rb.velocity.magnitude > 0)
        {
            float thrustRatio = Player.GetComponent<PlayerScript>().thrustRatio;

            source.volume = 0.3f + thrustRatio * volumeVariation;
            source.pitch = 0.8f + thrustRatio * pitchVariation;
        }
    }
}

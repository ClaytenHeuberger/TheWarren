using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrusterParticleScript : MonoBehaviour
{
    ParticleSystem ps;
    PlayerScript playerScript;

    bool joystickOn = false;
    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
        playerScript = FindObjectOfType<PlayerScript>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Space))
        {
            ps.Play();
        }
        else if (Input.GetMouseButtonUp(1) || Input.GetKeyUp(KeyCode.Space))
        {
            ps.Stop();
        }

        if((playerScript.joystickThrust && !joystickOn))
        {
            joystickOn = true;
            ps.Play();
        }else if(!playerScript.joystickThrust && joystickOn)
        {
            joystickOn = false;
            ps.Stop();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrusterParticleScript : MonoBehaviour
{
    ParticleSystem ps;
    PlayerScript playerScript;

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
        playerScript = FindObjectOfType<PlayerScript>();
    }
    void Update()
    {


        ParticleSystem.EmissionModule temp = ps.emission;
        temp.rateOverTime = Mathf.RoundToInt(playerScript.thrustRatio * 200);


    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecreaseLightIntensity : MonoBehaviour
{
    void Start()
    {
        GetComponent<Light>().intensity /= 25f;
    }

}

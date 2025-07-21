using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PIDController : MonoBehaviour
{
    public float Calculate(float dt, float currentValue, float targetValue, float ROC, float pGain, float dGain)
    {
        float error = targetValue - currentValue;


        //P term
        float P = pGain * error;

        //D term

        float errorRateOfChange = ROC;

        
        float D = dGain * errorRateOfChange;

        return P + D;
    }
}

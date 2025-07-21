using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenePosAdjust : MonoBehaviour
{
    [SerializeField] private Transform Player;


    void Update()
    {
        if(Player.position.magnitude > 10000)
        {
            transform.position -= Player.position;

        }
    }
}

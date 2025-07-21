using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shockwave : MonoBehaviour
{

    Transform player;
    void Start()
    {

        player = GameObject.Find("Player").transform;
        transform.LookAt(player);
    }


}

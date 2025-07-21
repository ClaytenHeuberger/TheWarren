using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Camera cam;
    [SerializeField] float distance = 10f;

    RectTransform rectTrans;
    private void Start()
    {
        rectTrans = GetComponent<RectTransform>();
    }
    void FixedUpdate()
    {
        rectTrans.position = Camera.main.WorldToScreenPoint(player.position + player.forward * distance);
    }
}

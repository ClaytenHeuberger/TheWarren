using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class NameTagCanvas : MonoBehaviour
{
    Canvas canvas;

    [SerializeField] RectTransform nameTagTransform;

    Camera cam;
    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
        cam = Camera.main;
    }

    private void Update()
    {
        nameTagTransform.position = transform.position + 0.5f * Vector3.up;
        nameTagTransform.LookAt(transform.position - cam.transform.position);
    }
}

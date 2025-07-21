using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerCam : MonoBehaviour
{

    Camera cam;
    Camera parentCam;
    private void Start()
    {
        cam = GetComponent<Camera>();
        parentCam = transform.parent.GetComponent<Camera>();
    }
    void Update()
    {
        cam.fieldOfView = parentCam.fieldOfView;
    }
}

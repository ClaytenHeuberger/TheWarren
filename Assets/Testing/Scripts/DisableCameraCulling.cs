using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableCameraCulling : MonoBehaviour
{

    private void Start()
    {

        GetComponent<Camera>().cullingMatrix = Matrix4x4.identity; // disables all culling

    }
}

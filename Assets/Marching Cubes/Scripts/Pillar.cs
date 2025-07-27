using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : MonoBehaviour
{
    public void SetPosition(Vector3 position)
    {
        float noise = CaveDetailTools.GetPillarNoise(position);
        Vector3 dir = new Vector3(noise * 5000, noise * noise * 5000, noise * noise * noise * 10000);

        RaycastHit hit1;
        RaycastHit hit2;

        Physics.Raycast(position, dir, out hit1, Mathf.Infinity, LayerMask.GetMask("Cave"));
        Physics.Raycast(position, -dir, out hit2, Mathf.Infinity, LayerMask.GetMask("Cave"));

        if(hit1.point != Vector3.zero && hit2.point != Vector3.zero)
        {

            Vector3 pos = (hit1.point + hit2.point) / 2f;


            transform.position = pos;
            transform.up = dir;
            transform.localScale = Vector3.one * (Vector3.Distance(hit1.point, hit2.point) / 2f);
        }

    }
}

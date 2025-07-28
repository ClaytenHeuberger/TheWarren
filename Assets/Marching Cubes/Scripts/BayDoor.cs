using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class BayDoor : MonoBehaviour
{

    [SerializeField] private GameObject bayDoor;
    [SerializeField] private float duration = 8f;
    [SerializeField] private float height = 6f;
    [SerializeField] private AudioSource doorOpenAudio;
    [SerializeField] private AudioSource doorCloseAudio;
    Vector3 startPos;


    bool busy = false;
    bool waiting = false;
    private void Start()
    {
        startPos = bayDoor.transform.position;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if(!busy)
                StartCoroutine(OpenDoor(duration, height));
            else
                waiting = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player" && !busy)
        {

            if (!busy)
                StartCoroutine(CloseDoor(duration));
            else
                waiting = true;
        }
    }
    IEnumerator OpenDoor(float duration, float height)
    {
        busy = true;

        doorOpenAudio.Play();
        float elapsed = 0.0f;
        Vector3 currentPos = bayDoor.transform.position;
        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;
            bayDoor.transform.position = Vector3.Lerp(currentPos, startPos + height * Vector3.up, elapsed / duration);

            yield return null;
        }

        busy = false;

        if (waiting)
        {
            StartCoroutine(CloseDoor(duration));
            waiting = false;
        }

    }
    IEnumerator CloseDoor(float duration)
    {
        busy = true;

        doorCloseAudio.Play();

        float elapsed = 0.0f;
        Vector3 currentPos = bayDoor.transform.position;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            bayDoor.transform.position = Vector3.Lerp(currentPos, startPos, elapsed / duration);

            yield return null;
        }

        busy = false;


        if (waiting)
        {
            StartCoroutine(OpenDoor(duration, height));
            waiting = false;
        }
    }
}

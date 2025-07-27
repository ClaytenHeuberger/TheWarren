using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class BayDoor : MonoBehaviour
{

    [SerializeField] private GameObject bayDoor;
    Vector3 startPos;

    private void Start()
    {
        startPos = bayDoor.transform.position;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            StopCoroutine(CloseDoor(5f));
            StartCoroutine(OpenDoor(5f, 6f));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            StopCoroutine(OpenDoor(5f, 6f));
            StartCoroutine(CloseDoor(5f));
        }
    }
    IEnumerator OpenDoor(float duration, float height)
    {
        float elapsed = 0.0f;
        Vector3 currentPos = bayDoor.transform.position;
        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;
            bayDoor.transform.position = Vector3.Lerp(currentPos, startPos + height * Vector3.up, elapsed / duration);

            yield return null;
        }
    }
    IEnumerator CloseDoor(float duration)
    {
        float elapsed = 0.0f;
        Vector3 currentPos = bayDoor.transform.position;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            bayDoor.transform.position = Vector3.Lerp(currentPos, startPos, elapsed / duration);

            yield return null;
        }
    }
}

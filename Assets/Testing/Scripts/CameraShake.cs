using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

    public void PrimeShake(float duration, float magnitude)
    {
        StartCoroutine(Shake(duration, magnitude));

    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        
        float elapsed = 0.0f;

        while(elapsed < duration)
        {
            float factor = (elapsed / duration);

            float x = Random.Range(-1f, 1f) * magnitude / (1 + Mathf.Pow(factor,3));
            float y = Random.Range(-1f, 1f) * magnitude / (1 + Mathf.Pow(factor,3));

            transform.localEulerAngles = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    [SerializeField] private GameObject shieldLight;
    [SerializeField] private float hitDuration = 1f;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private EnemyHandler enemyHandler;


    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        enemyHandler = FindObjectOfType<EnemyHandler>();
    }

    private void Update()
    {
        transform.rotation = Quaternion.identity;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "EnemyBullet")
        {
            Destroy(collision.gameObject);

            Vector3 pos = collision.GetContact(0).point;
            StartCoroutine(ShieldHit(pos, hitDuration));

            if(!enemyHandler.scoreIsUpdating)
                enemyHandler.AddScore(-5, Color.red);

            audioManager.Play("ShieldHit");
        }

    }


    IEnumerator ShieldHit(Vector3 pos, float duration)
    {

        Vector3 direction = (pos - transform.position).normalized;

        float elapsed = 0f;
        float distance = 0.2f;

        GameObject pointLight = Instantiate(shieldLight, pos, Quaternion.identity);
        pointLight.transform.parent = transform;
       
        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;

            pointLight.transform.position = Vector3.Lerp(pointLight.transform.position, pointLight.transform.position + direction * distance, elapsed / duration);
            pointLight.GetComponent<Light>().intensity = Mathf.Lerp(pointLight.GetComponent<Light>().intensity, 0f, Mathf.Pow(elapsed / duration,10));

            yield return null;
        }
        
        Destroy(pointLight);
    }
}

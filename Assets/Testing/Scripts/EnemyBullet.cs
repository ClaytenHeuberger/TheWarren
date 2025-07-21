using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{

    Rigidbody rb;
    public float speed;
    [SerializeField] private GameObject dustPoof;

    [HideInInspector]
    [SerializeField] public AudioManager audioManager;
    [SerializeField] private float lifeTime;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;

        StartCoroutine(Die());
    }


    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Asteroid")
        {
            Instantiate(dustPoof, transform.position, Quaternion.Euler(collision.contacts[0].normal));

            //audioManager.PlayAtPos("AsteroidHit", transform.position, 0.3f);
            Destroy(gameObject);
        }
    }

    IEnumerator Die()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }

}

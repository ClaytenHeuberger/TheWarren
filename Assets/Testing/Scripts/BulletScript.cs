using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{

    Rigidbody rb;
    public float speed;
    [SerializeField] private GameObject dustPoof;

    [HideInInspector]
    [SerializeField] public AudioManager audioManager;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;

    }


    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Asteroid")
        {
            Instantiate(dustPoof, transform.position, Quaternion.Euler(collision.contacts[0].normal));
            
            audioManager.PlayAtPos("AsteroidHit", transform.position, 0.3f, 1f);
        }

        Destroy(gameObject);

    }

}

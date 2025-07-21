using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class asteroid : MonoBehaviour
{
    [SerializeField] private float spin = 0.1f;
    [SerializeField] public GameObject asteroidGenerator;
    [SerializeField] public Transform Player;
    float rotX;
    float rotY;
    float rotZ;
    Rigidbody rb;
    private void Start()
    {
        rotX = Random.Range(-spin, spin);
        rotY = Random.Range(-spin, spin);
        rotZ = Random.Range(-spin, spin);
        rb = GetComponent<Rigidbody>();
        rb.angularVelocity = new Vector3(rotX, rotY, rotZ);

        transform.eulerAngles = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));

        rb.mass = transform.localScale.magnitude * 5f;
    }
    private void FixedUpdate()
    {
        if(Vector3.Distance(Player.position, transform.position) > 600)
        {
            Vector3 newPos = asteroidGenerator.GetComponent<AsteroidSpawning>().GetPolarCoordinates();
            transform.position = newPos;
        }
    }

}

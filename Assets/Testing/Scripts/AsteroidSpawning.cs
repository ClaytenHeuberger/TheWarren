using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AsteroidSpawning : MonoBehaviour
{
    [SerializeField] private int radius = 200;
    [SerializeField] private int occlusionRadius = 20;
    [SerializeField] private int count = 100;
    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private float minSize = 1;
    [SerializeField] private float maxSize = 10;
    [SerializeField] private Transform God;
    [SerializeField] private Transform Player;

    void Start()
    {


        for (int i = 0; i < count; i++)
        {
            GameObject newAsteroid = Instantiate(asteroidPrefab, GetPolarCoordinates(), Quaternion.identity);

            float size = Random.Range(minSize, maxSize);
            newAsteroid.transform.localScale = Vector3.one * size;

            newAsteroid.GetComponent<asteroid>().asteroidGenerator = gameObject;
            newAsteroid.GetComponent<asteroid>().Player = Player;
            newAsteroid.GetComponent<Rigidbody>().mass = size;
        }
    }

    public Vector3 GetPolarCoordinates()
    {
        float r = Random.Range(occlusionRadius, radius);

        if (Time.frameCount < 10)
        {
            r = Mathf.Pow((float)Random.Range(30, radius) / (float)radius, 0.5f) * (float)radius;
        }

        float theta = Random.Range(0, Mathf.PI / 2); //Front hemisphere only

        if(Time.frameCount < 10)
            theta = Random.Range(0, Mathf.PI);

        float alpha = Random.Range(-Mathf.PI, Mathf.PI);
        float x = r * Mathf.Sin(theta) * Mathf.Cos(alpha);
        float y = r * Mathf.Sin(theta) * Mathf.Sin(alpha);
        float z = r * Mathf.Cos(theta);

        return transform.TransformPoint(new Vector3(x,y,z));
    }

    static Vector3 Perlin3D(Vector3 pos)
    {
        float AB = Mathf.PerlinNoise(pos.x, pos.y);
        float BC = Mathf.PerlinNoise(pos.y, pos.z);
        float AC = Mathf.PerlinNoise(pos.x, pos.z);

        float BA = Mathf.PerlinNoise(pos.y, pos.x);
        float CB = Mathf.PerlinNoise(pos.z, pos.y);
        float CA = Mathf.PerlinNoise(pos.z, pos.x);

        float A = (AB + AC + BA + CA) / 4;
        float B = (AB + BC + BA + CB) / 4;
        float C = (BC + AC + CB + CA) / 4;

        return new Vector3(A - 0.5f, B - 0.5f, C - 0.5f);
    }

}

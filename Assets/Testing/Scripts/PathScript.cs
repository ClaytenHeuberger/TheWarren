using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathScript : MonoBehaviour
{
    [Header("Path Variables")]
    [SerializeField] private float speed = 1f;
    [SerializeField] private float scale = 0.05f;
    [SerializeField] private float turnRate = 1f;

    [Header("Generation Variables")]
    [SerializeField] private float maxRadius = 15f;
    [SerializeField] private float minRadius = 4f;
    [SerializeField] private float radiusScale = 50f;

    [SerializeField] private float rockSize = 4800;

    [SerializeField] private GameObject[] rocks;
    [SerializeField] private GameObject floorRock;
    [SerializeField] private float warpScale = 1f;
    [SerializeField] private float warpAmplitude = 1f;


    [Header("Objects")]
    [SerializeField] private GameObject detailMaker;


    private Vector2 prevGridLocation = Vector2.zero;

    float angleOffset = 0f;

    List<Vector3> prevPositions = new List<Vector3>();
    List<Quaternion> prevRotations = new List<Quaternion>();


    int iterations = 0;

    public void moveForward()
    {
        iterations++;


        transform.position += transform.forward * speed;

        prevPositions.Add(transform.position);
        prevRotations.Add(transform.rotation);

        //Set detail object behind this object
        if (iterations > 15)
        {
            detailMaker.transform.position = prevPositions[prevPositions.Count - 15];
            detailMaker.transform.rotation = prevRotations[prevRotations.Count - 15];
            //Debug.Log(prevPositions[prevPositions.Count - 4] - transform.position);
        }

        float rotX = (Mathf.PerlinNoise(transform.position.x * scale, transform.position.z * scale) - 0.5f) * turnRate;
        float rotY = (Mathf.PerlinNoise(transform.position.y * scale, transform.position.z * scale) - 0.5f) * turnRate;
        transform.Rotate(rotX, rotY, 0);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(Vector3.forward), scale * 5);


        //Spawn Tunnel -------------------------------------------------------------------------------------------------------------------------------

        //Change radius of tunnel based on noise
        float radiusNoise = Mathf.PerlinNoise(transform.position.x * radiusScale, transform.position.z * radiusScale);
        radiusNoise *= Mathf.PerlinNoise(transform.position.x * radiusScale / 3, transform.position.z * radiusScale / 3);
        float radius = radiusNoise * (maxRadius - minRadius) + minRadius;
        float objectsPerCycle = (int)Mathf.Floor(radius * 0.004f) + 3;
        //Spawn Tunnel

        //Offset angle of spawn points based on noise
        angleOffset = Perlin3D(transform.position * 5).magnitude * 360;
        float stepAngle = 360 / objectsPerCycle;
        for (int i = 0; i < objectsPerCycle; i++)
        {
            Vector3 spawnPos = transform.position + Quaternion.AngleAxis(stepAngle * i + angleOffset, transform.forward) * transform.up * radius;

            //Change rock type based on noise
            float rockSeed_scale = 10000;
            float rockSeed = Mathf.Abs((float)NoiseS3D.Noise(spawnPos.x * rockSeed_scale, spawnPos.y * rockSeed_scale, spawnPos.z * rockSeed_scale));
            int rockNum = (int)Mathf.Floor(rockSeed * rocks.Length);


            float downAngle = Vector3.Angle(spawnPos - transform.position, Vector3.down);

            if(downAngle < 40)
            {
                if (iterations % 2 == 0)
                {

                    //Create Floor Rock

                    GameObject newRock = null;
                    //Instantiate new if pool isn't full
                    if (TunnelPool.instance.floorRocks.Count < TunnelPool.instance.num_floorRocks)
                    {
                        //Make new rock
                        newRock = Instantiate(floorRock, spawnPos, Quaternion.identity);
                        //Set inactive
                        newRock.SetActive(false);
                        //Add to list
                        TunnelPool.instance.floorRocks.Add(newRock);
                    }
                    else //Otherwise grab from pool
                    {
                        //Grab last object in list
                        newRock = TunnelPool.instance.floorRocks[0];

                        //Reorder object in list
                        TunnelPool.instance.floorRocks.Remove(newRock);
                        TunnelPool.instance.floorRocks.Add(newRock);

                        //Set inactive
                        newRock.SetActive(false);

                        //Reset position
                        newRock.transform.position = spawnPos;
                        newRock.transform.rotation = Quaternion.identity;
                    }

                    if (newRock != null)
                    {

                        //Flatten around tunnel
                        //newRock.transform.LookAt(transform.position); //No rotation for floor rocks

                        //3D Movement Noise
                        Vector3 positionNoise = Perlin3D(newRock.transform.position * warpScale) * warpAmplitude;
                        newRock.transform.position -= Vector3.up * (50 + positionNoise.z * radius / 20); //more noise than outer rock
                        newRock.transform.position += new Vector3(positionNoise.x * radius / 5, 0, positionNoise.x * radius / 5);

                        


                        //Size based on noise
                        float sizeScale = 25;
                        float radiusScale = (radius - minRadius) / (maxRadius - minRadius);

                        float sizeNoise = Mathf.PerlinNoise(newRock.transform.position.x * sizeScale, newRock.transform.position.z * sizeScale);

                        newRock.transform.localScale = new Vector3(rockSize/2, rockSize/2, rockSize/2);
                        newRock.transform.localScale *= 2f + (sizeNoise * 5 + (Mathf.Pow(radiusScale, 3f) * 100)) / 2;


                        //Rotate based on noise
                        float rotScale = 100;
                        float rot = (float)NoiseS3D.Noise(spawnPos.x * rotScale, spawnPos.y * rotScale, spawnPos.z * rotScale) * 360;
                        newRock.transform.Rotate(90, rot, 0); //Rotate to align with Unity XYZ system

                        


                        newRock.SetActive(true);




                    }
                }
            }else
            {

                //Create Outer Rock

                GameObject newRock = null;
                //Instantiate new if pool isn't full
                if (TunnelPool.instance.outerRocks.Count < TunnelPool.instance.num_outerRocks)
                {
                    //Make new rock
                    newRock = Instantiate(rocks[rockNum], spawnPos, Quaternion.identity);
                    //Set inactive
                    newRock.SetActive(false);
                    //Add to list
                    TunnelPool.instance.outerRocks.Add(newRock);
                }
                else //Otherwise grab from pool
                {
                    //Grab last object in list
                    newRock = TunnelPool.instance.outerRocks[0];

                    //Reorder object in list
                    TunnelPool.instance.outerRocks.Remove(newRock);
                    TunnelPool.instance.outerRocks.Add(newRock);

                    //Set inactive
                    newRock.SetActive(false);

                    //Reset position
                    newRock.transform.position = spawnPos;
                    newRock.transform.rotation = Quaternion.identity;
                }

                if (newRock != null)
                {

                    //Flatten around tunnel
                    newRock.transform.LookAt(transform.position);

                    //3D Movement Noise
                    Vector3 positionNoise = Perlin3D(newRock.transform.position * warpScale) * warpAmplitude;
                    newRock.transform.position += positionNoise * radius / 20;


                    //Rotate based on noise
                    float rotScale = 15;
                    float rot = (float)NoiseS3D.Noise(spawnPos.x * rotScale, spawnPos.y * rotScale, spawnPos.z * rotScale) * 360;
                    newRock.transform.Rotate(Vector3.forward * rot, Space.Self);

                    //Size based on noise
                    float sizeScale = 15;
                    float radiusScale = (radius - minRadius) / (maxRadius - minRadius);

                    float sizeNoise = Mathf.PerlinNoise(newRock.transform.position.x * sizeScale, newRock.transform.position.z * sizeScale);

                    newRock.transform.localScale = new Vector3(rockSize, rockSize, rockSize);
                    newRock.transform.localScale *= 1.2f + (sizeNoise * 5 + (Mathf.Pow(radiusScale, 3f) * 100)) / 2;




                    newRock.SetActive(true);




                }
            }


        }



        detailMaker.GetComponent<DetailScript>().spawnDetails();
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

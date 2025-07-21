using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DetailScript : MonoBehaviour
{
    [Header("Detail Objects and Settings")]
    [SerializeField] private GameObject[] detailObjects;
    [SerializeField] private GameObject[] stalactites;
    [SerializeField] private GameObject[] crystals;
    [SerializeField] private GameObject skylight;


    public void spawnDetails()
    {
        int steps = 50;
        float stepAngle = 360 / steps;

        for (int i = 0; i < steps; i++)
        {
            Vector3 direction = Quaternion.AngleAxis(stepAngle * i, transform.forward) * transform.up;


            RaycastHit hit = new RaycastHit();
            Ray ray = new Ray(transform.position, direction);

            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.tag == "Rock")
            {
                float darknessScale = 0.0007f;
                float darknessNoise = (float)NoiseS3D.Noise(hit.point.x * darknessScale, hit.point.y * darknessScale, hit.point.z * darknessScale);

                //Trees

                /*
                float treeScale = 0.001f;
                float treeNoise = Mathf.PerlinNoise(hit.point.x * treeScale, hit.point.z * treeScale);
                treeNoise *= Mathf.PerlinNoise(hit.point.x * treeScale * 10, hit.point.z * treeScale * 10);

                if(treeNoise < 0.1f && hit.distance > 200)
                {
                    GameObject newTree = Instantiate(detailObjects[0], hit.point, Quaternion.Euler(hit.normal));
                    newTree.transform.rotation = Quaternion.FromToRotation(newTree.transform.up, hit.normal) * newTree.transform.rotation;
                    newTree.transform.Rotate(-90, 0, 0);

                    float treeSize = (treeNoise * 5 + 0.1f) * 0.07f * hit.distance + 5;
                    newTree.transform.localScale = new Vector3(treeSize, treeSize, treeSize);

                    Debug.Log(hit.distance);
                }
                */

                //Stalactites

                float stalactiteScale = 0.0005f;
                float stalactiteNoise = Mathf.PerlinNoise(hit.point.x * stalactiteScale, hit.point.z * stalactiteScale);
                if (stalactiteNoise > 0.6f && Vector3.Angle(hit.normal, -Vector3.up) < 30 && hit.point.z > transform.position.z && hit.distance > 200)
                {
                    #region Stalactites

                    //Randomize which stalactite to spawn
                    float stalactiteSeed_scale = 10000;
                    float stalactiteSeed = Mathf.Abs((float)NoiseS3D.Noise(hit.point.x * stalactiteSeed_scale, hit.point.y * stalactiteSeed_scale, hit.point.z * stalactiteSeed_scale));
                    int stalactiteNum = (int)Mathf.Floor(stalactiteSeed * (stalactites.Length - 1));


                    GameObject newStalactite = null;
                    if (TunnelPool.instance.stalactites.Count < TunnelPool.instance.num_stalactites)
                    {
                        newStalactite = Instantiate(stalactites[stalactiteNum], hit.point, Quaternion.identity);
                        TunnelPool.instance.stalactites.Add(newStalactite);
                        newStalactite.SetActive(false);

                    }
                    else
                    {
                        newStalactite = TunnelPool.instance.stalactites[0];
                        newStalactite.SetActive(false);

                        //Reorder
                        TunnelPool.instance.stalactites.Remove(newStalactite);
                        TunnelPool.instance.stalactites.Add(newStalactite);

                        newStalactite.transform.position = hit.point;
                        newStalactite.transform.rotation = Quaternion.identity;
                    }

                    float rotNoise = (float)NoiseS3D.Noise(hit.point.x * 1000, hit.point.y * 1000, hit.point.z * 1000) * 360;
                    newStalactite.transform.Rotate(0, rotNoise, 90);

                    float stalactiteSize = (stalactiteNoise * 2) * 8;
                    newStalactite.transform.localScale = new Vector3(stalactiteSize, stalactiteSize, stalactiteSize);

                    newStalactite.SetActive(true);


                    #endregion
                }
                else
                {

                    #region Crystals

                    float crystalScale = 0.005f;
                    float crystalNoise = (float)NoiseS3D.Noise(hit.point.x * crystalScale, hit.point.y * crystalScale, hit.point.z * crystalScale) + 1f;
                    crystalNoise *= (float)NoiseS3D.Noise(hit.point.x * (crystalScale*3), hit.point.y * (crystalScale * 3), hit.point.z * (crystalScale * 3)) + 1f;
                    //Add sparsness for darkness
                    if (crystalNoise < 0.05f && darknessNoise > 0.5f)
                    {

                        GameObject newCrystal = null;

                        if (TunnelPool.instance.crystals.Count < TunnelPool.instance.num_crystals)
                        {
                            newCrystal = Instantiate(crystals[0], hit.point, Quaternion.identity);
                            TunnelPool.instance.crystals.Add(newCrystal);
                            newCrystal.SetActive(false);
                        }
                        else
                        {
                            newCrystal = TunnelPool.instance.crystals[0];
                            newCrystal.SetActive(false);

                            TunnelPool.instance.crystals.Remove(newCrystal);
                            TunnelPool.instance.crystals.Add(newCrystal);

                            newCrystal.transform.position = hit.point;
                        }

                        float sizeScale = 0.0002f;
                        float sizeNoise = 3 + (crystalNoise + 1f) * 5 + (float)NoiseS3D.Noise(hit.point.x * sizeScale, hit.point.y * sizeScale, hit.point.z * sizeScale) * 5;

                        newCrystal.transform.localScale = new Vector3(sizeNoise, sizeNoise, sizeNoise);

                        float rotNoise = (float)NoiseS3D.Noise(hit.point.x * 1000, hit.point.y * 1000, hit.point.z * 1000) * 360;
                        newCrystal.transform.rotation = Quaternion.FromToRotation(newCrystal.transform.up, hit.normal) * newCrystal.transform.rotation;
                        newCrystal.transform.Rotate(-90, rotNoise, 0);


                        newCrystal.SetActive(true);




                    }
                    #endregion


                    #region Skylights

                    float skylightScale = 0.001f;
                    float skylightNoise = Mathf.PerlinNoise(hit.point.x * skylightScale, hit.point.z * skylightScale);

                    if (skylightNoise > 0.8f && Vector3.Angle(hit.normal, -Vector3.up) < 30 && hit.point.z > transform.position.z && hit.distance > 1000)
                    {

                        if (darknessNoise < 0f)
                        {

                            GameObject newSkylight = null;

                            if (TunnelPool.instance.skylights.Count < TunnelPool.instance.num_skylights)
                            {
                                newSkylight = Instantiate(skylight, hit.point, Quaternion.identity);
                                TunnelPool.instance.skylights.Add(newSkylight);
                                newSkylight.SetActive(false);
                            }
                            else
                            {
                                newSkylight = TunnelPool.instance.skylights[0];
                                newSkylight.SetActive(false);

                                TunnelPool.instance.skylights.Remove(newSkylight);
                                TunnelPool.instance.skylights.Add(newSkylight);

                                newSkylight.transform.position = hit.point;
                            }



                            newSkylight.transform.rotation = Quaternion.FromToRotation(newSkylight.transform.up, hit.normal) * newSkylight.transform.rotation;
                            newSkylight.transform.Rotate(-90, 0, 0);
                            newSkylight.SetActive(true);

                        }
                    }

                    #endregion

                }
            }
        }
    }
}

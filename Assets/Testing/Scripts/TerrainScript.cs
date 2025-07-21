using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainScript : MonoBehaviour
{
    [SerializeField] private Terrain t;
    [SerializeField] private GameObject Segment;
    private TerrainData tData;

    private void Awake()
    {
        tData = t.terrainData;
    }
    private void Start()
    {
        EditTerrain();
    }


    void EditTerrain()
    {
        int heightmapWidth = tData.heightmapResolution;
        int heightmapHeight = tData.heightmapResolution;
        float[,] heights = tData.GetHeights(0,0,heightmapWidth,heightmapHeight);

        for(int z = 0;z < heightmapHeight;z++)
        {
            for(int x = 0;x < heightmapWidth;x++)
            {

                //Create Semicircle
                float scale = (float)Mathf.Abs(x - heightmapWidth / 2) / (float)(heightmapWidth / 2);
                heights[x, z] = 1 - Mathf.Sqrt(1 - Mathf.Pow(scale, 2));

                //Add Noise
                float noiseX = (float)x / 1000;
                float noiseY = (float)z / 1000;
                heights[x, z] += Mathf.PerlinNoise(transform.position.x * 100, transform.position.z * 100);
 
            }
        }

        tData.SetHeights(0, 0, heights);


        

    }
}

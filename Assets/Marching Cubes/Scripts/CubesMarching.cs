using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CubesMarching : MonoBehaviour
{

    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;


    [SerializeField] private float heightThreshold = 0.5f;

    [SerializeField] private float noiseResolution = 1f;

    [SerializeField] private bool visualizeNoise;

    private float[,,] heights;

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();

    private MeshFilter meshFilter;
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        StartCoroutine(UpdateAll());
    }

    private IEnumerator UpdateAll()
    {
        while(true)
        {
            SetHeights();
            SetMesh();
            MarchCubes();
            yield return new WaitForSeconds(1);
        }
    }

    private void MarchCubes()
    {
        vertices.Clear();
        triangles.Clear();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < width; z++)
                {
                    float[] cubeCorners = new float[8];

                    for(int i = 0; i < 8; i++)
                    {
                        Vector3Int corner = new Vector3Int(x, y, z) + MarchingTable.Corners[i];
                        cubeCorners[i] = heights[corner.x, corner.y, corner.z];
                    }

                    MarchCube(new Vector3(x,y,z), GetConfigurationIndex(cubeCorners));
                }
            }
        }
    }

    private void MarchCube(Vector3 position, int configIndex)
    {
        if(configIndex == 0 || configIndex == 255) {
            return;
        }

        int edgeIndex = 0;

        for(int t = 0; t < 5; t++)
        {
            for(int v = 0; v < 3; v++)
            {
                int triTableValue = MarchingTable.Triangles[configIndex, edgeIndex];

                if (triTableValue == -1)
                {
                    return;
                }

                Vector3 edgeStart = position + MarchingTable.Edges[triTableValue, 0];
                Vector3 edgeEnd = position + MarchingTable.Edges[triTableValue, 1];

                Vector3 vertex = (edgeStart + edgeEnd) / 2;

                vertices.Add(vertex);
                triangles.Add(vertices.Count - 1);

                edgeIndex++;
            }
        }
    }
    private int GetConfigurationIndex(float[] cubeCorners)
    {
        int configIndex = 0;

        for(int i = 0; i < 8; i++)
        {
            if (cubeCorners[i] > heightThreshold)
            {
                configIndex |= 1 << i;
            }
        }

        return configIndex;
    }
    private void SetMesh()
    {
        Mesh mesh = new Mesh();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }
    private void SetHeights()
    {
        heights = new float[width + 1, height  + 1, width + 1];

        for(int x = 0; x < width + 1; x++)
        {
            for(int y = 0; y < height + 1; y++)
            {
                for(int z = 0; z < width + 1; z++)
                {
                    float currentHeight = height * Mathf.PerlinNoise(x * noiseResolution, z * noiseResolution);
                    float newHeight;

                    if(y > currentHeight)
                    {
                        newHeight = y - currentHeight;
                    }
                    else
                    {
                        newHeight = currentHeight - y;
                    }

                    heights[x, y, z] = newHeight;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if(!visualizeNoise || !Application.isPlaying)
        {
            return;
        }

        for(int x = 0;x < width + 1; x++)
        {
            for(int y = 0;y < height + 1; y++)
            {
                for(int z = 0;z < width + 1; z++)
                {
                    Gizmos.color = new Color(heights[x,y,z], heights[x,y,z], heights[x, y, z], 1);
                    Gizmos.DrawSphere(new Vector3(x, y, z), 0.2f);
                }
            }
        }
    }
}

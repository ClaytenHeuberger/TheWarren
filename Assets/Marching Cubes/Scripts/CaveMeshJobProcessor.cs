using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CaveMeshJobProcessor
{

    public static GeneratedMeshData GenerateMesh(Vector3 currentPos, int chunkSize, float noiseScale, float threshold)
    {

        GeneratedMeshData data = new GeneratedMeshData();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        (vertices, triangles) = MarchCubes(threshold, chunkSize, SetHeights(chunkSize, currentPos, noiseScale));

        data.position = Vector3Int.FloorToInt(currentPos);
        data.vertices = vertices.ToArray();
        data.triangles = triangles.ToArray();

        return data;
    }
    public static int GetConfigIndex(float[] cubeCorners, float heightThreshold)
    {
        int configIndex = 0;

        for (int i = 0; i < 8; i++)
        {
            if (cubeCorners[i] > heightThreshold)
            {
                configIndex |= 1 << i;
            }
        }

        return configIndex;
    }

    public static float[,,] SetHeights(int chunkSize, Vector3 currentPos, float noiseScale)
    {
        float[,,] heights = new float[chunkSize + 1, chunkSize + 1, chunkSize + 1];

        for (int x = 0; x < chunkSize + 1; x++)
        {
            for (int y = 0; y < chunkSize + 1; y++)
            {
                for (int z = 0; z < chunkSize + 1; z++)
                {
                    float currentHeight = GetCurrentHeight(x + currentPos.x, y + currentPos.y, z + currentPos.z, chunkSize, noiseScale);

                    heights[x, y, z] = currentHeight;

                }
            }
        }

        return heights;
    }

    private static (List<Vector3>, List<int>) MarchCubes(float heightThreshold, int chunkSize, float[,,] heights)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    float[] cubeCorners = new float[8];

                    for (int i = 0; i < 8; i++)
                    {
                        Vector3Int corner = new Vector3Int(x, y, z) + MarchingTable.Corners[i];
                        cubeCorners[i] = heights[corner.x, corner.y, corner.z];
                    }

                    MarchCube(new Vector3(x, y, z), cubeCorners, heightThreshold, vertices, triangles);
                }
            }
        }

        return (vertices, triangles);
    }

    private static void MarchCube(Vector3 position, float[] cubeCorners, float heightThreshold, List<Vector3> vertices, List<int> triangles)
    {
        int configIndex = GetConfigIndex(cubeCorners, heightThreshold);

        if (configIndex == 0 || configIndex == 255)
        {
            return;
        }

        int edgeIndex = 0;
        for (int t = 0; t < 5; t++)
        {
            for (int v = 0; v < 3; v++)
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

    private static float GetCurrentHeight(float x, float y, float z, float chunkSize, float noiseScale)
    {

        float noise = PerlinNoise3D((float)(x) / chunkSize * noiseScale, (float)(y) / chunkSize * noiseScale, (float)(z) / chunkSize * noiseScale);
        noise += PerlinNoise3D((float)(x) / chunkSize * (noiseScale / 1.5f), (float)(y) / chunkSize * (noiseScale / 1.5f), (float)(z) / chunkSize * (noiseScale / 1.5f));
        noise += PerlinNoise3D((float)(x) / chunkSize * (noiseScale * 1.2f), (float)(y) / chunkSize * (noiseScale * 1.2f), (float)(z) / chunkSize * (noiseScale * 1.2f));
        noise /= 3;



        return noise;
    }
    private static float PerlinNoise3D(float x, float y, float z)
    {
        float xy = Mathf.PerlinNoise(x, y);
        float xz = Mathf.PerlinNoise(x, z);
        float yz = Mathf.PerlinNoise(y, z);
        float yx = Mathf.PerlinNoise(y, x);
        float zx = Mathf.PerlinNoise(z, x);
        float zy = Mathf.PerlinNoise(z, y);
        return (xy + xz + yz + yx + zx + zy) / 6f;
    }
}

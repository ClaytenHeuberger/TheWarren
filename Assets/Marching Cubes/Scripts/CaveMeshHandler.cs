using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveMeshHandler : MonoBehaviour
{
    public static List<GameObject> meshObjects = new List<GameObject>();
    public static List<CaveMeshGenerator> generators = new List<CaveMeshGenerator>();
    public static List<Vector3> positions = new List<Vector3>();
    public static List<Vector3> offsets = new List<Vector3>();
    public static List<Vector3> playerActivePositions = new List<Vector3>();
    public static Vector3 playerStartPos;
    public static GameObject meshObject;

    
    public static void Initialize()
    {
        int numMeshObjects = 0;
        for (int i = 0;i < generators.Count;i++)
        {
            numMeshObjects += (int)(generators[i].offsets.Count * 2f);
        }

        for (int i = 0;i < numMeshObjects;i++)
        {
            GameObject newMesh = Instantiate(meshObject, Vector3.zero, Quaternion.identity);
            meshObjects.Add(newMesh);
            newMesh.SetActive(false);
        }
    }
    public static void UnloadChunks()
    {
        for (int i = 0; i < meshObjects.Count;i++)
        {

            if (meshObjects[i].activeSelf)
            {
                CheckMesh(generators, meshObjects[i]);

            }
        }

    }

    public static float GetCurrentHeight(Vector3 position)
    {
        float noiseMod = 0f;

        
        if (Vector3.Distance(position, playerStartPos) < 20)
        {
            return 1;
        }
        
        // Clear area for outposts
        float radius = 20f;
        for (int i = 0;i < Outposts.positions.Count; i++)
        {
            if(Vector3.Distance(position, Outposts.positions[i]) < radius)
            {
                return 1;
            }
        }

        float noiseScale = CaveMeshSettings.noiseScale;
        float chunkSize = CaveMeshSettings.chunkSize;

        float noise = PerlinNoise3D(position.x / chunkSize * noiseScale, position.y / chunkSize * noiseScale, position.z / chunkSize * noiseScale);
        noise += PerlinNoise3D(position.x / chunkSize * (noiseScale / 1.5f), position.y / chunkSize * (noiseScale / 1.5f), position.z / chunkSize * (noiseScale / 1.5f));
        noise += PerlinNoise3D(position.x / chunkSize * (noiseScale * 1.2f), position.y / chunkSize * (noiseScale * 1.2f), position.z / chunkSize * (noiseScale * 1.2f));
        noise /= 3;
        noise += noiseMod;


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

        return (xy + xz + yz + yx + zx + zy) / 6;
    }

    public static GameObject GetMeshObject()
    {
        for(int i = 0; i <  meshObjects.Count; i++)
        {
            if (!meshObjects[i].activeSelf)
            {
                return meshObjects[i];
            }
        }
        return null;
    } 
    private static void CheckMesh(List<CaveMeshGenerator> generators, GameObject mesh)
    {
        for(int i = 0;i < generators.Count;i++)
        {
            Vector3Int objectPosition = Vector3Int.FloorToInt(mesh.transform.position);
            Vector3 objectToV3 = objectPosition;

            Vector3 targetPos = generators[i].player.transform.position + generators[i].player.GetComponent<Rigidbody>().velocity;
            if (Vector3.Distance(objectToV3, targetPos) <= (generators[i].loadingChunkDistance + 1) * CaveMeshSettings.chunkSize)
            {
                return;
            }
        }
        positions.Remove(mesh.transform.position);
        mesh.SetActive(false);
    }
}

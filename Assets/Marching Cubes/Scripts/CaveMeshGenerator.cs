using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TB;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Concurrent;
using System.Threading;
using Unity.VisualScripting;
using Unity.Jobs;
using Unity.Burst;
using System.ComponentModel;
using JetBrains.Annotations;
public struct GeneratedMeshData
{
    public Vector3Int position;
    public Vector3[] vertices;
    public int[] triangles;
}

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CaveMeshGenerator : MonoBehaviour
{

    [SerializeField] private bool isMainPlayer;
    public int loadingChunkDistance = 5;
    [SerializeField] private GameObject meshObject;
    public GameObject player;

    //[SerializeField] private Transform target;

    [SerializeField] float gizmosResolution = 1;

    [SerializeField] bool visualizeNoise;
    [SerializeField] bool use3DNoise;

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private float[,,] heights;

    private MeshFilter meshFilter;

    Vector3 currentPos = Vector3.zero;

    [HideInInspector]
    public List<Vector3> offsets = new List<Vector3>();



    int chunkIndex = 0;
    Vector3Int currentPlayerChunkPosition;


    //private ConcurrentQueue<GeneratedMeshData> meshQueue = new();

    private void Awake()
    {
        CaveMeshHandler.generators.Add(this);

        CaveMeshHandler.meshObject = meshObject;

        if (isMainPlayer)
            CaveMeshHandler.playerStartPos = player.transform.position;

        UpdatePlayerPos();

    }
    void Start()
    {

        if (isMainPlayer)
            CaveMeshHandler.Initialize();

        meshFilter = GetComponent<MeshFilter>();

        /*
        for(int i = 0;i < offsets.Count;i++)
        {
            currentPos = offsets[meshIndex] + GetPlayerChunkPosition() * chunkSize;

            SetHeights();
            MarchCubes();
            SetMesh();
            CaveMeshHandler.positions.Add(currentPos);

            meshIndex++;
        }
        meshIndex = 0;
        */
    }


    private void Update()
    {


        bool found = false;
        chunkIndex = 0;
        while (chunkIndex < offsets.Count - 1)
        {     
            currentPos = offsets[chunkIndex] + GetPlayerChunkPosition() * CaveMeshSettings.chunkSize;
            if (!CaveMeshHandler.positions.Contains(currentPos))
            {
                found = true;
                CaveMeshHandler.positions.Add(currentPos);
                break;
            }   
            chunkIndex++;
        }

        if (found)
        {
            SetHeights();
            MarchCubes();
            SetMesh();
        }

        // Unload out of bounds chunks when player moves
        if (GetPlayerChunkPosition() != currentPlayerChunkPosition && Time.frameCount > 10)
        {
            //UpdatePlayerPos();
            CaveMeshHandler.UnloadChunks();
        }

        currentPlayerChunkPosition = GetPlayerChunkPosition();

    }


    private void SetMesh()
    {

        GameObject currentMesh = CaveMeshHandler.GetMeshObject();
        currentMesh.transform.position = currentPos;

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateUVDistributionMetrics();
        mesh.RecalculateNormals();
        currentMesh.GetComponent<MeshFilter>().mesh = mesh;
        currentMesh.GetComponent<MeshCollider>().sharedMesh = mesh;

        currentMesh.SetActive(true);

    }

    private void SetHeights()
    {
        heights = new float[CaveMeshSettings.chunkSize + 1, CaveMeshSettings.chunkSize + 1, CaveMeshSettings.chunkSize + 1];

        for (int x = 0; x < CaveMeshSettings.chunkSize + 1; x++)
        {
            for (int y = 0; y < CaveMeshSettings.chunkSize + 1; y++)
            {
                for (int z = 0; z < CaveMeshSettings.chunkSize + 1; z++)
                {
                    if (use3DNoise)
                    {
                        Vector3 pos = new Vector3(x + currentPos.x, y + currentPos.y, z + currentPos.z);
                        float height = CaveMeshHandler.GetCurrentHeight(pos);

                        if(isMainPlayer)
                            CaveDetailTools.AddDetails(pos, height);

                        heights[x, y, z] = height;
                    }
                }
            }
        }

    }


    private int GetConfigIndex(float[] cubeCorners)
    {
        int configIndex = 0;

        for (int i = 0; i < 8; i++)
        {
            if (cubeCorners[i] > CaveMeshSettings.heightThreshold)
            {
                configIndex |= 1 << i;
            }
        }

        return configIndex;
    }

    private void MarchCubes()
    {
        vertices.Clear();
        triangles.Clear();

        for (int x = 0; x < CaveMeshSettings.chunkSize; x++)
        {
            for (int y = 0; y < CaveMeshSettings.chunkSize; y++)
            {
                for (int z = 0; z < CaveMeshSettings.chunkSize; z++)
                {
                    float[] cubeCorners = new float[8];

                    for (int i = 0; i < 8; i++)
                    {
                        Vector3Int offset = MarchingTable.Corners[i];
                        cubeCorners[i] = heights[x + offset.x, y + offset.y, z + offset.z];
                    }

                    MarchCube(new Vector3(x, y, z), cubeCorners);
                }
            }
        }
    }

    private void MarchCube(Vector3 position, float[] cubeCorners)
    {
        int configIndex = GetConfigIndex(cubeCorners);

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

    private void OnDrawGizmosSelected()
    {
        /*
        for(int i = 0;i < offsets.Count;i++)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(offsets[i], 1f);
        }
        for (int i = 0; i < meshObjects.Count; i++)
        {
            Vector3Int objectPosition = Vector3Int.FloorToInt(meshObjects[i].transform.position);
            Vector3 objectToV3 = objectPosition;


            if (!offsets.Contains(objectToV3))
            {

                Gizmos.color = Color.red;
                Gizmos.DrawSphere(objectToV3, 2f);
            }

        }
        */


        if (!visualizeNoise || !Application.isPlaying)
        {
            return;
        }

        for (int x = 0; x < CaveMeshSettings.chunkSize + 1; x++)
        {
            for (int y = 0; y < CaveMeshSettings.chunkSize + 1; y++)
            {
                for (int z = 0; z < CaveMeshSettings.chunkSize + 1; z++)
                {
                    Gizmos.color = new Color(heights[x, y, z], heights[x, y, z], heights[x, y, z], 1);
                    Gizmos.DrawSphere(new Vector3(x * gizmosResolution, y * gizmosResolution, z * gizmosResolution), 0.2f * gizmosResolution);
                }
            }
        }

    }

    private void UpdatePlayerPos()
    {
        Vector3Int playerChunkPos = GetPlayerChunkPosition();

        offsets.Clear();
        // 1. Collect all chunk offsets in the cube
        for (int x = -loadingChunkDistance; x <= loadingChunkDistance; x++)
        {
            for (int y = -loadingChunkDistance; y <= loadingChunkDistance; y++)
            {
                for (int z = -loadingChunkDistance; z <= loadingChunkDistance; z++)
                {
                    offsets.Add(new Vector3Int(x, y, z));

                }
            }
        }
        // 2. Sort them by distance to the center (0, 0, 0)
        offsets.Sort((a, b) => a.sqrMagnitude.CompareTo(b.sqrMagnitude));

        float offsetsStartCount = offsets.Count;
        for(int i = 0; i < offsets.Count; i++)
        {
            offsets[i] = offsets[i] * CaveMeshSettings.chunkSize;

            // The ratio of a sphere to a cube, deletes the outer shell to leave just the sphere
            if(i / offsetsStartCount > Mathf.PI / 6)
            {
                offsets.Remove(offsets[i]);
            }
        }

    }
    
    private Vector3Int GetPlayerChunkPosition()
    {
        Vector3 playerPos = player.transform.position + player.GetComponent<Rigidbody>().velocity;
        return new Vector3Int(
            Mathf.FloorToInt(playerPos.x / CaveMeshSettings.chunkSize),
            Mathf.FloorToInt(playerPos.y / CaveMeshSettings.chunkSize),
            Mathf.FloorToInt(playerPos.z / CaveMeshSettings.chunkSize)
        );
    }

    


}

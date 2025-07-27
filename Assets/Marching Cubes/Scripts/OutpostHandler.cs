using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;


public static class Outposts
{
    public static List<Vector3> positions = new List<Vector3>();
    public static float radius = 40f;
    public static GameObject outpostObject;

}

public static class GradientDescent
{
    
    public static Vector3 FindLocalMax(Vector3 startPos, float scale, int stepSize, int maxSteps)
    {
        Vector3 currentPos = startPos;

        for (int i = 0; i < maxSteps; i++)
        {

            float maxNoise = 0f;
            Vector3 bestDir = Vector3.zero;


            for (int j = 0; j < 6; j++)
            {

                float noise = -1f;
                Vector3 dir = Vector3.zero;
                switch (j)
                {
                    case 0:
                        noise = PerlinNoise3D((currentPos + new Vector3(stepSize, 0, 0)) * scale);
                        dir = new Vector3(stepSize, 0, 0);
                        break;
                    case 1:
                        noise = PerlinNoise3D((currentPos + new Vector3(-stepSize, 0, 0)) * scale);
                        dir = new Vector3(-stepSize, 0, 0);
                        break;
                    case 2:
                        noise = PerlinNoise3D((currentPos + new Vector3(0, stepSize, 0)) * scale);
                        dir = new Vector3(0, stepSize, 0);
                        break;
                    case 3:
                        noise = PerlinNoise3D((currentPos + new Vector3(0, -stepSize, 0)) * scale);
                        dir = new Vector3(0, -stepSize, 0);
                        break;
                    case 4:
                        noise = PerlinNoise3D((currentPos + new Vector3(0, 0, stepSize)) * scale);
                        dir = new Vector3(0, 0, stepSize);
                        break;
                    case 5:
                        noise = PerlinNoise3D(currentPos + new Vector3(0, 0, -stepSize));
                        dir = new Vector3(0, 0, -stepSize);
                        break;
                }

                if (noise > maxNoise)
                {
                    maxNoise = noise;
                    bestDir = dir;

                }
            }


            currentPos += bestDir;

            if (bestDir == Vector3.zero)
                return currentPos;

        }

        return currentPos;
    }

    private static float PerlinNoise3D(Vector3 pos)
    {
        float xy = Mathf.PerlinNoise(pos.x, pos.y);
        float xz = Mathf.PerlinNoise(pos.x, pos.z);
        float yz = Mathf.PerlinNoise(pos.y, pos.z);

        float yx = Mathf.PerlinNoise(pos.y, pos.x);
        float zx = Mathf.PerlinNoise(pos.z, pos.x);
        float zy = Mathf.PerlinNoise(pos.z, pos.y);

        return (xy + xz + yz + yx + zx + zy) / 6;
    }
}
public class OutpostHandler : MonoBehaviour
{
    [SerializeField] private Transform debug;
    [SerializeField] private GameObject outpostObject;
    List<Vector3> positions;
    void Start()
    {
        positions = new List<Vector3>();

        Vector3 outpost = GradientDescent.FindLocalMax(CaveMeshHandler.playerStartPos, 0.01f, 10, 500);
        Outposts.positions.Add(outpost);
        GameObject newOutpost = Instantiate(outpostObject, outpost, Quaternion.identity);
        newOutpost.transform.localScale = Vector3.one * Outposts.radius;
        Outposts.outpostObject = newOutpost;
        debug.position = outpost;
    }

    private void Update()
    {
        for(int i = 0;i < positions.Count-1;i++)
        {
            Debug.DrawLine(positions[i], positions[i + 1]);
        }
    }

}

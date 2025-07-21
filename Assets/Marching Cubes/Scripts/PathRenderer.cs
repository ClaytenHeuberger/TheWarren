using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
public class PathRenderer : MonoBehaviour
{
    public ThreadedPathfinder pathfinder;
    public Transform start;
    public Transform end;


    [SerializeField] private SplineContainer container;

    Vector3 startPos;
    private void Start()
    {
        StartCoroutine(ConstructPath());
    }

    
    IEnumerator ConstructPath()
    {
        int counter = 0;
        while(true)
        {
            pathfinder.FindPathAsync(Vector3Int.FloorToInt(startPos), Vector3Int.FloorToInt(end.position), OnPathFound);

            counter++;
            if(counter >= 2)
            {
                counter = 0;
                startPos = start.position;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    void OnPathFound(List<Vector3Int> path)
    {
        if (path == null)
        {
            //Debug.Log("No path found.");
            return;
        }
        container.Spline.Clear();
        int numKnots = 0;
        for (int i = 0;i < path.Count-1; i+=5)
        {
            BezierKnot knot = new BezierKnot();
            float3 knotPos = new float3(path[i].x, path[i].y, path[i].z);
            knot.Position = knotPos;

            container.Spline.Add(knot);
            container.Spline.SetTangentMode(numKnots, TangentMode.AutoSmooth);
            numKnots++;

        }



    }

}

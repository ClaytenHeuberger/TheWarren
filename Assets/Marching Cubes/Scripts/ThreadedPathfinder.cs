using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ThreadedPathfinder : MonoBehaviour
{
    public int maxSearchDistance = 100;

    public void FindPathAsync(Vector3Int start, Vector3Int end, Action<List<Vector3Int>> callback)
    {
        Task.Run(() =>
        {
            var path = FindPath(start, end);
            UnityMainThreadDispatcher.Enqueue(() => callback?.Invoke(path));
        });
    }

    private List<Vector3Int> FindPath(Vector3Int start, Vector3Int end, float maxDistance = 50f)
    {
        MinHeap openSet = new MinHeap();
        HashSet<Vector3Int> closedSet = new();
        Dictionary<Vector3Int, Vector3Int> cameFrom = new();
        Dictionary<Vector3Int, float> gScore = new();

        Vector3Int closest = start;
        float closestDist = Heuristic(start, end);

        openSet.Push(start, 0f);
        gScore[start] = 0f;

        int iterations = 0;
        int maxIterations = 20000;

        while (openSet.Count > 0 && iterations < maxIterations)
        {
            iterations++;

            Vector3Int current = openSet.Pop();
            float distToTarget = Heuristic(current, end);

            // Update closest node
            if (distToTarget < closestDist)
            {
                closest = current;
                closestDist = distToTarget;
            }

            // Stop if too far
            if ((current - start).sqrMagnitude > maxDistance * maxDistance)
                break;

            // Success condition
            if (current == end)
                return ReconstructPath(cameFrom, current);

            closedSet.Add(current);

            foreach (Vector3Int dir in Directions)
            {
                Vector3Int neighbor = current + dir;
                if (closedSet.Contains(neighbor)) continue;
                if (CaveMeshHandler.GetCurrentHeight(neighbor) < CaveMeshSettings.heightThreshold) continue;

                float tentativeG = gScore[current] + 1f;

                if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    float f = tentativeG + Heuristic(neighbor, end);
                    openSet.Push(neighbor, f);
                }
            }
        }

        // Couldn't reach end — return partial path to closest known point
        return closest != start ? ReconstructPath(cameFrom, closest) : null;
    }

    private List<Vector3Int> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int current)
    {
        List<Vector3Int> path = new() { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Add(current);
        }
        path.Reverse();
        return path;
    }

    private float Heuristic(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
    }

    private static readonly Vector3Int[] Directions = {
        new(1, 0, 0), new(-1, 0, 0),
        new(0, 1, 0), new(0, -1, 0),
        new(0, 0, 1), new(0, 0, -1)
    };
}
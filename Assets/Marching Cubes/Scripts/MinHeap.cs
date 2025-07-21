using System.Collections.Generic;
using UnityEngine;

public class MinHeap
{
    private List<(Vector3Int node, float cost)> heap = new();

    public int Count => heap.Count;

    public void Push(Vector3Int node, float cost)
    {
        heap.Add((node, cost));
        int i = heap.Count - 1;
        while (i > 0 && heap[i].cost < heap[(i - 1) / 2].cost)
        {
            (heap[i], heap[(i - 1) / 2]) = (heap[(i - 1) / 2], heap[i]);
            i = (i - 1) / 2;
        }
    }

    public Vector3Int Pop()
    {
        var result = heap[0].node;
        heap[0] = heap[heap.Count - 1];
        heap.RemoveAt(heap.Count - 1);
        Heapify(0);
        return result;
    }

    private void Heapify(int i)
    {
        int left = 2 * i + 1;
        int right = 2 * i + 2;
        int smallest = i;

        if (left < heap.Count && heap[left].cost < heap[smallest].cost)
            smallest = left;
        if (right < heap.Count && heap[right].cost < heap[smallest].cost)
            smallest = right;

        if (smallest != i)
        {
            (heap[i], heap[smallest]) = (heap[smallest], heap[i]);
            Heapify(smallest);
        }
    }
}
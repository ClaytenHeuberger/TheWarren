using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelPool : MonoBehaviour
{
    public static TunnelPool instance;

    public List<GameObject> outerRocks = new List<GameObject>();
    public int num_outerRocks = 10000;
    [SerializeField] private GameObject[] outerRocksPrefabs;

    public List<GameObject> floorRocks = new List<GameObject>();
    public int num_floorRocks = 5000;
    [SerializeField] private GameObject floorRocksPrefab;

    public List<GameObject> stalactites = new List<GameObject>();
    public int num_stalactites = 1000;
    [SerializeField] private GameObject[] stalactitesPrefabs;

    public List<GameObject> crystals = new List<GameObject>();
    public int num_crystals = 1000;
    [SerializeField] private GameObject[] crystalPrefab;


    public List<GameObject> skylights = new List<GameObject>();
    public int num_skylights = 1000;
    [SerializeField] private GameObject[] skylightPrefab;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {

    }

    public GameObject GetOuterRock()
    {
        for (int i = 0; i < outerRocks.Count; i++)
        {
            if (!outerRocks[i].gameObject.activeInHierarchy)
            {
                return outerRocks[i];
            }
        }

        return null;
    }
}

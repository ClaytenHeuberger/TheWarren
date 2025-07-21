using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Biome
{
    public string name;
    public Color caveColor;
    public Color fogColor;
    public float rarity = 1f;
    public ParticleSystem ambientParticle;
    public GameObject oreObject;
    [HideInInspector]
    public List<GameObject> oreObjects;


    public GameObject GetOre()
    {
        for(int i = 0; i < oreObjects.Count; i++)
        {
            if (!oreObjects[i].activeSelf)
                return oreObjects[i];
        }

        return null;
    }
}
public class CaveDetailHandler : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float biomeScale = 0.005f;
    [SerializeField] private float easeRate = 0.02f;
    [SerializeField] private Material caveMat;
    [SerializeField] private int oresPerBiome = 20;
    public float oreThreshold = 0.05f;
    public Biome[] biomes;

    Gradient biomeGradient;
    Biome currentBiome;
    Biome previousBiome;
    Camera cam;
    void Start()
    {
        cam = Camera.main;

        // Set up biome gradient //

        biomeGradient = new Gradient();

        float raritySum = 0f;
        for(int i = 0;i < biomes.Length;i++)
        {
            raritySum += biomes[i].rarity;
        }

        int numKeys = biomes.Length * 2 - 2;
        int keyIndex = 0;
        var keys = new GradientColorKey[numKeys];

        float time = 0f;
        for (int i = 1; i < biomes.Length; i++)
        {
            time += biomes[i-1].rarity / raritySum;

            keys[keyIndex].time = time - (easeRate / 2);
            keys[keyIndex].color = biomes[i - 1].caveColor;
            keyIndex++;
            keys[keyIndex].time = time + (easeRate / 2);
            keys[keyIndex].color = biomes[i].caveColor;
            keyIndex++;


            for(int j = 0; j < oresPerBiome; j++)
            {
                GameObject newOre = Instantiate(biomes[i].oreObject);
                newOre.SetActive(false);
                newOre.transform.parent = transform;
                biomes[i].oreObjects.Add(newOre);
            }
        }


        var alphas = new GradientAlphaKey[0]; //No alpha
        biomeGradient.SetKeys(keys, alphas);

        float noise = Noise.get3DPerlinNoise(player.position, biomeScale) + 0.5f;

        GetBiomeByNoise(noise).ambientParticle.Play();
    }
    private void Update()
    {
        float noise = Noise.get3DPerlinNoise(player.position, biomeScale) + 0.5f;

        Color col = biomeGradient.Evaluate(noise);
        caveMat.SetColor("_MainColor", col);
        RenderSettings.fogColor = col;
        cam.backgroundColor = col;


        if(GetBiomeByNoise(noise) != null)
            currentBiome = GetBiomeByNoise(noise);

        if (previousBiome != null && currentBiome != previousBiome)
        {
            previousBiome.ambientParticle.Stop();
            currentBiome.ambientParticle.Play();

        }

        if (GetBiomeByNoise(noise) != null)
            previousBiome = GetBiomeByNoise(noise);


    }

    private Biome GetBiomeByNoise(float noise)
    {
        Color matchingColor = biomeGradient.Evaluate(noise);

        for(int i = 0;i < biomes.Length; i++)
        {
            if (biomes[i].caveColor == matchingColor)
            {
                return biomes[i];

            }
        }

        return null;
    }

    public void AddDetails(Vector3 position, float height)
    {
        /*
        float oreNoise = PerlinNoise3D(position * 50f);
        if(Mathf.Abs(height - CaveMeshSettings.heightThreshold) < 0.02f && oreNoise < 0.1f)
            GameObject ore = 
        */
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

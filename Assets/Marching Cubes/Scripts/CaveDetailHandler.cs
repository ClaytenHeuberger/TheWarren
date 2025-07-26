using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Biome
{
    public string _name;
    public Color caveColor; //Note: All colors have to have an alpha of '1', or it throws errors
    public Color fogColor;
    public float rarity = 1f;
    public ParticleSystem ambientParticle;
    public GameObject oreObject;
    [HideInInspector]
    public Queue<GameObject> oreObjects = new Queue<GameObject>();
}

public static class CaveDetailTools
{

    public static Gradient biomeGradient;
    public static Biome[] biomes;
    public static float biomeScale;
    public static float oreScale;
    public static float pillarScale;
    public static float oreThreshold;
    public static float pillarThreshold;
    public static Biome currentBiome;
    public static Biome GetBiomeByPosition(Vector3 position)
    {
        float noise = GetBiomeNoise(position);
        Color matchingColor = biomeGradient.Evaluate(noise);

        for (int i = 0; i < biomes.Length; i++)
        {
            if (biomes[i].caveColor == matchingColor)
            {
                return biomes[i];

            }
        }

        return null;
    }

    public static void AddDetails(Vector3 position, float height)
    {
        currentBiome = GetBiomeByPosition(position);
        if (currentBiome != null)
        {
            // ORE //
            float oreNoise = GetOreNoise(position);
            if (Mathf.Abs(height - CaveMeshSettings.heightThreshold) < 0.02f && oreNoise > oreThreshold)
            {
                GameObject ore = currentBiome.oreObjects.Dequeue();

                if (ore != null)
                {
                    currentBiome.oreObjects.Enqueue(ore);
                    ore.SetActive(false);
                    ore.transform.position = position;
                    ore.transform.rotation = Quaternion.Euler(new Vector3(height * 10000f, oreNoise * 10000f, height * oreNoise * 10000f));
                    ore.transform.localScale = Vector3.one * (1 - (oreNoise - oreThreshold));
                    ore.SetActive(true);
                }
            }

            // PILLARS //

            float pillarNoise = GetPillarNoise(position);
            if (height > 0.9f && pillarNoise > pillarThreshold)
            {

            }
        }

    }

    public static float GetBiomeNoise(Vector3 position)
    {
        return Noise.get3DPerlinNoise(position, biomeScale) + 0.5f;
    }
    public static float GetOreNoise(Vector3 position)
    {
        return Noise.get3DPerlinNoise(position, oreScale) + 0.5f;
    }

    public static float GetPillarNoise(Vector3 position)
    {
        return Noise.get3DPerlinNoise(position, pillarScale) + 0.5f;
    }

}
public class CaveDetailHandler : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Material caveMat;

    [Header("Biomes")]
    [SerializeField] private float biomeScale = 0.005f;
    [SerializeField] private float easeRate = 0.02f;
    public Biome[] biomes;

    [Header("Ores")]
    [SerializeField] private int oresPerBiome = 20;
    [SerializeField] private float oreScale = 5.15329f;
    [SerializeField] private float oreThreshold = 20.15329f;

    [Header("Pillars")]
    [SerializeField] private float pillarScale = 300.15213f;
    [SerializeField] private float pillarThreshold = 1.33f;


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

        }

        for(int i = 0;i < biomes.Length;i++)
        {
            for (int j = 0; j < oresPerBiome; j++)
            {
                GameObject newOre = Instantiate(biomes[i].oreObject);
                newOre.SetActive(false);
                newOre.transform.parent = transform;
                biomes[i].oreObjects.Enqueue(newOre);
            }
        }


        var alphas = new GradientAlphaKey[0]; //No alpha
        biomeGradient.SetKeys(keys, alphas);

        float noise = CaveDetailTools.GetBiomeNoise(player.position);

        #region tooDumbToLook
        CaveDetailTools.biomeGradient = biomeGradient;
        CaveDetailTools.biomes = biomes;
        CaveDetailTools.biomeScale = biomeScale;
        CaveDetailTools.oreScale = oreScale;
        CaveDetailTools.oreThreshold = oreThreshold;
        CaveDetailTools.pillarScale = pillarScale;
        CaveDetailTools.pillarThreshold = pillarThreshold;
        #endregion


        CaveDetailTools.GetBiomeByPosition(player.position).ambientParticle.Play();
    }
    private void Update()
    {
        float noise = CaveDetailTools.GetBiomeNoise(player.position);

        Color col = biomeGradient.Evaluate(noise);
        caveMat.SetColor("_MainColor", col);
        RenderSettings.fogColor = col;
        cam.backgroundColor = col;


        if(CaveDetailTools.GetBiomeByPosition(player.position) != null)
            currentBiome = CaveDetailTools.GetBiomeByPosition(player.position);

        if (previousBiome != null && currentBiome != previousBiome)
        {
            previousBiome.ambientParticle.Stop();
            currentBiome.ambientParticle.Play();

        }

        if (CaveDetailTools.GetBiomeByPosition(player.position) != null)
            previousBiome = CaveDetailTools.GetBiomeByPosition(player.position);

    }

    






}

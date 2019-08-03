using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantManager : MonoBehaviour
{
    [SerializeField] private Plant Plant;

    [SerializeField] private List<GameObject> LeafPrefabOptions;
    [SerializeField] private List<GameObject> FlowerPrefabOptions;
    [SerializeField] private List<Texture2D> StemTextureOptions;

    [SerializeField] private Vector3 FlowerOriginOffset;

    void Start()
    {
        // TODO Call this when we get seed
        CreatePlant(1);
    }

    private void CreatePlant(int seed)
    {
        Plant.Setup(GetFlowerPrefab(seed), GetFlowerHue(seed), GetLeafPrefab(seed), GetStemTexture(seed), Plant.transform.position + FlowerOriginOffset);
    }

    private float GetFlowerHue(int seed)
    {
        //TODO Implement
        return 0f;
    }

    private GameObject GetLeafPrefab(int seed)
    {
        if (LeafPrefabOptions.Count == 0)
        {
            return null;
        }

        int index = (seed * 13) % LeafPrefabOptions.Count;
        return LeafPrefabOptions[index];
    }

    private GameObject GetFlowerPrefab(int seed)
    {
        if (FlowerPrefabOptions.Count == 0)
        {
            return null;
        }

        int index = (seed * 17) % FlowerPrefabOptions.Count;
        return FlowerPrefabOptions[index];
    }

    private Texture2D GetStemTexture(int seed)
    {
        if (StemTextureOptions.Count == 0)
        {
            return null;
        }

        int index = (seed * 19) % StemTextureOptions.Count;
        return StemTextureOptions[index];
    }
}

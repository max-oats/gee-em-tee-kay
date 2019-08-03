using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> LeafPrefabOptions;
    [SerializeField] private List<GameObject> FlowerPrefabOptions;
    [SerializeField] private List<Texture2D> StemTextureOptions;

    [SerializeField] private Transform FlowerOriginPos;

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

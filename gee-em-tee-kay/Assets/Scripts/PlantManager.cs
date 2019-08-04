using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantManager : MonoBehaviour
{
    [SerializeField] private Plant Plant;

    [SerializeField] private GameObject[] LeafPrefabOptions;
    [SerializeField] private GameObject[] FlowerPrefabOptions;
    [SerializeField] private Texture2D[] StemTextureOptions;

    [SerializeField] private Vector3 FlowerOriginOffset;

    [SerializeField] private string PlantName;
    private bool HasStarted = false;

    void Update()
    {
        //TODO Remove Debug code
        if (HasStarted)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            CreatePlant(PlantName.GetHashCode());
        }
    }

    private void CreatePlant(int seed)
    {
        Debug.Log("Creating Plant");
        Plant.Setup(GetFlowerPrefab(seed), GetFlowerHue(seed), GetLeafPrefab(seed), GetStemTexture(seed), Plant.transform.position + FlowerOriginOffset);
    }

    private float GetFlowerHue(int seed)
    {
        //TODO Implement
        return 0f;
    }

    private GameObject GetLeafPrefab(int seed)
    {
        if (LeafPrefabOptions.Length == 0)
        {
            return null;
        }

        int index = (seed * 13) % LeafPrefabOptions.Length;
        return LeafPrefabOptions[index];
    }

    private GameObject GetFlowerPrefab(int seed)
    {
        if (FlowerPrefabOptions.Length == 0)
        {
            return null;
        }

        int index = (seed * 17) % FlowerPrefabOptions.Length;
        return FlowerPrefabOptions[index];
    }

    private Texture2D GetStemTexture(int seed)
    {
        if (StemTextureOptions.Length == 0)
        {
            return null;
        }

        int index = (seed * 19) % StemTextureOptions.Length;
        return StemTextureOptions[index];
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantManager : MonoBehaviour
{
    [SerializeField] private Plant Plant;
    [SerializeField] private PlantPot PlantPot;

    [SerializeField] private GameObject[] LeafPrefabOptions;
    [SerializeField] private GameObject[] FlowerPrefabOptions;

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
            Debug.Log("Seed: " + PlantName);
            CreatePlant(PlantName.GetHashCode());
        }
    }

    private void CreatePlant(int seed)
    {
        Debug.Log("Creating Plant with seed " + seed);
        Vector3 FlowerOriginOffset = new Vector3(0,PlantPot.bottomOfPlantOffset,0);
        Plant.Setup(GetFlowerPrefab(seed), GetFlowerHue(seed), GetLeafPrefab(seed), GetStemColour(seed), Plant.transform.position + FlowerOriginOffset);
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

        int index = Mathf.Abs(seed * 13) % LeafPrefabOptions.Length;
        return LeafPrefabOptions[index];
    }

    private GameObject GetFlowerPrefab(int seed)
    {
        if (FlowerPrefabOptions.Length == 0)
        {
            return null;
        }

        int index = Mathf.Abs(seed * 17) % FlowerPrefabOptions.Length;
        return FlowerPrefabOptions[index];
    }

    private Color GetStemColour(int seed)
    {
        return new Color(0,1,0);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantManager : MonoBehaviour
{
    [SerializeField] private Plant plant;
    [SerializeField] private PlantPot plantPot;

    [SerializeField] private GameObject[] leafPrefabOptions;
    [SerializeField] private GameObject[] flowerPrefabOptions;

    [SerializeField] private float minStemRed;
    [SerializeField] private float maxStemRed;
    [SerializeField] private float minStemGreen;
    [SerializeField] private float maxStemGreen;
    [SerializeField] private float minStemBlue;
    [SerializeField] private float maxStemBlue;

    [SerializeField] private float initialFlowerColorSat;
    [SerializeField] private float initialFlowerColorVal;

    public void CreatePlant(int seed)
    {
        Random.InitState(seed);
        Vector3 flowerOriginOffset = new Vector3(0,plantPot.GetBottomOfPlantOffset(),0);
        plant.Setup(seed, GetFlowerPrefab(seed), GetFlowerColorHue(), initialFlowerColorSat, initialFlowerColorVal, GetLeafPrefab(seed), GetStemColour(seed), plant.transform.position + flowerOriginOffset);
    }

    private float GetFlowerColorHue()
    {
        // TODO Restrict green maybe?
        return Random.value;
    }

    private GameObject GetLeafPrefab(int seed)
    {
        if (leafPrefabOptions.Length == 0)
        {
            return null;
        }

        int index = Random.Range(0, leafPrefabOptions.Length);
        return leafPrefabOptions[index];
    }

    private GameObject GetFlowerPrefab(int seed)
    {
        if (flowerPrefabOptions.Length == 0)
        {
            return null;
        }

        int index = Random.Range(0, flowerPrefabOptions.Length);
        return flowerPrefabOptions[index];
    }

    private Color GetStemColour(int seed)
    {
        float red = Mathf.Lerp(minStemRed, maxStemRed, Random.value);
        float green = Mathf.Lerp(minStemGreen, maxStemGreen, Random.value);
        float blue = Mathf.Lerp(minStemBlue, maxStemBlue, Random.value);

        return new Color(red,green,blue);
    }
}

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

    [SerializeField] private float MinStemRed;
    [SerializeField] private float MaxStemRed;
    [SerializeField] private float MinStemGreen;
    [SerializeField] private float MaxStemGreen;
    [SerializeField] private float MinStemBlue;
    [SerializeField] private float MaxStemBlue;

    public void CreatePlant(int seed)
    {
        Vector3 FlowerOriginOffset = new Vector3(0,PlantPot.GetBottomOfPlantOffset(),0);
        Plant.Setup(seed, GetFlowerPrefab(seed), GetFlowerHue(seed), GetLeafPrefab(seed), GetStemColour(seed), Plant.transform.position + FlowerOriginOffset);
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
        float RedAlpha = (Mathf.Abs(seed * 7) / 64f) % 1;
        float Red = Mathf.Lerp(MinStemRed, MaxStemRed, RedAlpha);

        float GreenAlpha = (Mathf.Abs(seed * 11) / 64f) % 1;
        float Green = Mathf.Lerp(MinStemGreen, MaxStemGreen, GreenAlpha);

        float BlueAlpha = (Mathf.Abs(seed * 13) / 64f) % 1;
        float Blue = Mathf.Lerp(MinStemBlue, MaxStemBlue, BlueAlpha);

        Debug.Log(string.Format("Selected Color: {0}, {1}, {2}", Red, Green, Blue));
        return new Color(Red,Green,Blue);
    }
}

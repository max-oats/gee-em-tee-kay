using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayManager : MonoBehaviour
{
    [SerializeField] private Plant Plant;
    [SerializeField] private PlantPot PlantPot;
    
    [SerializeField] private int TotalNumDays;
    [SerializeField] private int DaysWithoutWaterToBeThirsty;
    [SerializeField] private int DaysWithWaterToBeDrowning;
    [SerializeField] private int TooMuchLightThreshold;
    [SerializeField] private int NotEnoughLightThreshold;
    [SerializeField] private int DaysConversedForMineOption;
    [SerializeField] private int DaysConversedForThirdOption;

    [SerializeField] private int InitialHealth;
    [SerializeField] private int HealthPenaltyForWater;
    [SerializeField] private int HealthPenaltyForLight;
    [SerializeField] private int HealthRewardForGoodDay;
    [SerializeField] private int HealthRewardForTalkingToday;

    [SerializeField] private List<LightSettings> lightSettings = new List<LightSettings>();
    [SerializeField] private Light ambientLight;
    [SerializeField] private Light sunLight;
    
    private int currentDay = 0;
    private PlantHealthPersistentData persistentData;
    private PlantHealthTransientData transientData;

    void Start()
    {
        persistentData = new PlantHealthPersistentData();
        persistentData.GeneralHealth = InitialHealth;
        transientData = new PlantHealthTransientData();
    }

    void Update()
    {
        // TODO Remove Debug code
        if (Input.GetKeyDown(KeyCode.Q))
        {
            EndDay();
        }
    }

    public void StartDay()
    {
        // Update lighting settings
        ambientLight.color = lightSettings[currentDay].ambientLight; 
        ambientLight.intensity = lightSettings[currentDay].ambientLightIntensity;

        sunLight.color = lightSettings[currentDay].sunlight; 
        sunLight.shadowStrength = lightSettings[currentDay].shadowStrength;

        Global.cameraController.SetBackgroundColour(lightSettings[currentDay].skyboxColour);
    }

    public void EndDay()
    {
        Debug.Log("Ending Day");

        if (transientData.WasWateredToday)
        {
            persistentData.DaysWateredStreak++;
            persistentData.DaysNotWateredStreak = 0;
        }
        else
        {
            persistentData.DaysWateredStreak = 0;
            persistentData.DaysNotWateredStreak++;
        }

        persistentData.AccumulatedLight += transientData.LightGettingToday;

        bool HasBeenDamaged = false;
        if (IsThirsty() || IsDrowning())
        {
            persistentData.GeneralHealth -= HealthPenaltyForWater;
            HasBeenDamaged = true;
        }
        if (HasTooMuchLight() || HasNotEnoughLight())
        {
            persistentData.GeneralHealth -= HealthPenaltyForLight;
            HasBeenDamaged = true;
        }

        if (!HasBeenDamaged)
        {
            persistentData.GeneralHealth += HealthRewardForGoodDay;
        }

        if (transientData.HaveConversedToday)
        {
            persistentData.DaysConversed++;
            persistentData.GeneralHealth += HealthRewardForTalkingToday;
        }

        Plant.AddSectionsForDay();

        transientData = new PlantHealthTransientData();
    }

    // Water
    public void Water()
    {
        transientData.WasWateredToday = true;
    }

    public bool HasBeenWateredToday()
    {
        return transientData.WasWateredToday;
    }

    public bool IsThirsty()
    {
        return persistentData.DaysNotWateredStreak > DaysWithoutWaterToBeThirsty;
    }

    public bool IsDrowning()
    {
        return persistentData.DaysWateredStreak > DaysWithWaterToBeDrowning;
    }

    // Light
    public void SetLightIncrementForToday(int lightIncrement)
    {
        transientData.LightGettingToday = lightIncrement;
    }

    public bool HasTooMuchLight()
    {
        return persistentData.AccumulatedLight > TooMuchLightThreshold;
    }

    public bool HasNotEnoughLight()
    {
        return persistentData.AccumulatedLight < NotEnoughLightThreshold;
    }


    // Talking
    public void Talk()
    {
        FindObjectOfType<Yarn.Unity.DialogueRunner>().StartDialogue(SelectNode());
        transientData.HaveConversedToday = true;
    }

    public string SelectNode()
    {
        return "Day1.Talk";
    }

    public bool HasEverConversed()
    {
        return persistentData.DaysConversed > 0;
    }

    public bool HasMineOption()
    {
        return persistentData.DaysConversed >= DaysConversedForMineOption;
    }

    public bool HasThirdOption()
    {
        return persistentData.DaysConversed >= DaysConversedForThirdOption;
    }


    // Dimensions
    public float GetMaxHeightOfSection()
    {
        return PlantPot.height / TotalNumDays;
    }

    public float GetMaxDistanceFromPotCenter()
    {
        return PlantPot.distanceFromCenter;
    }
}

public class PlantHealthPersistentData
{
    public int DaysWateredStreak = 0;
    public int DaysNotWateredStreak = 0;
    public int AccumulatedLight = 0;
    public int GeneralHealth;
    public int DaysConversed = 0;
}

public class PlantHealthTransientData
{
    public bool WasWateredToday = false;
    public int LightGettingToday = 0;
    public bool HaveConversedToday = false;
}

[System.Serializable]
public class LightSettings
{
    public Color ambientLight;
    public Color sunlight;

    public Color skyboxColour;

    [Range(0.0f, 1.0f)]
    public float shadowStrength;

    [Range(0.0f, 2.0f)]
    public float ambientLightIntensity;
}
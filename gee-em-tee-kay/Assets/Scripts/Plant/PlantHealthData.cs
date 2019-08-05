using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantHealthData : MonoBehaviour
{
    [SerializeField] private Plant Plant;
    [SerializeField] private PlantPot PlantPot;

    [SerializeField] private int DaysWithoutWaterToBeThirsty;
    [SerializeField] private int DaysWithWaterToBeDrowning;
    [SerializeField] private int TooMuchLightThreshold;
    [SerializeField] private int NotEnoughLightThreshold;
    [SerializeField] private int DaysConversedForMineOption;
    [SerializeField] private int DaysConversedForThirdOption;

    [SerializeField] private int MaxHealth;
    [SerializeField] private int HealthThresholdToLookUnhealthy;
    [SerializeField] private int InitialHealth;
    [SerializeField] private int HealthPenaltyForWater;
    [SerializeField] private int HealthPenaltyForLight;
    [SerializeField] private int HealthRewardForGoodDay;
    [SerializeField] private int HealthRewardForTalkingToday;

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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ReviewDay();
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            Talk();
        }
    }

    public void ReviewDay()
    {
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

        persistentData.GeneralHealth = Mathf.Clamp(persistentData.GeneralHealth, 0, MaxHealth);

        Plant.AddSectionsForDay();

        persistentData.DayNumber++;

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
        Debug.Log("Debug Talking!");
        transientData.HaveConversedToday = true;
    }

    public string SelectDialogueNode()
    {
        return "Day" + (persistentData.DaysConversed+1) +  ".Talk";
    }

    public bool HaveConversedToday()
    {
        return transientData.HaveConversedToday;
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


    // Health
    public bool PlantIsDead()
    {
        return persistentData.GeneralHealth < 0;
    }

    public bool PlantIsUnhealthy()
    {
        return persistentData.GeneralHealth < HealthThresholdToLookUnhealthy;
    }

    public float CurrentHealthPercentage()
    {
        Debug.Log((float)persistentData.GeneralHealth);
        Debug.Log(MaxHealth);
        Debug.Log((float)persistentData.GeneralHealth / MaxHealth);
        return (float)persistentData.GeneralHealth / MaxHealth;
    }


    // Dimensions
    public float GetMaxHeightOfSection()
    {
        return PlantPot.GetPlantHeightLimit() / Global.dayManager.GetTotalNumDays();
    }

    public float GetMaxDistanceFromPotCenter()
    {
        return PlantPot.GetDistanceFromCenter();
    }
}

public class PlantHealthPersistentData
{
    public int DaysWateredStreak = 0;
    public int DaysNotWateredStreak = 0;
    public int AccumulatedLight = 0;
    public int GeneralHealth;
    public int DaysConversed = 0;
    public int DayNumber = 0;
}

public class PlantHealthTransientData
{
    public bool WasWateredToday = false;
    public int LightGettingToday = 0;
    public bool HaveConversedToday = false;
}

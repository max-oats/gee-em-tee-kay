using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantHealthData : MonoBehaviour
{
    [SerializeField] private Plant plant;
    [SerializeField] private PlantPot plantPot;

    [SerializeField] private int daysWithoutWaterToBeThirsty;
    [SerializeField] private int daysWithWaterToBeDrowning;
    [SerializeField] private int tooMuchLightThreshold;
    [SerializeField] private int notEnoughLightThreshold;
    [SerializeField] private int daysConversedForMineOption;
    [SerializeField] private int daysConversedForThirdOption;

    [SerializeField] private int maxHealth;
    [SerializeField] private int healthThresholdToLookUnhealthy;
    [SerializeField] private int initialHealth;
    [SerializeField] private int healthPenaltyForWater;
    [SerializeField] private int healthPenaltyForLight;
    [SerializeField] private int healthRewardForGoodDay;
    [SerializeField] private int healthRewardForTalkingToday;

    private PlantHealthPersistentData persistentData;
    private PlantHealthTransientData transientData;

    void Start()
    {
        persistentData = new PlantHealthPersistentData();
        persistentData.GeneralHealth = initialHealth;
        transientData = new PlantHealthTransientData();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ReviewDay();
        }
        /*else if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Debug Talking!");
            Talk();
        }*/
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

        bool hasBeenDamaged = false;
        if (IsThirsty() || IsDrowning())
        {
            persistentData.GeneralHealth -= healthPenaltyForWater;
            hasBeenDamaged = true;
        }
        if (HasTooMuchLight() || HasNotEnoughLight())
        {
            persistentData.GeneralHealth -= healthPenaltyForLight;
            hasBeenDamaged = true;
        }

        if (!hasBeenDamaged)
        {
            persistentData.GeneralHealth += healthRewardForGoodDay;
        }

        if (transientData.HaveConversedToday)
        {
            persistentData.DaysConversed++;
            persistentData.GeneralHealth += healthRewardForTalkingToday;
        }

        persistentData.GeneralHealth = Mathf.Clamp(persistentData.GeneralHealth, 0, maxHealth);

        plant.AddSectionsForDay();

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
        return persistentData.DaysNotWateredStreak > daysWithoutWaterToBeThirsty;
    }

    public bool IsDrowning()
    {
        return persistentData.DaysWateredStreak > daysWithWaterToBeDrowning;
    }

    // Light
    public void SetLightIncrementForToday(int lightIncrement)
    {
        transientData.LightGettingToday = lightIncrement;
    }

    public bool HasTooMuchLight()
    {
        return persistentData.AccumulatedLight > tooMuchLightThreshold;
    }

    public bool HasNotEnoughLight()
    {
        return persistentData.AccumulatedLight < notEnoughLightThreshold;
    }


    // Talking
    public void Talk()
    {
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
        return persistentData.DaysConversed >= daysConversedForMineOption;
    }

    public bool HasThirdOption()
    {
        return persistentData.DaysConversed >= daysConversedForThirdOption;
    }


    // Health
    public bool PlantIsDead()
    {
        return persistentData.GeneralHealth < 0;
    }

    public bool PlantIsUnhealthy()
    {
        return persistentData.GeneralHealth < healthThresholdToLookUnhealthy;
    }

    public float CurrentHealthPercentage()
    {
        return (float)persistentData.GeneralHealth / maxHealth;
    }


    // Dimensions
    public float GetMaxHeightOfSection()
    {
        return plantPot.GetPlantHeightLimit() / Global.dayManager.GetTotalNumDays();
    }

    public float GetMaxDistanceFromPotCenter()
    {
        return plantPot.GetDistanceFromCenter();
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

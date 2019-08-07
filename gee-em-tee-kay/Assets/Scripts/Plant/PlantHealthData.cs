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
        persistentData.generalHealth = initialHealth;
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
        if (transientData.wasWateredToday)
        {
            persistentData.daysWateredStreak++;
            persistentData.daysNotWateredStreak = 0;
        }
        else
        {
            persistentData.daysWateredStreak = 0;
            persistentData.daysNotWateredStreak++;
        }

        persistentData.accumulatedLight += transientData.lightGettingToday;

        bool hasBeenDamaged = false;
        if (IsThirsty() || IsDrowning())
        {
            persistentData.generalHealth -= healthPenaltyForWater;
            hasBeenDamaged = true;
        }
        if (HasTooMuchLight() || HasNotEnoughLight())
        {
            persistentData.generalHealth -= healthPenaltyForLight;
            hasBeenDamaged = true;
        }

        if (!hasBeenDamaged)
        {
            persistentData.generalHealth += healthRewardForGoodDay;
        }

        if (transientData.haveConversedToday)
        {
            persistentData.daysConversed++;
            persistentData.generalHealth += healthRewardForTalkingToday;
        }

        persistentData.generalHealth = Mathf.Clamp(persistentData.generalHealth, 0, maxHealth);

        plant.AddSectionsForDay();

        transientData = new PlantHealthTransientData();
    }



    // Water
    public void Water()
    {
        transientData.wasWateredToday = true;
    }

    public bool HasBeenWateredToday()
    {
        return transientData.wasWateredToday;
    }

    public bool IsThirsty()
    {
        return persistentData.daysNotWateredStreak > daysWithoutWaterToBeThirsty;
    }

    public bool IsDrowning()
    {
        return persistentData.daysWateredStreak > daysWithWaterToBeDrowning;
    }

    // Light
    public void SetLightIncrementForToday(int lightIncrement)
    {
        transientData.lightGettingToday = lightIncrement;
    }

    public bool HasTooMuchLight()
    {
        return persistentData.accumulatedLight > tooMuchLightThreshold;
    }

    public bool HasNotEnoughLight()
    {
        return persistentData.accumulatedLight < notEnoughLightThreshold;
    }


    // Talking
    public void Talk()
    {
        transientData.haveConversedToday = true;
    }

    public string SelectDialogueNode()
    {
        return "Day" + (persistentData.daysConversed+1) +  ".Talk";
    }

    public bool HaveConversedToday()
    {
        return transientData.haveConversedToday;
    }

    public bool HasEverConversed()
    {
        return persistentData.daysConversed > 0;
    }

    public bool HasMineOption()
    {
        return persistentData.daysConversed >= daysConversedForMineOption;
    }

    public bool HasThirdOption()
    {
        return persistentData.daysConversed >= daysConversedForThirdOption;
    }


    // Health
    public bool PlantIsDead()
    {
        return persistentData.generalHealth < 0;
    }

    public bool PlantIsUnhealthy()
    {
        return persistentData.generalHealth < healthThresholdToLookUnhealthy;
    }

    public float CurrentHealthPercentage()
    {
        return (float)persistentData.generalHealth / maxHealth;
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
    public int daysWateredStreak = 0;
    public int daysNotWateredStreak = 0;
    public int accumulatedLight = 0;
    public int generalHealth;
    public int daysConversed = 0;
}

public class PlantHealthTransientData
{
    public bool wasWateredToday = false;
    public int lightGettingToday = 0;
    public bool haveConversedToday = false;
}

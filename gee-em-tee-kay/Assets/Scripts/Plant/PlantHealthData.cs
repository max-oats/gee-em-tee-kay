using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantHealthData : MonoBehaviour
{
    [SerializeField] private Plant plant;
    [SerializeField] private PlantPot plantPot;

    [SerializeField] private int tooMuchWaterThreshold;
    [SerializeField] private int notEnoughWaterThreshold;
    [SerializeField] private int waterLostPerDay;
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
        if (Global.debug.keyQGrowsPlant && Input.GetKeyDown(KeyCode.Q))
        {
            ReviewDay(false);
        }
    }

    public void ReviewDay(bool shouldSpawnFlower)
    {
        if (transientData.waterGettingToday > 0)
        {
            persistentData.accumulatedWater += transientData.waterGettingToday;
        }
        else
        {
            persistentData.accumulatedWater -= waterLostPerDay;
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

        persistentData.generalHealth = Mathf.Min(persistentData.generalHealth, maxHealth);

        plant.AddSectionsForDay(shouldSpawnFlower);

        transientData = new PlantHealthTransientData();
    }



    // Water
    public void Water()
    {
        transientData.waterGettingToday++;
    }

    public bool IsThirsty()
    {
        return persistentData.accumulatedWater < notEnoughWaterThreshold;
    }

    public bool IsDrowning()
    {
        return persistentData.accumulatedWater > tooMuchWaterThreshold;
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
        int dayNumber = persistentData.daysConversed+1;
        int debugDayNumber = Global.debug.dayToPlayDialogueFor_1Indexed;
        if (debugDayNumber >=1 && debugDayNumber <= 5)
        {
            dayNumber = debugDayNumber;
            Global.debug.dayToPlayDialogueFor_1Indexed = -1;
        }

        return "Day" + dayNumber +  ".Talk";
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
    public int accumulatedWater = 0;
    public int accumulatedLight = 0;
    public int generalHealth;
    public int daysConversed = 0;
}

public class PlantHealthTransientData
{
    public int waterGettingToday = 0;
    public int lightGettingToday = 0;
    public bool haveConversedToday = false;
}

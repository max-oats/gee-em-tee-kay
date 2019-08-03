using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayManager : MonoBehaviour
{
    [SerializeField] private int TotalNumDays;
    [SerializeField] private int DaysWithoutWaterToBeThirsty;
    [SerializeField] private int DaysWithWaterToBeDrowning;
    [SerializeField] private int TooMuchLightThreshold;
    [SerializeField] private int NotEnoughLightThreshold;

    [SerializeField] private int InitialHealth;
    [SerializeField] private int HealthPenaltyForWater;
    [SerializeField] private int HealthPenaltyForLight;
    [SerializeField] private int HealthRewardForGoodDay;

    private PlantHealthPersistentData persistentData;
    private PlantHealthTransientData transientData;

    void Start()
    {
        persistentData = new PlantHealthPersistentData();
        persistentData.GeneralHealth = InitialHealth;
        transientData = new PlantHealthTransientData();
    }

    public void EndDay()
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

        transientData = new PlantHealthTransientData();
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

    public bool HasTooMuchLight()
    {
        return persistentData.AccumulatedLight > TooMuchLightThreshold;
    }

    public bool HasNotEnoughLight()
    {
        return persistentData.AccumulatedLight < NotEnoughLightThreshold;
    }
}

public class PlantHealthPersistentData
{
    public int DaysWateredStreak = 0;
    public int DaysNotWateredStreak = 0;
    public int AccumulatedLight = 0;
    public int GeneralHealth;
}

public class PlantHealthTransientData
{
    public bool WasWateredToday = false;
    public int LightGettingToday = 0;
}

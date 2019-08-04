using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayManager : MonoBehaviour
{
    public delegate void DayEnded();
    public DayEnded dayEnded;

    public bool seedPlanted = false;

    [SerializeField] private Plant Plant;
    [SerializeField] private PlantPot PlantPot;

    [SerializeField] private int TotalNumDays;
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

    [SerializeField] private List<LightSettings> lightSettings = new List<LightSettings>();
    [SerializeField] private Light ambientLight;
    [SerializeField] private Light sunLight;

    [SerializeField] private GameObject faderObject;
    [SerializeField] private GameObject rainObject;
    [SerializeField] private float fadeTime = 1.0f;
    [SerializeField] private Color fadedInColor = Color.white;
    [SerializeField] private Color fadedOutColor = Color.white;

    private int currentDay = 0;
    private PlantHealthPersistentData persistentData;
    private PlantHealthTransientData transientData;

    void Start()
    {
        persistentData = new PlantHealthPersistentData();
        persistentData.GeneralHealth = InitialHealth;
        transientData = new PlantHealthTransientData();

        StartNewDay();
    }

    void Update()
    {
        // TODO Remove Debug code
        if (Input.GetKeyDown(KeyCode.Q))
        {
            EndDay();
        }
    }

    public void StartNewDay()
    {
        StartCoroutine(StartDayFade());

        if (currentDay == 0)
        {
            FindObjectOfType<Yarn.Unity.DialogueRunner>().StartDialogue("Day1.Intro");
        }

        if (currentDay == 2)
        {
            rainObject.SetActive(true);
        }
        else
        {
            rainObject.SetActive(false);
        }

        Global.input.controllers.maps.SetMapsEnabled(true, "Movement");
    }

    public IEnumerator StartDayFade()
    {
        bool fadedOut = false;
        float timeCounter = 0f;
        while (!fadedOut)
        {
            if (timeCounter > fadeTime)
            {
                fadedOut = true;
            }
            else
            {
                timeCounter += Time.deltaTime;
            }

            faderObject.GetComponent<Renderer>().material.color = Color.Lerp(fadedOutColor, fadedInColor, timeCounter/fadeTime);

            yield return null;
        }
    }

    public void EndDay()
    {
        StartCoroutine(EndDayFade());
    }

    public IEnumerator EndDayFade()
    {
        bool fadedOut = false;
        float timeCounter = 0f;
        while (!fadedOut)
        {
            if (timeCounter > fadeTime)
            {
                fadedOut = true;
            }
            else
            {
                timeCounter += Time.deltaTime;
            }

            faderObject.GetComponent<Renderer>().material.color = Color.Lerp(fadedInColor, fadedOutColor, timeCounter/fadeTime);

            yield return null;
        }

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

        persistentData.DayNumber++;

        transientData = new PlantHealthTransientData();

        currentDay++;

        // Update lighting settings
        ambientLight.color = lightSettings[currentDay].ambientLight;
        ambientLight.intensity = lightSettings[currentDay].ambientLightIntensity;

        sunLight.color = lightSettings[currentDay].sunlight;
        sunLight.shadowStrength = lightSettings[currentDay].shadowStrength;

        Global.cameraController.SetBackgroundColour(lightSettings[currentDay].skyboxColour);

        dayEnded?.Invoke();

        yield return new WaitForSeconds(1.0f);

        StartNewDay();
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
        Debug.Log("Setting light to " + lightIncrement);
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
        string nodeName = "";
        if (IsThirsty())
        {

        }
        else if (IsDrowning())
        {

        }
        else
        {
            return "Day" + (persistentData.DaysConversed+1) +  ".Talk";
        }

        return nodeName;
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
        return (float)persistentData.GeneralHealth / MaxHealth;
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


    // Progression
    public int GetPlantProgressionDay()
    {
        return persistentData.DayNumber;
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

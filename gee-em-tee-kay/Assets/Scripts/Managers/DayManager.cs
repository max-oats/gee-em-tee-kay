using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayManager : MonoBehaviour
{
    public delegate void DayEnded();
    public DayEnded dayEnded;

    public bool seedPlanted = false;

    // ~Begin Debug
    [SerializeField] private bool skipIntros;
    // ~End Debug

    [SerializeField] private int TotalNumDays;

    [SerializeField] private List<LightSettings> lightSettings = new List<LightSettings>();
    [SerializeField] private Light ambientLight;
    [SerializeField] private Light sunLight;

    [SerializeField] private GameObject faderObject;
    [SerializeField] private GameObject rainObject;
    [SerializeField] private float fadeTime = 1.0f;
    [SerializeField] private Color fadedInColor = Color.white;
    [SerializeField] private Color fadedOutColor = Color.white;

    public int currentDay = 0;

    void Start()
    {
        StartNewDay();
    }

    public int GetTotalNumDays()
    {
        return TotalNumDays;
    }

    public void StartNewDay()
    {
        if (currentDay == 5)
        {
            Application.Quit();
        }

        StartCoroutine(StartDayFade());

        if (currentDay == 0 && !skipIntros)
        {
            FindObjectOfType<Yarn.Unity.DialogueRunner>().StartDialogue("Day1.Intro");
        }
        else if (currentDay == 4 && !skipIntros)
        {
            FindObjectOfType<Yarn.Unity.DialogueRunner>().StartDialogue("Day5.Intro");
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

        Global.plantHealthData.ReviewDay();

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

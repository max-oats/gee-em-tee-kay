using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeatherSettings
{
    public Color ambientLight;
    public Color sunlight;

    public Color skyboxColour;

    [Range(0.0f, 1.0f)]
    public float shadowStrength;

    [Range(0.0f, 2.0f)]
    public float ambientLightIntensity;

    [Range(-180, 180)]
    public float xRotation;

    [Range(-180, 180)]
    public float yRotation;

    public bool isRaining;

    public bool isThundering;

    public bool isWindy;
}

public class WeatherManager : MonoBehaviour
{
    // ~ Begin debug
    [Range(1,5)]
    [SerializeField] private int debugDaySelector;
    // ~ End debug

    [SerializeField] private List<WeatherSettings> weatherSettings = new List<WeatherSettings>();
    [SerializeField] private Light ambientLight;
    [SerializeField] private Light sunLight;
    [SerializeField] private GameObject rainObject;
    [SerializeField] private GameObject windObject;

    void Start()
    {
        Global.dayManager.dayStarted += UpdateWeather;
    }

    void OnValidate()
    {
        UpdateWeather(debugDaySelector - 1);
    }

    void UpdateWeather(int dayNo)
    {
        // Update ambient light
        ambientLight.color = weatherSettings[dayNo].ambientLight;
        ambientLight.intensity = weatherSettings[dayNo].ambientLightIntensity;

        // Update sunlight
        sunLight.color = weatherSettings[dayNo].sunlight;
        sunLight.shadowStrength = weatherSettings[dayNo].shadowStrength;

        sunLight.transform.eulerAngles = new Vector3(weatherSettings[dayNo].xRotation, weatherSettings[dayNo].yRotation, 0.0f);

        // Update skybox if game has started
        if (Global.cameraController != null)
        {
            Global.cameraController.SetBackgroundColour(weatherSettings[dayNo].skyboxColour);
        }

        // Update weather things
        if (weatherSettings[dayNo].isRaining)
        {
            rainObject.SetActive(true);
        }
        else
        {
            rainObject.SetActive(false);
        }

        if (weatherSettings[dayNo].isWindy)
        {
            windObject.SetActive(true);
        }
        else
        {
            windObject.SetActive(false);
        }
    }
}

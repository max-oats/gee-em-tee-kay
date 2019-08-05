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
    [SerializeField] private List<WeatherSettings> weatherSettings = new List<WeatherSettings>();
    [SerializeField] private Light ambientLight;
    [SerializeField] private Light sunLight;
    [SerializeField] private GameObject rainObject;

    void Start()
    {
        Global.dayManager.dayStarted += UpdateWeather;
    }

    void UpdateWeather(int dayNo)
    {
        // Update ambient light
        ambientLight.color = weatherSettings[dayNo].ambientLight;
        ambientLight.intensity = weatherSettings[dayNo].ambientLightIntensity;

        // Update sunlight
        sunLight.color = weatherSettings[dayNo].sunlight;
        sunLight.shadowStrength = weatherSettings[dayNo].shadowStrength;

        // Update skybox
        Global.cameraController.SetBackgroundColour(weatherSettings[dayNo].skyboxColour);

        // Update weather things
        if (weatherSettings[dayNo].isRaining)
        {
            rainObject.SetActive(true);
        }
        else
        {
            rainObject.SetActive(false);
        }
    }
}

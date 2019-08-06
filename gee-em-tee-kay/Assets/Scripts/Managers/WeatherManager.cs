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

    [SerializeField] private Vector2 lightningTimeRange;
    [SerializeField] private float timeBetweenThunderAndLightning;
    [SerializeField] private float lightningLerpTime;
    [SerializeField] private List<WeatherSettings> weatherSettings = new List<WeatherSettings>();
    [SerializeField] private WeatherSettings lightningWeather;
    [SerializeField] private Light ambientLight;
    [SerializeField] private Light sunLight;
    [SerializeField] private GameObject rainObject;
    [SerializeField] private GameObject windObject;
    [SerializeField] private GameObject normalLeaves;
    [SerializeField] private GameObject windyLeaves;

    private Coroutine lightningCoroutine;

    void Start()
    {
        Global.dayManager.dayStarted += UpdateWeather;
    }

    void OnValidate()
    {
        if (debugDaySelector > 0
            && ambientLight != null
            && sunLight != null
            && rainObject != null
            && windObject != null
            && normalLeaves != null
            && windyLeaves != null)
        {
            UpdateWeather(debugDaySelector - 1);
        }
        else
        {
            Debug.Log("Invalid OnValidate in WeatherManager!");
            return;
        }
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
            windyLeaves.SetActive(true);
            normalLeaves.SetActive(false);
        }
        else
        {
            windObject.SetActive(false);
            windyLeaves.SetActive(false);
            normalLeaves.SetActive(true);
        }

        if (Global.hasStarted)
        {
            if (weatherSettings[dayNo].isThundering)
            {
                lightningCoroutine = StartCoroutine(LightningStrikes(dayNo));
            }
            else
            {
                if (lightningCoroutine != null)
                {
                    StopCoroutine(lightningCoroutine);
                }
            }
        }
    }

    IEnumerator LightningStrikes(int dayNo)
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(lightningTimeRange.x, lightningTimeRange.y));

            float timeCounter = 0f;
            while (timeCounter < lightningLerpTime)
            {
                timeCounter += Time.deltaTime;

                // Update ambient light
                ambientLight.color = Color.Lerp(weatherSettings[dayNo].ambientLight, lightningWeather.ambientLight, timeCounter/lightningLerpTime);
                ambientLight.intensity = Mathf.Lerp(weatherSettings[dayNo].ambientLightIntensity, lightningWeather.ambientLightIntensity, timeCounter/lightningLerpTime);

                // Update sunlight
                sunLight.color = Color.Lerp(weatherSettings[dayNo].sunlight, lightningWeather.sunlight, timeCounter/lightningLerpTime);
                sunLight.shadowStrength = Mathf.Lerp(weatherSettings[dayNo].shadowStrength, lightningWeather.shadowStrength, timeCounter/lightningLerpTime);

                // Update skybox if game has started
                if (Global.cameraController != null)
                {
                    Global.cameraController.SetBackgroundColour(Color.Lerp(weatherSettings[dayNo].skyboxColour, lightningWeather.skyboxColour, timeCounter/lightningLerpTime));
                }

                yield return null;
            }

            timeCounter = lightningLerpTime;

            while (timeCounter > 0)
            {
                timeCounter -= Time.deltaTime;

                // Update ambient light
                ambientLight.color = Color.Lerp(weatherSettings[dayNo].ambientLight, lightningWeather.ambientLight, timeCounter/lightningLerpTime);
                ambientLight.intensity = Mathf.Lerp(weatherSettings[dayNo].ambientLightIntensity, lightningWeather.ambientLightIntensity, timeCounter/lightningLerpTime);

                // Update sunlight
                sunLight.color = Color.Lerp(weatherSettings[dayNo].sunlight, lightningWeather.sunlight, timeCounter/lightningLerpTime);
                sunLight.shadowStrength = Mathf.Lerp(weatherSettings[dayNo].shadowStrength, lightningWeather.shadowStrength, timeCounter/lightningLerpTime);

                // Update skybox if game has started
                if (Global.cameraController != null)
                {
                    Global.cameraController.SetBackgroundColour(Color.Lerp(weatherSettings[dayNo].skyboxColour, lightningWeather.skyboxColour, timeCounter/lightningLerpTime));
                }

                yield return null;
            }

            yield return new WaitForSeconds(timeBetweenThunderAndLightning);

            // DO THUNDER STRIKE SOUND
            Global.cameraController.ScreenShake(0.1f);
            yield return new WaitForSeconds(0.2f);
            Global.cameraController.ScreenShake(0.15f);
            yield return new WaitForSeconds(0.2f);
            Global.cameraController.ScreenShake(0.2f);
            yield return new WaitForSeconds(0.2f);
            Global.cameraController.ScreenShake(0.15f);
            yield return new WaitForSeconds(0.2f);
            Global.cameraController.ScreenShake(0.1f);
        }
    }
}

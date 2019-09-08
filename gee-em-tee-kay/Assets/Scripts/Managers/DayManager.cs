using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayManager : MonoBehaviour
{
    public delegate void DayLoaded(int dayNo);
    public DayLoaded dayLoaded;

    public delegate void DayStarted(int dayNo);
    public DayStarted dayStarted;

    [HideInInspector] public bool seedPlanted = false;

    [SerializeField] private int totalNumDays;
    [SerializeField] private GameObject faderObject;
    [SerializeField] private GameObject titleObject;
    [SerializeField] private float fadeTime = 1.0f;
    [SerializeField] private Color fadedInColor = Color.white;
    [SerializeField] private Color fadedOutColor = Color.white;

    public int currentDay = 0;

    void Start()
    {
        StartCoroutine(GameStartCoroutine());
    }

    IEnumerator GameStartCoroutine()
    {
        if (Global.debug.skipTitleScreen)
        {
            titleObject.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 0f);
            StartNewDay();
            yield break;
        }

        yield return new WaitForSeconds(3.0f);

        while (!Global.input.GetButtonDown("Start"))
        {
            yield return null;
        }

        titleObject.GetComponent<Animator>().CrossFadeInFixedTime("TitleBack", 0f);

        yield return new WaitForSeconds(10.0f);

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

            titleObject.GetComponent<Renderer>().material.color = Color.Lerp(Color.white, new Color(1f, 1f, 1f, 0f), timeCounter/fadeTime);

            yield return null;
        }

        StartNewDay();
    }

    public int GetTotalNumDays()
    {
        return totalNumDays;
    }

    public void StartNewDay(bool fadeIn = true)
    {
        // Invoke delegate
        dayLoaded?.Invoke(currentDay);

        if (fadeIn)
        {
            StartCoroutine(StartDayFade());
        } 

        Global.input.controllers.maps.SetMapsEnabled(true, "Movement");
    }

    public IEnumerator StartDayFade()
    {
        bool fadedOut = false;
        float timeCounter = 0f;
        titleObject.GetComponent<Animator>().CrossFadeInFixedTime("Day" + (currentDay+1), 0f);
        while (!fadedOut)
        {
            if (timeCounter > 0.5f)
            {
                fadedOut = true;
            }
            else
            {
                timeCounter += Time.deltaTime;
            }

            titleObject.GetComponent<Renderer>().material.color = Color.Lerp(new Color(1f, 1f, 1f, 0f), Color.white, timeCounter/0.5f);

            yield return null;
        }

        titleObject.GetComponent<Renderer>().material.color = Color.white;

        yield return new WaitForSeconds(3.0f);

        // do sound effects here
        
        faderObject.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 0f);
        titleObject.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 0f);

        dayStarted?.Invoke(currentDay);
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

        Global.plantHealthData.ReviewDay(ShouldSpawnFlower());

        currentDay++;

        yield return new WaitForSeconds(1.0f);

        StartNewDay();
    }

    bool ShouldSpawnFlower()
    {
        // We check this before we update the day counter, so we want to know
        // whether TOMORROW will be the last day. Then, there is the off-by-one
        // consideration due to "indexing the totalNumDays list with currentDay"
        return currentDay + 1 == totalNumDays - 1;
    }
}

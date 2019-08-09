using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayManager : MonoBehaviour
{
    public delegate void DayStarted(int dayNo);
    public DayStarted dayStarted;

    public delegate void GameStarted(int dayNo);
    public GameStarted gameStarted;

    public delegate void StartPressed();
    public StartPressed startPressed;

    public bool seedPlanted = false;

    [SerializeField] private int totalNumDays;
    [SerializeField] private GameObject faderObject;
    [SerializeField] private float fadeTime = 1.0f;
    [SerializeField] private Color fadedInColor = Color.white;
    [SerializeField] private Color fadedOutColor = Color.white;

    public int currentDay = 0;

    void Start()
    {
        gameStarted?.Invoke(0);
        
        StartCoroutine(GameStartCoroutine());
    }

    IEnumerator GameStartCoroutine()
    {
        while (!Global.input.GetButtonDown("Start"))
        {
            yield return null;
        }

        startPressed?.Invoke();

        Global.cameraController.MoveCamera();

        yield return new WaitForSeconds(1.0f);

        StartNewDay(false);
    }

    public int GetTotalNumDays()
    {
        return totalNumDays;
    }

    public void StartNewDay(bool fadeIn = true)
    {
        // Invoke delegate
        dayStarted?.Invoke(currentDay);

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

        yield return new WaitForSeconds(1.0f);

        StartNewDay();
    }
}

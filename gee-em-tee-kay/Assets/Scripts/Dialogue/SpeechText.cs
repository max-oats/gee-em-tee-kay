using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/** The script that handles the data within the Canvas itself */
public class SpeechText : MonoBehaviour
{
    public float waveMultiplier = 10.0f;
    public float waveSpeed = 10.0f;

    public float jitterMultiplier = 10.0f;
    public int jitterEveryThisFrame = 2;
    
    public AnimationCurve introCurve;
    public AnimationCurve bigCurve;
    
    private Vector2 initialPosition = new Vector2();
    private bool bIsWavey = false;
    private bool bIsJittery = false;
    private bool bIsSwirly = false;
    private bool bIsBig = false;

    private RectTransform text;

    private float waveCounter = 0f;

    private float offset = 0f;

    private Vector3 initialScale;

    private bool bIsActive = false;
    private float initTime = 0f;

    private int jitterCounter = 0;

    public void Awake()
    {
        text = GetComponent<RectTransform>();
    }

    public void Start()
    {
        initialScale = text.localScale;
    }

    public void Update()
    {
        if (bIsWavey || bIsJittery || bIsSwirly)
        {
            Vector2 finalPos = initialPosition;

            if (bIsJittery)
            {
                jitterCounter++;
                if (jitterCounter == jitterEveryThisFrame)
                {
                    finalPos = finalPos + (Random.insideUnitCircle*jitterMultiplier);
                    jitterCounter = 0;
                }
            }

            if (bIsSwirly)
            {
                waveCounter = (Time.time + (offset/2)) * waveSpeed;
                finalPos = finalPos + new Vector2(waveMultiplier * Mathf.Cos(waveCounter), waveMultiplier * Mathf.Sin(waveCounter));
            }
            else if (bIsWavey)
            {
                waveCounter = (Time.time + offset) * waveSpeed;
                finalPos = finalPos + new Vector2(0.0f, waveMultiplier * Mathf.Sin(waveCounter));
            }

            text.anchoredPosition = finalPos;
        }

        if (bIsActive)
        {
            if (initTime < introCurve.length)
            {
                if (!bIsBig)
                    text.localScale = initialScale + Vector3.one * introCurve.Evaluate(initTime);
                else
                    text.localScale = initialScale + Vector3.one * bigCurve.Evaluate(initTime);
                
                initTime += Time.deltaTime;
            }
            else
            {
                text.localScale = initialScale;
                bIsActive = false;
            }
        }
    }

    public void Init(bool isBig)
    {
        text = GetComponent<RectTransform>();
        bIsActive = true;

        bIsBig = isBig;
    }

    public void SetWavey(float newOffset)
    {
        initialPosition = text.anchoredPosition;

        offset = newOffset;

        bIsWavey = true;
    }

    public void SetJittery()
    {
        initialPosition = text.anchoredPosition;

        bIsJittery = true;
    }

    public void SetSwirly(float newOffset)
    {
        initialPosition = text.anchoredPosition;

        offset = newOffset;

        bIsSwirly = true;
    }
}

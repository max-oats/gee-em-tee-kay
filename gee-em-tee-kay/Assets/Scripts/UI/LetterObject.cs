using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// todo: add text effects ()

/**
 * LetterObject
 * - Individual GameObject used for a letter
 */
public class LetterObject : MonoBehaviour
{
    [Tooltip("The text script"), SerializeField]
    private Text text;

    [Tooltip("The rect transform"), SerializeField]
    private RectTransform rt;

    [Tooltip("The strength of the wave effect"), SerializeField]
    private float waveStrength = 10.0f;

    [Tooltip("The speed of the wave effect"), SerializeField]
    private float waveSpeed = 10.0f;

    [Tooltip("The strength of the jitter effect"), SerializeField]
    private float jitterStrength = 10.0f;

    [Tooltip("The frequency of the jitter effect, >1 stops it from being illegible"), SerializeField]
    private int jitterEveryThisFrame = 2;
    
    [Tooltip("The curve used to animate the intro bounce of a letter"), SerializeField]
    private AnimationCurve introCurve;

    [Tooltip("The curve used to animate the BIG intro bounce of a letter"), SerializeField]
    private AnimationCurve bigIntroCurve;

    // Actual letter/number character
    [HideInInspector] public char character;

    // Color and delay
    [HideInInspector] public Color color = Color.white;
    [HideInInspector] public float postDelay = 0f;

    // Other characteristics (#lol)
    [HideInInspector] public bool isBold;
    [HideInInspector] public bool isItalics;
    [HideInInspector] public bool isWavey;
    [HideInInspector] public bool isJittery;
    [HideInInspector] public bool isScreenShake;
    [HideInInspector] public bool isSwirly;
    [HideInInspector] public bool isLineBreak;
    [HideInInspector] public bool isBig;

    // Offset, used for swirl/wave/etc
    [HideInInspector] public float offset = 0f;

    private Vector2 initialPosition;
    private bool isActive;
    private bool isGrowing;
    private float initTime = 0f;
    private int jitterCounter = 0;
    private float waveCounter = 0f;

    public void Update()
    {
        if (isActive)
        {
            if (isWavey || isJittery || isSwirly)
            {
                Vector2 finalPos = initialPosition;

                if (isJittery)
                {
                    jitterCounter++;
                    if (jitterCounter == jitterEveryThisFrame)
                    {
                        finalPos = finalPos + (Random.insideUnitCircle * jitterStrength);
                        jitterCounter = 0;
                    }
                }

                if (isSwirly)
                {
                    waveCounter = (Time.time + (offset/2)) * waveSpeed;
                    finalPos = finalPos + new Vector2(waveStrength * Mathf.Cos(waveCounter), waveStrength * Mathf.Sin(waveCounter));
                }
                else if (isWavey)
                {
                    waveCounter = (Time.time + offset) * waveSpeed;
                    finalPos = finalPos + new Vector2(0.0f, waveStrength * Mathf.Sin(waveCounter));
                }

                rt.anchoredPosition = finalPos;
            }

            if (isGrowing)
            {
                if (initTime < introCurve.length)
                {
                    if (!isBig)
                        rt.localScale = Vector3.one * introCurve.Evaluate(initTime);
                    else
                        rt.localScale = Vector3.one * bigIntroCurve.Evaluate(initTime);
                    
                    initTime += Time.deltaTime;
                }
                else
                {
                    rt.localScale = Vector3.one;
                    isGrowing = false;
                }
            }
        }
    }

    public void InitText(Font newFont, int fontSize, bool showByDefault)
    {
        text.text = character.ToString();

        text.fontSize = fontSize;
        text.font = newFont;

        if (isBold && isItalics)
        {
            text.fontStyle = FontStyle.BoldAndItalic;
        }
        else if (isBold)
        {
            text.fontStyle = FontStyle.Bold;
        }
        else if (isItalics)
        {
            text.fontStyle = FontStyle.Italic;
        }

        text.color = color;

        text.enabled = showByDefault;
    }

    public void SetPosition(Vector2 newPos, float width)
    {
        rt.anchoredPosition = newPos;

        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }

    public void Show(bool grow = false)
    {
        text.enabled = true;

        initialPosition = rt.anchoredPosition;

        isActive = true;

        if (grow)
        {
            isGrowing = true;
        }
    }

}
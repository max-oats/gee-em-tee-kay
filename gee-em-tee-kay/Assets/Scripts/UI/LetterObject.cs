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
    private float defaultWaveStrength = 10.0f;

    [Tooltip("The speed of the wave effect"), SerializeField]
    private float waveSpeed = 10.0f;

    [Tooltip("The strength of the jitter effect"), SerializeField]
    private float defaultJitterStrength = 10.0f;

    [Tooltip("The frequency of the jitter effect, >1 stops it from being illegible"), SerializeField]
    private int jitterEveryThisFrame = 2;
    
    [Tooltip("The curve used to animate the intro bounce of a letter"), SerializeField]
    private AnimationCurve introCurve;

    [Tooltip("The curve used to animate the BIG intro bounce of a letter"), SerializeField]
    private AnimationCurve bigIntroCurve;

    // Actual letter/number character
    [HideInInspector] public char character;

    // Other characteristics (#lol)
    [HideInInspector] public bool isBold;
    [HideInInspector] public bool isItalics;
    [HideInInspector] public bool isLineBreak;
    [HideInInspector] public bool isBig;

    // Floats
    [HideInInspector] public Color color = Color.white;
    [HideInInspector] public float waveStrength;
    [HideInInspector] public float jitterStrength;
    [HideInInspector] public float swirlStrength;

    // Action character stuff
    [HideInInspector] public bool isActionCharacter = false;
    [HideInInspector] public float delay = 0f;
    [HideInInspector] public float screenShakeStrength = 0f;

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
            if (waveStrength > 0f || jitterStrength > 0f || swirlStrength > 0f)
            {
                Vector2 finalPos = initialPosition;

                if (jitterStrength > 0f)
                {
                    jitterCounter++;
                    if (jitterCounter == jitterEveryThisFrame)
                    {
                        finalPos = finalPos + (Random.insideUnitCircle * defaultJitterStrength * jitterStrength);
                        jitterCounter = 0;
                    }
                }

                if (swirlStrength > 0f)
                {
                    waveCounter = (Time.time + (offset/2)) * waveSpeed;
                    finalPos = finalPos + new Vector2(defaultWaveStrength * swirlStrength * Mathf.Cos(waveCounter), defaultWaveStrength * swirlStrength * Mathf.Sin(waveCounter));
                }
                else if (waveStrength > 0f)
                {
                    waveCounter = (Time.time + offset) * waveSpeed;
                    finalPos = finalPos + new Vector2(0.0f, defaultWaveStrength * waveStrength * Mathf.Sin(waveCounter));
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
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, width * 1.7f);
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
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

    // Offset, used for swirl/wave/etc
    [HideInInspector] public float offset = 0f;

    public void InitText(Font newFont, int fontSize)
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
    }

    public void SetPosition(Vector2 newPos, float width)
    {
        rt.anchoredPosition = newPos;

        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }

}
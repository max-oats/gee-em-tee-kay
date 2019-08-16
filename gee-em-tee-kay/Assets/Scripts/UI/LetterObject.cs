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

    // Offset, used for swirl/wave/etc
    [HideInInspector] public float offset = 0f;

    public void InitText()
    {
        text.text = character.ToString();
    }

}
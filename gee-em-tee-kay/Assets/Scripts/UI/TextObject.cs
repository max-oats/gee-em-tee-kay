using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// todo: pool letterobjects instead of constantly destroying/creating them

/**
 * TextObject
 * - Contains a List of subobjects of type "LetterObject".
 * - Equivalent to the Unity "Text" UI class.
 */
public class TextObject : MonoBehaviour
{
    /** Serialized fields */
    [Tooltip("The letter object being created"), SerializeField]
    private GameObject _letterObject;

    [Tooltip("The text string within the container"), SerializeField, TextArea]
    private string text;

    [Tooltip("Should the text be parsed using the tags?"), SerializeField]
    private bool parseText;

    [Tooltip("The font that the text should use"), SerializeField]
    private Font font;

    [Tooltip("The size of the font"), SerializeField]
    private int fontSize;

    [Tooltip("The horizontal letter spacing of the font"), SerializeField]
    private float extraKerning;

    [Tooltip("The line spacing between in addition to the font size"), SerializeField]
    private float lineSpacing;

    [Tooltip("Should the width of the object be used as the line length"), SerializeField]
    public bool useWidthAsLineLength;
    
    [Tooltip("The length of lines"), SerializeField]
    private int lineLength = 15;

    [Tooltip("The case format of the text"), SerializeField]
    private TextFormat textCaseFormatting = TextFormat.Standard;
    
    [Tooltip("Whether the text should be visible by default, or should appear later"), SerializeField]
    public bool visibleByDefault = true;

    [Tooltip("The speed of the text. Used only if !visibleByDefault"), SerializeField]
    private float textSpeed = 0.05f;

    [Tooltip("The offset used when wavey or swirly"), SerializeField]
    private float offsetWhenWavey = 0.05f;

    [Tooltip("The list of colours used for colour settings"), SerializeField]
    private List<Color> normalColours;

    [Tooltip("The list of colours used for rainbow colour settings"), SerializeField]
    private List<Color> rainbowColours;

    [Tooltip("The list of delays used for delay timing"), SerializeField]
    private List<float> delays;

    /** Privates */
    private float fontWidth = 0f;
    private int noOfLines = 0; // The number of lines in the text object
    private string parsedString; // The parsed string, only used if parseText==true
    private List<LetterObject> letterObjects = new List<LetterObject>(); // A list of the letter objects

    /**
     * SetText
     * - Updates the existing text.
     * - Kills all existing LetterObjects
     * - Builds new LetterObjects
     */
    public void SetText(string newText)
    {
        // Destroy existing letters
        DestroyLetters();

        // Update the text to the new text
        text = newText;

        // Update text case
        if (textCaseFormatting == TextFormat.AllLower)
        {
            text = text.ToLower();
        }
        else if (textCaseFormatting == TextFormat.AllUpper)
        {
            text = text.ToUpper();
        }

        fontWidth = (float)fontSize * 0.6f;

        // Build new letters
        BuildLetters();
    }

    void DestroyLetters()
    {
        // Destroy all existing letter objects
        foreach (LetterObject letterObject in letterObjects)
        {
            UnityEditor.EditorApplication.delayCall+=()=>
            {
                if (letterObject != null)
                    DestroyImmediate(letterObject.gameObject);
            };
        }

        // Clear the letter objects
        letterObjects.Clear();
    }

    void BuildLetters()
    {
        // Reset parsed string
        parsedString = "";
        
        // Listeners
        bool bIsListening = false; // Used to check whether we are listening for the next character
        bool bIsListeningForColour = false; // Checks whether we're listening for colour chars
        bool bIsListeningForDelay = false; // Checks whether we're listening for delay chars

        // Toggles
        bool bIsBold = false;
        bool bIsItalics = false;
        bool bIsWavey = false;
        bool bIsJittery = false;
        bool bIsSwirly = false;
        bool bIsShake = false;
        bool bIsRainbow = false;

        noOfLines = 0;

        // Used to increment rainbow colours
        int rainbowCounter = 0;

        Color color = Color.white; // Colour setting (default white)
        float delay = textSpeed;

        int counter = 0; // Counter used when building up a code
        string letterCode = ""; // Code built up when we're listening

        float offset = 0f;

        if (parseText)
        {
            // Iterate through the text string and create an array of characters
            foreach (char c in text)
            {
                if (c == '\\')
                {
                    // Start listening for next character
                    bIsListening = true;
                }
                else if (bIsListening)
                {
                    // Set to lower for listening purposes
                    char listenedCharacter = char.ToLower(c);

                    // Stop listening
                    bIsListening = false;

                    // Check the listened to character
                    if (listenedCharacter == 'b') // BOLD
                    {
                        bIsBold = !bIsBold;
                    }
                    else if (listenedCharacter == 'i') // ITALICS
                    {
                        bIsItalics = !bIsItalics;
                    }
                    else if (listenedCharacter == 'c') // COLOURS
                    {
                        // Start listening for colour inputs
                        bIsListeningForColour = true;
                    }
                    else if (listenedCharacter == 'w') // WAVEY
                    {
                        bIsWavey = !bIsWavey;
                    }
                    else if (listenedCharacter == 'd') // DELAY
                    {
                        bIsListeningForDelay = true;
                    }
                    else if (listenedCharacter == 's') // SCREEN SHAKE
                    {
                        bIsShake = true;
                    }
                    else if (listenedCharacter == 'r')
                    {
                        bIsRainbow = !bIsRainbow;
                    }
                    else if (listenedCharacter == 'j')
                    {
                        bIsJittery = !bIsJittery;
                    }
                    else if (listenedCharacter == 'a')
                    {
                        bIsSwirly = !bIsSwirly;
                    }
                }
                else if (bIsListeningForColour)
                {
                    // Add colour code and increment counter
                    letterCode += c;
                    counter++;

                    if (counter > 2)
                    {
                        // Reset colour
                        color = Color.white;

                        // Set current colour
                        int colorCode = 0;

                        // Attempt to parse colour code. If not, set to white by default
                        if (int.TryParse(letterCode, out colorCode))
                        {
                            if (colorCode < normalColours.Count && colorCode >= 0)
                            {
                                color = normalColours[colorCode];
                            }
                        }

                        // Reset colour stuff
                        letterCode = "";
                        counter = 0;

                        // Stop listening
                        bIsListeningForColour = false;
                    }
                }
                else if (bIsListeningForDelay)
                {
                    // Add delay code and increment counter
                    letterCode += c;
                    counter++;

                    if (counter > 2)
                    {
                        // Reset colour
                        delay = 0f;

                        // Set current colour
                        int delayCode = 0;

                        // Attempt to parse colour code. If not, set to white by default
                        if (int.TryParse(letterCode, out delayCode))
                        {
                            if (delayCode < delays.Count && delayCode >= 0)
                            {
                                delay = delays[delayCode];
                            }
                        }

                        // Reset delay stuff
                        letterCode = "";
                        counter = 0;

                        // Stop listening
                        bIsListeningForDelay = false;
                    }
                }
                else
                {
                    // If we're not using any special character, we must be using the actual character
                    GameObject go = Instantiate(_letterObject, transform);
                    LetterObject letterObject = go.GetComponent<LetterObject>();

                    letterObject.character = c;
                    letterObject.color = color;
                    letterObject.postDelay = delay;

                    // Characteristics
                    letterObject.isBold = bIsBold;
                    letterObject.isItalics = bIsItalics;
                    letterObject.isJittery = bIsJittery;
                    letterObject.isScreenShake = bIsShake;
                    letterObject.isWavey = bIsWavey;
                    letterObject.isSwirly = bIsSwirly;

                    // Update offset if toggle
                    if (bIsWavey || bIsSwirly)
                    {
                        if (c != ' ')
                        {
                            offset += offsetWhenWavey;
                        }
                    }
                    else
                    {
                        offset = 0f;
                    }

                    letterObject.offset = offset;

                    // Update rainbow stuff
                    if (bIsRainbow && rainbowColours.Count > 0)
                    {
                        letterObject.color = rainbowColours[rainbowCounter];

                        rainbowCounter++;
                        if (rainbowCounter >= rainbowColours.Count)
                        {
                            rainbowCounter = 0;
                        }
                    }

                    letterObject.InitText(font, fontSize);

                    letterObjects.Add(letterObject);

                    // Build up final string
                    parsedString += c;

                    // Clean up "one-shot" settings
                    bIsShake = false;
                }
            }
        }
        else
        {
            parsedString = text;

            foreach (char c in text)
            {
                // Build character
                GameObject go = Instantiate(_letterObject, transform);
                LetterObject letterObject = go.GetComponent<LetterObject>();

                letterObject.character = c;

                letterObject.InitText(font, fontSize);

                letterObjects.Add(letterObject);
            }
        }

        if (lineLength > 0)
        {
            int finalLineLength = lineLength;

            if (useWidthAsLineLength)
            {
                finalLineLength = (int)(GetComponent<RectTransform>().sizeDelta.x / fontWidth);
            }

            // Step through the final string, grabbing the nearest spaces
            for (int i = finalLineLength; i < parsedString.Length; i += finalLineLength)
            {
                int spaceIndex = parsedString.LastIndexOf(" ", i);

                if (spaceIndex != -1 && spaceIndex >= (i - finalLineLength))
                {
                    // Found a space!
                    i = spaceIndex;

                    // Set to be a line break
                    letterObjects[i].isLineBreak = true;

                    // Increase number of lines
                    noOfLines++;
                }
                else
                {
                    // If not found a space, just break on the line
                    letterObjects[i].isLineBreak = true;
                    noOfLines++;
                }
            }
        }

        // Place letters
        Vector2 letterPlacement = Vector2.zero;

        foreach (LetterObject letterObject in letterObjects)
        {
            // Set new position
            letterObject.SetPosition(letterPlacement, fontSize);

            // Update X location
            letterPlacement.x += (fontWidth + extraKerning);

            // Add linebreak if necessary
            if (letterObject.isLineBreak)
            {
                // Update Y location
                letterPlacement.y -= (fontSize + lineSpacing);
                letterPlacement.x = 0f;
            }
        }

        // Update size
    }

    /** Debug editor script */
    public void UpdateUIString()
    {
        SetText(text);
    }
}

[CustomEditor(typeof(TextObject))]
public class ObjectBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TextObject textObject = (TextObject)target;

        if(GUILayout.Button("Update UI String"))
        {
            textObject.UpdateUIString();
        }

        DrawDefaultInspector();
    }
}
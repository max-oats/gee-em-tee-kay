using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// todo: pool letterobjects instead of constantly destroying/creating them

[System.Serializable]
public class ColourPreset
{  
    public string name;

    public Color color;
}

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

    [Tooltip("The horizontal letter spacing of the font"), SerializeField]
    private string wideCharacters;
    
    [Tooltip("The horizontal letter spacing of the font"), SerializeField]
    private float wideCharacterMultiplier = 1.1f;

    [Tooltip("The horizontal letter spacing of the font"), SerializeField]
    private string thinCharacters;

    [Tooltip("The horizontal letter spacing of the font"), SerializeField]
    private float thinCharacterMultiplier = 0.8f;

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
    private List<ColourPreset> colourPresets;

    [Tooltip("The list of colours used for rainbow colour settings"), SerializeField]
    private List<Color> rainbowColours;

    /** Privates */
    private float fontWidth = 0f;
    private float longestLine = 0f;
    private int noOfLines = 1; // The number of lines in the text object
    private string parsedString; // The parsed string, only used if parseText==true
    private List<LetterObject> letterObjects = new List<LetterObject>(); // A list of the letter objects

    public void Start()
    {
        if (parseText && visibleByDefault)
        {
            SetText(text);

            foreach (LetterObject lo in letterObjects)
            {
                lo.Show();
            }
        }
    }

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
            if (Application.isPlaying)
            {
                if (letterObject != null)
                    Destroy(letterObject.gameObject);
            }
            else
            {
                UnityEditor.EditorApplication.delayCall+=()=>
                {
                    if (letterObject != null)
                        DestroyImmediate(letterObject.gameObject);
                };
            }            
        }

        foreach (Transform child in transform)
        {
            if (Application.isPlaying)
            {
                if (child.GetComponent<LetterObject>() != null)
                    Destroy(child.gameObject);
            }
            else
            {
                if (child.GetComponent<LetterObject>() != null)
                {
                    UnityEditor.EditorApplication.delayCall+=()=>
                    {
                        if (child != null)
                            DestroyImmediate(child.gameObject);
                    };
                }
            }
        }

        // Clear the letter objects
        letterObjects.Clear();
    }

    void ParseTag(string tag)
    {

    }

    void BuildLetters()
    {
        parsedString = ""; // Reset parsed string
        noOfLines = 1; // Reset number of lines
        
        // Listeners
        bool isListeningTag = false; // Used to check whether we are listening for the next character
        bool isListeningAction = false; // Used to check whether we are listening for the next character

        // Toggles
        bool isBold = false;
        bool isItalics = false;
        bool isRainbow = false;
        bool isBig = false;

        // Different values
        Color color = Color.white; // Colour setting (default white)
        float waveStrength = 0f;
        float jitterStrength = 0f;
        float swirlStrength = 0f;

        float delay = 0f;
        float shakeStrength = 0f;

        int rainbowCounter = 0;

        float offset = 0f;

        if (parseText)
        {
            // Set up built tag
            string builtTag = "";

            // Iterate through the text string and create an array of characters
            foreach (char c in text)
            {
                if (isListeningTag)
                {
                    if (c == ']')
                    {
                        // Stop listening
                        isListeningTag = false;

                        bool setTagTo = true;

                        // Check tag and apply
                        if (builtTag.Length > 0)
                        {
                            // Set to false if its an end tag
                            if (builtTag[0] == '/')
                            {
                                setTagTo = false;
                                builtTag = builtTag.Remove(0, 1);
                            }

                            // Check tag and apply
                            if (builtTag.Length > 0)
                            {
                                // Grab multiple strings
                                string[] tagStrings = builtTag.Split('=');
                                
                                // Set tag name to this
                                string tagName = tagStrings[0];

                                // Standard toggles
                                if (tagName.Equals("bold") || tagName.Equals("b"))
                                {
                                    isBold = setTagTo;
                                }
                                else if (tagName.Equals("italics") || tagName.Equals("i"))
                                {
                                    isItalics = setTagTo;
                                }
                                else if (tagName.Equals("rainbow") || tagName.Equals("r"))
                                {
                                    isRainbow = setTagTo;
                                }
                                else if (tagName.Equals("big") || tagName.Equals("e"))
                                {
                                    isBig = setTagTo;
                                }
                                else
                                {
                                    // Set up param
                                    string tagParam = "";
                                    if (tagStrings.Length >= 2)
                                    {
                                        tagParam = tagStrings[1];
                                    }

                                    if (tagName.Equals("wavey") || tagName.Equals("wave") || tagName.Equals("w"))
                                    {
                                        if (setTagTo)
                                        {
                                            float outputStrength = 0f;
                                            if (tagParam.Length > 0 && float.TryParse(tagParam, out outputStrength))
                                            {
                                                waveStrength = outputStrength;
                                            }
                                            else
                                            {
                                                waveStrength = 1f;
                                            }
                                        }
                                        else
                                        {
                                            waveStrength = 0f;
                                        }
                                    }
                                    else if (tagName.Equals("jitter") || tagName.Equals("jittery") || tagName.Equals("j"))
                                    {
                                        if (setTagTo)
                                        {
                                            float outputStrength = 0f;
                                            if (tagParam.Length > 0 && float.TryParse(tagParam, out outputStrength))
                                            {
                                                jitterStrength = outputStrength;
                                            }
                                            else
                                            {
                                                jitterStrength = 1f;
                                            }
                                        }
                                        else
                                        {
                                            jitterStrength = 0f;
                                        }
                                    }
                                    else if (tagName.Equals("swirly") || tagName.Equals("a"))
                                    {
                                        if (setTagTo)
                                        {
                                            float outputStrength = 0f;
                                            if (tagParam.Length > 0 && float.TryParse(tagParam, out outputStrength))
                                            {
                                                swirlStrength = outputStrength;
                                            }
                                            else
                                            {
                                                swirlStrength = 1f;
                                            }
                                        }
                                        else
                                        {
                                            swirlStrength = 0f;
                                        }
                                    }
                                    else if (tagName.Equals("color") || tagName.Equals("colour") || tagName.Equals("c"))
                                    {
                                        color = Color.white;
                                        Color newColor = Color.white;
                                        if (setTagTo)
                                        {
                                            if (tagParam.Length > 0)
                                            {
                                                // Param exists. Time to select colour
                                                if (tagParam[0] == '!')
                                                {
                                                    // Hex code
                                                    if (ColorUtility.TryParseHtmlString(tagParam, out newColor))
                                                    {
                                                        color = newColor;
                                                    }
                                                }
                                                else
                                                {
                                                    ColourPreset newColourPreset = colourPresets.Find(x => x.name == tagName);
                                                    if (newColourPreset != null)
                                                    {
                                                        color = newColourPreset.color;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Debug.LogError("TEXT: Tag length with '/' removed = 0, text input: " + text);
                            }
                        }
                        else
                        {
                            Debug.LogError("TEXT: Tag length = 0, text input: " + text);
                        }
                        
                        builtTag = "";
                    }
                    else
                    {
                        // Set to lower for listening purposes
                        char listenedCharacter = char.ToLower(c);

                        builtTag += listenedCharacter;
                    }
                }
                else if (isListeningAction)
                {
                    if (c == '}')
                    {
                        isListeningAction = false;

                        // Check tag and apply
                        if (builtTag.Length > 0)
                        {
                            // Grab multiple strings
                            string[] tagStrings = builtTag.Split('=');
                            
                            // Set tag name to this
                            string tagName = tagStrings[0];

                            // Set up param
                            string tagParam = "";
                            if (tagStrings.Length >= 2)
                            {
                                tagParam = tagStrings[1];
                            }

                            if (tagName.Equals("shake") || tagName.Equals("s"))
                            {
                                float outputStrength = 0f;
                                if (tagParam.Length > 0 && float.TryParse(tagParam, out outputStrength))
                                {
                                    shakeStrength = outputStrength;
                                }
                                else
                                {
                                    shakeStrength = 1f;
                                }
                            }
                            else if (tagName.Equals("delay") || tagName.Equals("d"))
                            {
                                float outputStrength = 0f;
                                if (tagParam.Length > 0 && float.TryParse(tagParam, out outputStrength))
                                {
                                    delay = outputStrength;
                                }
                                else
                                {
                                    delay = 0.2f;
                                }
                            }

                            // If we're not using any special character, we must be using the actual character
                            GameObject go = Instantiate(_letterObject, transform);
                            LetterObject letterObject = go.GetComponent<LetterObject>();

                            letterObject.isActionCharacter = true;
                            letterObject.screenShakeStrength = shakeStrength;
                            letterObject.delay = delay;

                            letterObjects.Add(letterObject);

                            delay = 0f;
                            shakeStrength = 0f;
                        }
                        else
                        {
                            Debug.LogError("TEXT: Tag length = 0, text input: " + text);
                        }
                        
                        builtTag = "";
                    }
                    else
                    {
                        // Set to lower for listening purposes
                        char listenedCharacter = char.ToLower(c);

                        builtTag += listenedCharacter;
                    }
                }
                else if (c == '{')
                {
                    isListeningAction = true;
                }
                else if (c == '[')
                {
                    // Start listening for next character
                    isListeningTag = true;
                }
                else
                {
                    // If we're not using any special character, we must be using the actual character
                    GameObject go = Instantiate(_letterObject, transform);
                    LetterObject letterObject = go.GetComponent<LetterObject>();

                    letterObject.character = c;
                    letterObject.color = color;

                    // Characteristics
                    letterObject.isBold = isBold;
                    letterObject.isItalics = isItalics;
                    letterObject.isBig = isBig;

                    letterObject.jitterStrength = jitterStrength;
                    letterObject.waveStrength = waveStrength;
                    letterObject.swirlStrength = swirlStrength;

                    // Update offset if toggle
                    if (waveStrength > 0f || swirlStrength > 0f)
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
                    if (isRainbow && rainbowColours.Count > 0)
                    {
                        letterObject.color = rainbowColours[rainbowCounter];

                        rainbowCounter++;
                        if (rainbowCounter >= rainbowColours.Count)
                        {
                            rainbowCounter = 0;
                        }
                    }

                    letterObject.InitText(font, fontSize, visibleByDefault);

                    letterObjects.Add(letterObject);

                    // Build up final string
                    parsedString += c;
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

                letterObject.InitText(font, fontSize, visibleByDefault);

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

            if (finalLineLength > 0)
            {
                // Step through the final string, grabbing the nearest spaces
                for (int i = finalLineLength-1; i < parsedString.Length; i += finalLineLength)
                {
                    int spaceIndex = parsedString.LastIndexOf(" ", i);

                    if (spaceIndex != -1 && spaceIndex > (i - finalLineLength))
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
        }

        // Place letters
        Vector2 letterPlacement = Vector2.zero;
        longestLine = 0f;

        foreach (LetterObject letterObject in letterObjects)
        {
            // Set new position
            letterObject.SetPosition(letterPlacement, fontSize);

            // Update X location (if real character)
            if (!letterObject.isActionCharacter)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(letterObject.character.ToString().ToLower(), "[" + wideCharacters + "]"))
                {
                    letterPlacement.x += ((fontWidth * wideCharacterMultiplier) + extraKerning);
                }
                else if (System.Text.RegularExpressions.Regex.IsMatch(letterObject.character.ToString().ToLower(), "[" + thinCharacters + "]"))
                {
                    letterPlacement.x += ((fontWidth * thinCharacterMultiplier) + extraKerning);
                }
                else
                {
                    letterPlacement.x += (fontWidth + extraKerning);  
                }
            }
            
            if (letterPlacement.x > longestLine)
            {
                longestLine = letterPlacement.x;
            }

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

    /**
     * Attempt to show by index. Used for loop-based letter showing (dialogue for example)
     * - Returns true if successful.
     * - Also contains an out parameter.
     */
    public LetterObject GetLetterObject(int index)
    {
        if (index < letterObjects.Count && index >= 0)
        {
            return letterObjects[index];
        }

        Debug.LogError("Attempted to get a letter with index: " + index + ", index out of range.");

        return null;
    }

    public List<LetterObject> GetLetterObjects()
    {
        return letterObjects;
    }

    public void SetPosition(Vector2 newPos)
    {
        GetComponent<RectTransform>().anchoredPosition = newPos;
    }

    public int GetLengthOfText()
    {
        return letterObjects.Count;
    }

    public Vector2 GetSize()
    {
        return new Vector2(longestLine, noOfLines * (fontSize + lineSpacing));
    }

    public float GetTextSpeed()
    {
        return textSpeed;
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
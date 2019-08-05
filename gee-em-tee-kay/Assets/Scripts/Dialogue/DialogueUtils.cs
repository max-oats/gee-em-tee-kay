using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/** Class to hold data about each character before displaying them. */
public class DialogueCharacter
{
    public char character; // Actual character
    
    // Properties
    public bool isBold = false;
    public bool isItalics = false;
    public bool isWavey = false;
    public bool isJittery = false;
    public bool isSwirly = false;
    public bool isShake = false;
    public Color colour = new Color(255f, 255f, 255f, 255f);
    public float afterDelay = 0f;

    public float waveyOffset = 0f;
    
    public bool isLineBreak = false; // Should be used only when (character == " ")

    public bool Equals(char otherChar)
    {
        return character == otherChar;
    }

    public void SetUpTextObject(Text textObject)
    {
        textObject.text = character.ToString();

        textObject.GetComponent<SpeechText>().Init(false);

        if (isBold)
            textObject.fontStyle = FontStyle.Bold;

        if (isItalics)
            textObject.fontStyle = FontStyle.Italic;

        if (isJittery)
            textObject.GetComponent<SpeechText>().SetJittery();

        if (isSwirly)
            textObject.GetComponent<SpeechText>().SetSwirly(waveyOffset);
        else if (isWavey)
            textObject.GetComponent<SpeechText>().SetWavey(waveyOffset);

        // if (isShake)
        //     Global.cameraControl.ScreenShake(Global.dialogueHandler.screenShakeMultiplier);

        textObject.color = colour;
    }
}

/** An entire string of DialogueCharacters */
public class DialogueString
{
    public string initialString; // The original input string
    public string parsedString; // The input string
    public List<DialogueCharacter> dialogue = new List<DialogueCharacter>(); // The string as a list of DialogueCharacters. Used to present the string.

    public int noOfLines = 1; // The number of lines in the string
    public int lineLength = 0; // The length of given lines

    /** Constructor */
    public DialogueString(string inputString, bool parsed = true)
    {
        // Initially just update the associated string
        UpdateString(inputString, parsed);
    }

    public void UpdateString(string inputString, bool parsed = true)
    {
        // Kill whatever string currently exists including all text objects
        KillExistingString();

        initialString = inputString;
        parsedString = "";

        // Default all uppercase
        initialString = initialString.ToLower();
        
        // Toggles
        bool bIsListening = false; // Used to check whether we are listening for the next character
        bool bIsListeningForColour = false; // Checks whether we're listening for colour chars
        bool bIsListeningForDelay = false; // Checks whether we're listening for delay chars
        bool bIsBold = false; // Toggle to say if bold
        bool bIsItalics = false; // Toggle to say if italics
        bool bIsWavey = false;
        bool bIsJittery = false;
        bool bIsSwirly = false;
        bool bIsShake = false;
        bool bIsDelayToggle = false;
        bool bIsRainbow = false;

        int rainbowCounter = 0;

        Color color = Color.white; // Colour setting (default white)
        float delay = Global.dialogueHandler.textSpeed;

        int counter = 0; // Counter used when building up a code
        string letterCode = ""; // Code built up when we're listening

        float offset = 0f;

        if (parsed)
        {
            // Iterate through the text string and create an array of characters
            foreach (char c in initialString)
            {
                if (c == '\\')
                {
                    // Start listening for next character
                    bIsListening = true;
                }
                else if (bIsListening)
                {
                    // Stop listening
                    bIsListening = false;

                    // Check the listened to character
                    if (c == 'b') // BOLD
                    {
                        bIsBold = !bIsBold;
                    }
                    else if (c == 'i') // ITALICS
                    {
                        bIsItalics = !bIsItalics;
                    }
                    else if (c == 'c') // COLOURS
                    {
                        // Start listening for colour inputs
                        bIsListeningForColour = true;
                    }
                    else if (c == 'w') // WAVEY
                    {
                        bIsWavey = !bIsWavey;
                    }
                    else if (c == 'd') // DELAY
                    {
                        bIsListeningForDelay = true;
                    }
                    else if (c == 's') // SCREEN SHAKE
                    {
                        bIsShake = true;
                    }
                    else if (c == 'r')
                    {
                        bIsRainbow = !bIsRainbow;
                    }
                    else if (c == 'j')
                    {
                        bIsJittery = !bIsJittery;
                    }
                    else if (c == 'a')
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
                        // Set current colour
                        color = Global.dialogueHandler.normalColours[int.Parse(letterCode)];

                        // Reset colour stuff
                        letterCode = "";
                        counter = 0;

                        // Stop listening
                        bIsListeningForColour = false;
                    }
                }
                else if (bIsListeningForDelay)
                {
                    if (counter == 1)
                    {
                        if (float.Parse(letterCode) == 9)
                        {
                            bIsDelayToggle = true;
                            letterCode = "0";
                        }
                    }

                    // Add delay code and increment counter
                    letterCode += c;
                    counter++;

                    if (counter > 2)
                    {
                        // Set current colour
                        delay = Global.dialogueHandler.delayCodes[int.Parse(letterCode)];

                        // Reset colour stuff
                        letterCode = "";
                        counter = 0;

                        // Stop listening
                        bIsListeningForDelay = false;
                    }
                }
                else
                {
                    // Build character
                    DialogueCharacter dc = new DialogueCharacter();

                    // Set-up characteristics
                    dc.character = c;
                    dc.colour = color;
                    dc.isBold = bIsBold;
                    dc.isItalics = bIsItalics;
                    dc.isWavey = bIsWavey;
                    dc.afterDelay = delay;
                    dc.isShake = bIsShake;
                    dc.isJittery = bIsJittery;
                    dc.isSwirly = bIsSwirly;

                    if (bIsWavey || bIsSwirly)
                    {
                        if (c != ' ')
                            offset += Global.dialogueHandler.letterOffset;
                    }
                    else
                    {
                        offset = 0f;
                    }

                    dc.waveyOffset = offset;

                    // Update rainbow stuff
                    if (bIsRainbow)
                    {
                        dc.colour = Global.dialogueHandler.rainbowColours[rainbowCounter];

                        rainbowCounter++;
                        if (rainbowCounter >= Global.dialogueHandler.rainbowColours.Count)
                        {
                            rainbowCounter = 0;
                        }
                    }

                    // Add to array
                    dialogue.Add(dc);

                    // Build up final string
                    parsedString += c;

                    // Clean up "one-shot" settings
                    bIsShake = false;

                    if (!bIsDelayToggle)
                        delay = Global.dialogueHandler.textSpeed;
                }
            }
        }
        else
        {
            parsedString = initialString;

            int charcounter = 0;

            foreach (char c in parsedString)
            {
                bool bFound = false;
                bool bListenerFound = false;

                if (c == '\\')
                {
                    // Start listening for next character
                    bIsListening = true;
                }
                else if (bIsListening)
                {
                    // Stop listening
                    bIsListening = false;

                    // Check the listened to character
                    if (c == 'B') // BOLD
                    {
                        bFound = true;
                    }
                    else if (c == 'I') // ITALICS
                    {
                        bFound = true;
                    }
                    else if (c == 'C') // COLOURS
                    {
                        // Start listening for colour inputs
                        bIsListeningForColour = true;
                    }
                    else if (c == 'W') // WAVEY
                    {
                        bFound = true;
                    }
                    else if (c == 'D') // DELAY
                    {
                        bIsListeningForDelay = true;
                    }
                    else if (c == 'S') // SCREEN SHAKE
                    {
                        bFound = true;
                    }
                    else if (c == 'R')
                    {
                        bFound = true;
                    }
                    else if (c == 'J')
                    {
                        bFound = true;
                    }
                    else if (c == 'A')
                    {
                        bFound = true;
                    }
                }
                else if (bIsListeningForColour)
                {
                    counter++;

                    if (counter > 2)
                    { 
                        bListenerFound = true;
                        bIsListeningForColour = false;
                        counter = 0;
                    }
                }
                else if (bIsListeningForDelay)
                {
                    counter++;

                    if (counter > 2)
                    { 
                        bListenerFound = true;
                        bIsListeningForDelay = false;
                        counter = 0;
                    }
                }

                // Build character
                DialogueCharacter dc = new DialogueCharacter();

                // Set-up characteristics
                dc.character = c;
                dc.colour = color;

                dialogue.Add(dc);

                charcounter++;

                if (bFound)
                {
                    Color newColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);
                    dc.colour = newColor;
                    dialogue[charcounter-2].colour = newColor;
                }

                if (bListenerFound)
                {
                    Color newColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);
                    dc.colour = newColor;
                    dialogue[charcounter-2].colour = newColor;
                    dialogue[charcounter-3].colour = newColor;
                    dialogue[charcounter-4].colour = newColor;
                    dialogue[charcounter-5].colour = newColor;
                }
            }
        }

        // Set default line lengths
        lineLength = parsedString.Length;
    }

    public void KillExistingString()
    {
        dialogue.Clear();
    }

    public void AddLineBreaks(int initialLineLength)
    {
        lineLength = Global.dialogueHandler.lineLength;

        if (parsedString.Length < lineLength)
        {
            lineLength = parsedString.Length;
        }

        // Step through the final string, grabbing the nearest spaces
        for (int i = lineLength; i < parsedString.Length; i += lineLength)
        {
            int spaceIndex = parsedString.LastIndexOf(" ", i);

            // Found a space!
            if (spaceIndex != -1)
            {
                i = spaceIndex;

                // Set to be a line break
                dialogue[spaceIndex].isLineBreak = true;

                // Increase number of lines
                noOfLines++;
            }
            else
            {
                dialogue[i].isLineBreak = true;
                noOfLines++;
            }
        }
    }

}

// Utils class
public static class DialogueUtils
{
    public static Text CreateTextObject(GameObject _textPrefab, DialogueCharacter dc, Transform parent, Vector2 location, out float delay)
    {
        // Create the text object and grab the text
        GameObject go = GameObject.Instantiate(_textPrefab, parent);
        Text textObject = go.GetComponent<Text>();

        // Set up the text object
        textObject.rectTransform.anchoredPosition = location;
        dc.SetUpTextObject(textObject);

        delay = dc.afterDelay;

        return textObject;
    }
}
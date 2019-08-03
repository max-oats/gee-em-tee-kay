﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System.Collections.Generic;

/** Dialogue Handler. */
public class DialogueHandler : Yarn.Unity.DialogueUIBehaviour
{
    /// A delegate (ie a function-stored-in-a-variable) that
    /// we call to tell the dialogue system about what option
    /// the user selected
    private Yarn.OptionChooser SetSelectedOption;

    public GameObject textPfb;

    /// How quickly to show the text, in seconds per character
    [Tooltip("How quickly to show the text, in seconds per character")]
    public float textSpeed = 0.025f;

    public float speedyTextMultiplier = 0.3f;

    public Vector2 defaultInset = new Vector2(100f, -100f);

    public float heightPadding = 100f;
    public float widthPadding = 100f;

    public float letterHeight = 100f;
    public float letterWidth = 100f;

    public float letterOffset = 0.2f;

    public float optionOffset = 10f;
    public float optionDelay = 0.1f;

    public float cameraUpOffset = -2f;

    public int lineLength = 20;

    public float lineLengthMultiplier = 0.85f;
    
    public float offsetAddition = 0.1f;

    public float screenShakeMultiplier = 0.6f;

    public SpeechBubbleHandler playerSpeechHandler;

    public List<Color> normalColours;
    public List<float> delayCodes;
    public List<Color> rainbowColours;

    public string rainbowColourFile = "/DialogueData/rainbow.txt";
    public string normalColourFile = "/DialogueData/colours.txt";
    public string delayFile = "/DialogueData/delays.txt";

    public bool inDialogue = false;

    private bool currentlyRunningLine = false;
    private float delayTimeMultiplier = 1f;

    void Start()
    {
        rainbowColours = GeneralUtils.LoadColoursFromFile(rainbowColourFile);
        normalColours = GeneralUtils.LoadColoursFromFile(normalColourFile);
        delayCodes = GeneralUtils.LoadFloatsFromFile(delayFile);    
    }

    void Update()
    {
        if (Global.input.GetButtonDown("Talk"))
        {

        }

        if (currentlyRunningLine && Global.input.GetButtonDown("Talk"))
        {
            delayTimeMultiplier = speedyTextMultiplier;
        }
    }

    /// Show a line of dialogue, gradually
    public override IEnumerator RunLine (Yarn.Line line)
    {
        // todo: add support for tags. at the start of line: [nc] (no camera), [ni] (no input)
        // if (line.text[0] == "[")
        //      there must be a tag. set variables here

        // Let the system know that we're running a line
        currentlyRunningLine = true;

        // Parse the friend from the line
        string friendName = line.text.Substring(0, line.text.IndexOf(": "));

        // Grab the handler for the UI side
        SpeechBubbleHandler speechBubbleHandler = playerSpeechHandler;

        // Show the bubble
        speechBubbleHandler.mainBubble.ShowBubble();

        speechBubbleHandler.mainBubble.GrowBubble();

        // Pull the contents
		string stringContents = line.text.Substring(line.text.IndexOf(": ") + 2);
        
        // Create the string using the Dialogue Util and add LineBreaks
        DialogueString dialogueString = new DialogueString(stringContents);
        dialogueString.AddLineBreaks(lineLength);

        speechBubbleHandler.mainBubble.SetSize((dialogueString.lineLength * letterWidth) + widthPadding, (dialogueString.noOfLines * letterHeight) + heightPadding);

        // Create objects
        float tempXLocation = 0f;
        float tempYLocation = 0f;
        foreach (DialogueCharacter dc in dialogueString.dialogue)
        {
            float delay = textSpeed;
            speechBubbleHandler.mainBubble.AddText(DialogueUtils.CreateTextObject(textPfb, dc, 
                                                                            speechBubbleHandler.mainBubble.transform, 
                                                                            new Vector2(tempXLocation, tempYLocation),  
                                                                            out delay));
            // Update X location
            tempXLocation += Global.dialogueHandler.letterWidth;

            // Add linebreak if necessary
            if (dc.isLineBreak)
            {
                // Update Y location
                tempYLocation -= Global.dialogueHandler.letterHeight;
                tempXLocation = 0f;
            }

            yield return new WaitForSeconds(delay * delayTimeMultiplier);
        }

        // Wait for talk input
        while (Global.input.GetButtonDown("Talk") == false) 
        {
            yield return null;
        }

        // Kill the text elements
        speechBubbleHandler.mainBubble.KillTextElements();
        speechBubbleHandler.mainBubble.ShrinkBubble();

        // Line is OVER
        currentlyRunningLine = false;
        delayTimeMultiplier = 1f;
    }

    /**
     *  RUN OPTIONS
     */
    public override IEnumerator RunOptions (Yarn.Options optionsCollection, 
                                            Yarn.OptionChooser optionChooser)
    {
        // todo: account for multiple lines
        // Find out the width of the longest option
        float longestOption = 0f;
        foreach (string optionString in optionsCollection.options)
        {
            if (optionString.Length > longestOption)
            {
                longestOption = optionString.Length;
            }
        }
        float longestWidth = (longestOption * letterWidth) + widthPadding;

        // Grab the handler for the UI side
        SpeechBubbleHandler friendSpeechHandler = playerSpeechHandler;

        List<SpeechBubbleImage> buttons = friendSpeechHandler.buttons;

        float offsetOption = ((letterHeight + heightPadding + optionOffset) * (optionsCollection.options.Count));

        offsetOption -= (letterHeight + heightPadding + optionOffset);

        yield return new WaitForSeconds(0.1f);

        // Display each option in a button, and make it visible
        int j = 0;
        foreach (var optionString in optionsCollection.options) 
        {
            if (j == 0)
                buttons[j].SelectButton(false);

            string finalString = optionString.ToUpper();

            // Grab the length of the contents
            int contentsLength = finalString.Length;

            float buttonWidth = (contentsLength * letterWidth) + widthPadding;
            
            // Resize/reposition 
            buttons[j].SetSizeAndOffset(buttonWidth, (letterHeight) + heightPadding, -((longestWidth-buttonWidth)/2f), offsetOption);
            buttons[j].ShowBubble();
            buttons[j].GrowBubble();
            
            float tempXLocation = 0f;
            float tempYLocation = 0f;

            foreach (char c in finalString)
            {
                GameObject go = Instantiate(textPfb, buttons[j].transform);
                Text textObject = go.GetComponent<Text>();
                textObject.rectTransform.anchoredPosition = new Vector2(tempXLocation, tempYLocation) + defaultInset;
                textObject.text = c.ToString();

                buttons[j].AddText(textObject);

                tempXLocation += letterWidth;
            }

            offsetOption -= (letterHeight + heightPadding + optionOffset);

            j++;

            yield return new WaitForSeconds(0.1f);
        }

        // Record that we're using it
        SetSelectedOption = optionChooser;

        int selected = 0;

        // Wait until the chooser has been used and then removed (see SetOption below)
        while (SetSelectedOption != null) 
        {
            if (Global.input.GetButtonDown("Talk"))
            {
                SetOption(selected);
            }
            else if (Global.input.GetButtonDown("UI|Up"))
            {
                buttons[selected].DeselectButton();

                if (selected > 0)
                {
                    selected--;
                }

                buttons[selected].SelectButton();
            }
            else if (Global.input.GetButtonDown("UI|Down"))
            {
                buttons[selected].DeselectButton();

                if (selected < optionsCollection.options.Count-1)
                {
                    selected++;
                }

                buttons[selected].SelectButton();
            }

            yield return null;
        }

        // Hide all the buttons
        foreach (var button in buttons) 
        {
            button.DeselectButton();
            button.KillTextElements();
            button.HideBubble();
        }

        yield break;
    }

    /// Called by buttons to make a selection.
    public void SetOption (int selectedOption)
    {
        // Call the delegate to tell the dialogue system that we've
        // selected an option.
        SetSelectedOption (selectedOption);

        // Now remove the delegate so that the loop in RunOptions will exit
        SetSelectedOption = null; 
    }

    /// Run an internal command.
    public override IEnumerator RunCommand (Yarn.Command command)
    {
        // "Perform" the command
        Debug.Log ("Command: " + command.text);

        yield break;
    }

    /// Called when the dialogue system has started running.
    public override IEnumerator DialogueStarted ()
    {
        Debug.Log ("Dialogue starting!");
        inDialogue = true;

        // Disable movement input during dialogue
        Global.input.controllers.maps.SetMapsEnabled(false, "Movement");

        yield break;
    }

    /// Called when the dialogue system has finished running.
    public override IEnumerator DialogueComplete ()
    {
        Debug.Log ("Complete!");
        inDialogue = false;

        // Re-enable movement input
        Global.input.controllers.maps.SetMapsEnabled(true, "Movement");

        yield break;
    }

}
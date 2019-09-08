using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System.Collections.Generic;

[System.Serializable]
public class SpecialName
{
    public string plantName;

    public string nodeName;
}

/** Dialogue Handler. */
public class DialogueHandler : Yarn.Unity.DialogueUIBehaviour
{
    /// A delegate (ie a function-stored-in-a-variable) that
    /// we call to tell the dialogue system about what option
    /// the user selected
    private Yarn.OptionChooser SetSelectedOption;

    public delegate void DialogueEnd();
    public DialogueEnd dialogueEnd;

    /** Prefabs */
    [Tooltip("Speech bubble prefab"), SerializeField] 
    private GameObject _speechBubble;
    
    [Tooltip("Option bubble prefab"), SerializeField] 
    private GameObject _optionBubble;

    [Tooltip("Prefab for the talk noise"), SerializeField] 
    private GameObject _talkNoise;

    /** Scene components */
    [Tooltip("The animator for the player"), SerializeField] 
    private Animator playerAnimator;

    [Tooltip("Script used for the thoughts"), SerializeField] 
    private TitleCanvas thoughtCanvas;

    [Tooltip("Yarn Dialogue Runner"), SerializeField] 
    private Yarn.Unity.DialogueRunner dialogueRunner;

    [Tooltip("The speech bubble handler for the player"), SerializeField] 
    private SpeechBubbleHandler playerSpeechHandler;

    /** Balancing variables */
    [SerializeField] private float timeBetweenBleeps;
    [SerializeField] private float speedyTextMultiplier = 0.3f;
    [SerializeField] private float interactionMenuOptionGap = 10f;
    [SerializeField] private float optionDelay = 0.1f;
    [SerializeField] private float cameraUpOffset = -2f;
    [SerializeField] private float offsetAddition = 0.1f;
    [SerializeField] private float screenShakeMultiplier = 0.6f;

    /** Actual publics */
    public bool inDialogue = false;
    public List<SpecialName> specialNames;

    /** Actual privates */
    private bool currentlyRunningLine = false;
    private float delayTimeMultiplier = 1f;

    void Update()
    {
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

        // Grab the speech bubble
        SpeechBubble speechBubble = speechBubbleHandler.CreateSpeechBubble();
        if (speechBubble == null)
        {
            Debug.LogError("Attempted to create SpeechBubble in DialogueHandler, but returned null.");

            yield break;
        }

        speechBubble.ShowBubble();
        speechBubble.GrowBubble();

        // Pull the contents
		string stringContents = line.text.Substring(line.text.IndexOf(": ") + 2);

        // Swap out plant name for the given plant name
        stringContents = stringContents.Replace("PLANTNAME", "[color=#10DF10]" + Global.plantName + "[/color]");

        string[] splitString = stringContents.Split('|'); 
        stringContents = splitString[0];

        string thoughtText = "";

        if (splitString.Length > 1)
        {
            thoughtText = splitString[1];
        }
        
        // Sets the contents of the speech bubble
        speechBubble.SetContents(stringContents);

        // Set time counter
        float timeCounter = 0.0f;

        foreach (LetterObject letterObject in speechBubble.text.GetLetterObjects())
        {
            // Show letter object
            letterObject.Show(true);

            // Set delay
            float delay = speechBubble.text.GetTextSpeed();

            if (letterObject.isActionCharacter)
            {
                // No delay by default
                delay = 0f;
                
                if (letterObject.screenShakeStrength > 0f)
                {
                    Global.cameraController.ScreenShake(screenShakeMultiplier * letterObject.screenShakeStrength);
                }
                else if (letterObject.delay > 0f)
                {
                    delay = letterObject.delay;
                }
            }
        
            // Do bleep if needed
            if (timeCounter >= timeBetweenBleeps)
            {
                if ((letterObject.character != '.' && letterObject.character != ' '))
                {
                    GameObject go = Instantiate(_talkNoise);
                    Destroy(go, 1.0f);
                    playerAnimator.CrossFadeInFixedTime("Talk", 0.05f);

                    timeCounter = 0.0f;
                }
            }

            timeCounter += (delay * delayTimeMultiplier);

            yield return new WaitForSeconds(delay * delayTimeMultiplier);
        }

        if (thoughtText.Length > 0)
        {
            yield return new WaitForSeconds(0.3f);

            delayTimeMultiplier = 1f;

            // Sets the contents of the speech bubble
            List<LetterObject> letterObjects = thoughtCanvas.SetText("[color=#101010][b][jittery]" + thoughtText);

            foreach (LetterObject letterObject in letterObjects)
            {
                // Show letter object
                letterObject.Show(true);

                // Set delay
                float delay = speechBubble.text.GetTextSpeed();

                if (letterObject.isActionCharacter)
                {
                    // No delay by default
                    delay = 0f;
                    
                    if (letterObject.screenShakeStrength > 0f)
                    {
                        Global.cameraController.ScreenShake(screenShakeMultiplier * letterObject.screenShakeStrength);
                    }
                    else if (letterObject.delay > 0f)
                    {
                        delay = letterObject.delay;
                    }
                }

                yield return new WaitForSeconds(delay * delayTimeMultiplier);
            }
        }

        // Wait for talk input
        while (Global.input.GetButtonDown("Talk") == false) 
        {
            yield return null;
        }

        if (thoughtText.Length > 0)
        {
            thoughtCanvas.FadeOut();
        }

        // Kill the text elements
        speechBubble.ShrinkBubble();

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
        List<SpeechBubble> optionButtons = new List<SpeechBubble>();

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

        float offsetOption = interactionMenuOptionGap * (optionsCollection.options.Count / 2);

        offsetOption -= interactionMenuOptionGap;

        yield return new WaitForSeconds(0.1f);

        // Display each option in a button, and make it visible
        int j = 0;
        foreach (var optionString in optionsCollection.options) 
        {
            GameObject go = Instantiate(_optionBubble, playerSpeechHandler.transform);
            SpeechBubble button = go.GetComponent<SpeechBubble>();

            button.SetHeight(offsetOption);

            if (j == 0)
                button.SelectButton(false);
            else
                button.DeselectButton();

            button.SetContents(optionString);

            // Grab the length of the contents
            int contentsLength = optionString.Length;

            button.ShowBubble();
            button.GrowBubble();

            optionButtons.Add(button);

            offsetOption -= interactionMenuOptionGap;

            j++;

            yield return new WaitForSeconds(0.1f);
        }

        int selected = 0;
        bool optionSelected = false;

        // Wait until the chooser has been used and then removed (see SetOption below)
        while (!optionSelected) 
        {
            if (Global.input.GetButtonDown("Talk"))
            {
                optionSelected = true;
            }
            else if (Global.input.GetButtonDown("UI|Up"))
            {
                optionButtons[selected].DeselectButton();

                if (selected > 0)
                {
                    selected--;
                }

                optionButtons[selected].SelectButton();
            }
            else if (Global.input.GetButtonDown("UI|Down"))
            {
                optionButtons[selected].DeselectButton();

                if (selected < optionsCollection.options.Count-1)
                {
                    selected++;
                }

                optionButtons[selected].SelectButton();
            }

            yield return null;
        }

        // Hide all the buttons
        foreach (var button in optionButtons) 
        {
            button.DeselectButton();
            button.ShrinkBubble();
        }

        optionButtons.Clear();

        optionChooser?.Invoke(selected);

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

        yield break;
    }

    /// Called when the dialogue system has finished running.
    public override IEnumerator DialogueComplete ()
    {
        Debug.Log ("Complete!");
        inDialogue = false;

        dialogueEnd?.Invoke();

        yield break;
    }

    public void StartDialogue(string nodeName)
    {
        dialogueRunner.StartDialogue(nodeName);
    }

}
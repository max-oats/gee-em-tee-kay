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

    public GameObject textPfb;
    public GameObject _speechBubble;
    public GameObject _optionBubble;
    public GameObject _audioSource;
    public Animator playerAnimator;

    /// How quickly to show the text, in seconds per character
    [Tooltip("How quickly to show the text, in seconds per character")]
    public float textSpeed = 0.025f;

    public float timeBetweenBleeps;

    public float speedyTextMultiplier = 0.3f;

    public Vector2 defaultInset = new Vector2(100f, -100f);

    public float interactionMenuOptionGap = 10f;

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

    public List<SpecialName> specialNames;

    [SerializeField] private Yarn.Unity.DialogueRunner dialogueRunner;

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
        stringContents = stringContents.Replace("PLANTNAME", "\\c002" + Global.plantName + "\\c000");
        
        // Sets the contents of the speech bubble
        speechBubble.SetContents(stringContents);

        // Set time counter
        float timeCounter = 0.0f;

        foreach (LetterObject lo in speechBubble.text.GetLetterObjects())
        {
            float delay = textSpeed;

            // Show letter object
            lo.Show(true);

            // Set delay
            delay = lo.postDelay + speechBubble.text.GetTextSpeed();
        
            // Do bleep if needed
            if (timeCounter >= timeBetweenBleeps)
            {
                if ((lo.character != '.' && lo.character != ' '))
                {
                    GameObject go = Instantiate(_audioSource);
                    Destroy(go, 1.0f);
                    playerAnimator.CrossFadeInFixedTime("Talk", 0.05f);

                    timeCounter = 0.0f;
                }
            }

            timeCounter += (delay * delayTimeMultiplier);

            yield return new WaitForSeconds(delay * delayTimeMultiplier);
        }

        // Wait for talk input
        while (Global.input.GetButtonDown("Talk") == false) 
        {
            yield return null;
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

        float offsetOption = interactionMenuOptionGap * optionsCollection.options.Count;

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

            offsetOption -= (Global.dialogueHandler.letterHeight + Global.dialogueHandler.heightPadding + Global.dialogueHandler.optionOffset);

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
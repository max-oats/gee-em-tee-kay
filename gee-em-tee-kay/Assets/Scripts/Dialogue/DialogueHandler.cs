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
    // When true, the DialogueRunner is waiting for the user to
    // indicate that they want to proceed to the next line.
    private bool waitingForLineContinue = false;

    // When true, the DialogueRunner is waiting for the user to press
    // one of the option buttons.
    private bool waitingForOptionSelection = false;      

    public UnityEngine.Events.UnityEvent onDialogueStart;

    public UnityEngine.Events.UnityEvent onDialogueEnd;  

    public UnityEngine.Events.UnityEvent onLineStart;
    public UnityEngine.Events.UnityEvent onLineFinishDisplaying;
    public Yarn.Unity.DialogueRunner.StringUnityEvent onLineUpdate;
    public UnityEngine.Events.UnityEvent onLineEnd;

    public UnityEngine.Events.UnityEvent onOptionsStart;
    public UnityEngine.Events.UnityEvent onOptionsEnd;

    public Yarn.Unity.DialogueRunner.StringUnityEvent onCommand;

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

    public override Yarn.Dialogue.HandlerExecutionType RunLine (Yarn.Line line, IDictionary<string,string> strings, System.Action onComplete)
    {
        // Start displaying the line; it will call onComplete later
        // which will tell the dialogue to continue
        StartCoroutine(DoRunLine(line, strings, onComplete));
        return Yarn.Dialogue.HandlerExecutionType.PauseExecution;
    }

    /// Show a line of dialogue, gradually
    private IEnumerator DoRunLine(Yarn.Line line, IDictionary<string,string> strings, System.Action onComplete)
    {
        // Let the system know that we're running a line
        currentlyRunningLine = true;

        if (strings.TryGetValue(line.ID, out var text) == false) 
        {
            Debug.LogWarning($"Line {line.ID} doesn't have any localised text.");
            text = line.ID;
        }

        // Parse the friend from the line
        string friendName = text.Substring(0, text.IndexOf(": "));

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
        string stringContents = text.Substring(text.IndexOf(":") + 1).Trim();

        // Swap out plant name for the given plant name
        stringContents = CommandUtils.FillInTags(stringContents);

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
            List<LetterObject> letterObjects = thoughtCanvas.SetText("[color=!101010][b][jittery]" + thoughtText);

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

        waitingForLineContinue = true;

        onLineFinishDisplaying?.Invoke();

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

        // Avoid skipping lines if textSpeed == 0
        yield return new WaitForEndOfFrame();

        // Hide the text and prompt
        onLineEnd?.Invoke();

        onComplete();
    }

        /**
     *  RUN OPTIONS
     */
    public override void RunOptions (Yarn.OptionSet optionsCollection, IDictionary<string,string> strings, System.Action<int> selectOption) 
    {
        StartCoroutine(DoRunOptions(optionsCollection, strings, selectOption));
    }
    
    private IEnumerator DoRunOptions (Yarn.OptionSet optionsCollection, IDictionary<string,string> strings, System.Action<int> selectOption)
    {
        List<SpeechBubble> optionButtons = new List<SpeechBubble>();

        // todo: account for multiple lines
        // Find out the width of the longest option
        float longestOption = 0f;

        foreach (var optionString in optionsCollection.Options)
        {
            if (strings.TryGetValue(optionString.Line.ID, out var optionText) == false) 
            {
                Debug.LogWarning($"Option {optionString.Line.ID} doesn't have any localised text");
                optionText = optionString.Line.ID;
            }

            if (optionText.Length > longestOption)
            {
                longestOption = optionText.Length;
            }
        }

        float offsetOption = interactionMenuOptionGap * (optionsCollection.Options.Length / 2);

        offsetOption -= interactionMenuOptionGap;

        yield return new WaitForSeconds(0.1f);

        // Display each option in a button, and make it visible
        int j = 0;
        foreach (var optionString in optionsCollection.Options)
        {
            GameObject go = Instantiate(_optionBubble, playerSpeechHandler.transform);
            SpeechBubble button = go.GetComponent<SpeechBubble>();

            button.SetHeight(offsetOption);

            if (j == 0)
            {
                button.SelectButton(false);
            }
            else
            {
                button.DeselectButton();
            }

            if (strings.TryGetValue(optionString.Line.ID, out var optionText) == false) 
            {
                Debug.LogWarning($"Option {optionString.Line.ID} doesn't have any localised text");
                optionText = optionString.Line.ID;
            }

            button.SetContents(optionText);

            // Grab the length of the contents
            int contentsLength = optionText.Length;

            button.ShowBubble();
            button.GrowBubble();

            optionButtons.Add(button);

            offsetOption -= interactionMenuOptionGap;

            j++;

            yield return new WaitForSeconds(0.1f);
        }

        int selected = 0;

        // Wait until the chooser has been used and then removed (see SetOption below)
        while (waitingForOptionSelection)
        {
            if (Global.input.GetButtonDown("Talk"))
            {
                waitingForOptionSelection = false;
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

                if (selected < optionsCollection.Options.Length - 1)
                {
                    selected++;
                }

                optionButtons[selected].SelectButton();
            }

            yield return null;
        }

        selectOption(optionsCollection.Options[selected].ID);
        print("selected option: " + optionsCollection.Options[selected].ID);

        // Hide all the buttons
        foreach (var button in optionButtons)
        {
            button.DeselectButton();
            button.ShrinkBubble();
        }

        optionButtons.Clear();

        // Avoid skipping lines if textSpeed == 0
        yield return new WaitForEndOfFrame();
        
        onOptionsEnd?.Invoke();

        yield break;
    }

    /// Run an internal command.
    public override Yarn.Dialogue.HandlerExecutionType RunCommand (Yarn.Command command, System.Action onComplete) 
    {
        StartCoroutine(DoRunCommand(command, onComplete));

        return Yarn.Dialogue.HandlerExecutionType.PauseExecution;
    }

    public IEnumerator DoRunCommand (Yarn.Command command, System.Action onComplete)
    {
        yield return StartCoroutine(CommandUtils.Dispatch(command.Text, this));

        onComplete();
    }

    /// Called when the dialogue system has started running.
    public override void DialogueStarted()
    {
        Debug.Log ("Dialogue starting!");
        
        // Set variable to true
        inDialogue = true;

        // Send to any listeners
        onDialogueStart?.Invoke();
    }

    /// Called when the dialogue system has finished running.
    public override void DialogueComplete()
    {
        Debug.Log("Complete!");

        // Set variable to false
        inDialogue = false;

        // Send to any listeners
        onDialogueEnd?.Invoke();
    }

    public void StartDialogue(string nodeName)
    {
        dialogueRunner.StartDialogue(nodeName);
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionComponent : MonoBehaviour
{
    public delegate void SelectedMenuOption(string option);
    public SelectedMenuOption selectedMenuOption;

    /** Serialized fields */
    [SerializeField] private Transform canvas;
    [SerializeField] private GameObject _speechBubble;
    [SerializeField] private Collider theCollider;
    [SerializeField] private float interactionMenuOptionGap;

    /** Non-inspector publics */
    [HideInInspector] public bool isAbleToInteract = false;

    public List<string> menuOptions;

    // ~Begin Debug
    [SerializeField] private bool ableToSleepWithoutTalking;
    // ~End Debug

    private SpeechBubble speechBubble = null;
    private List<SpeechBubble> optionButtons = new List<SpeechBubble>();

    void ShowInteract()
    {
        speechBubble = Instantiate(_speechBubble, canvas).GetComponent<SpeechBubble>();
        speechBubble.SetContents("interact");
        
        speechBubble.ShowBubble();
        speechBubble.GrowBubble();
    }

    void HideInteract()
    {
        speechBubble.ShrinkBubble();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.gameObject.tag != "PlantPot")
        {
            return;
        }
        
        if (!Global.dialogueHandler.inDialogue)
        {
            isAbleToInteract = true;

            ShowInteract();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.parent.gameObject.tag != "PlantPot")
        {
            return;
        }

        if (isAbleToInteract && !Global.dialogueHandler.inDialogue)
        {
            isAbleToInteract = false;

            HideInteract();
        }
    }

    public void SetUpMenu(List<string> options)
    {
        if (options == null)
        {
            options = new List<string>(menuOptions);

            // Remove talk option if have talked today
            if (Global.plantHealthData.HaveConversedToday())
            {
                options.RemoveAt(0);
            }
            else
            {
                if (!ableToSleepWithoutTalking)
                {
                    options.RemoveAt(options.Count-1);
                }
            }
        }

        HideInteract();
        isAbleToInteract = false;

        StartCoroutine(SetUpBubbles(options));
    }

    public void NamePlant()
    {
        StartCoroutine(NamePlantCoroutine());
    }

    public IEnumerator NamePlantCoroutine()
    {
        GameObject go = Instantiate(_speechBubble, canvas);
        SpeechBubble nameBubble = go.GetComponent<SpeechBubble>();
        
        string interactString = "";
        int maxLength = 30;

        float carriageReturnSpeed = 0.5f;
        float carriageReturnCounter = carriageReturnSpeed;
        bool carriageReturnOnOff = false;

        nameBubble.ShowBubble();
        nameBubble.GrowBubble();

        bool plantNamed = false;
        string previousString = "";

        nameBubble.SetContents(interactString);

        while (!plantNamed)
        {
            foreach (char c in System.Text.RegularExpressions.Regex.Replace(Input.inputString, @"[^A-Za-z0-9 ]+", ""))
            {
                if (interactString.Length < maxLength)
                {
                    interactString += c;
                }
            }

            interactString = interactString.ToLower();

            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (interactString.Length > 0)
                {
                    plantNamed = true;
                    Global.plantName = interactString;
                    Global.plantManager.CreatePlant(interactString.GetHashCode());
                }
            }
            else if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (interactString.Length > 0)
                    interactString = interactString.Remove(interactString.Length-1);
            }


            if (!previousString.Equals(interactString))
            {
                nameBubble.BumpButton();

                string finalString = interactString;

                if (interactString.Length < maxLength)
                {
                    if (carriageReturnOnOff)
                    {
                        finalString += "_";
                    }
                    else
                    {
                        finalString += " ";
                    }
                }

                nameBubble.SetContents(finalString);
            }

            previousString = interactString;

            carriageReturnCounter += Time.deltaTime;
            if (carriageReturnCounter > carriageReturnSpeed)
            {
                carriageReturnCounter = 0f;

                string finalString = interactString;

                if (interactString.Length < maxLength)
                {
                    if (carriageReturnOnOff)
                    {
                        finalString += "_";
                    }
                    else
                    {
                        finalString += " ";
                    }
                }

                nameBubble.SetContents(finalString);

                carriageReturnOnOff = !carriageReturnOnOff;
            }

            yield return null;
        }

        Global.input.controllers.maps.SetMapsEnabled(true, "Movement");

        // speechBubble.KillTextElements();
        nameBubble.ShrinkBubble();

        string nodeName = "Day1.NamePlant";

        Global.AddName();

        foreach (SpecialName sn in Global.dialogueHandler.specialNames)
        {
            if (Global.plantName == sn.plantName)
            {
                nodeName += "." + sn.nodeName;
            }
        }

        Global.dialogueHandler.StartDialogue(nodeName);
    }

    public void BumpCollider()
    {
        theCollider.enabled = false;
        StartCoroutine(BumpColliderCoroutine());
    }

    IEnumerator BumpColliderCoroutine()
    {
        yield return new WaitForSeconds(0.05f);

        theCollider.enabled = true;
    }

    IEnumerator SetUpBubbles(List<string> options)
    {
        // Disable movement input during dialogue
        Global.input.controllers.maps.SetMapsEnabled(false, "Movement");

        // todo: account for multiple lines
        // Find out the width of the longest option
        float longestOption = 0f;

        foreach (string optionString in options)
        {
            if (optionString.Length > longestOption)
            {
                longestOption = optionString.Length;
            }
        }

        float offsetOption = interactionMenuOptionGap * (options.Count / 2);

        offsetOption -= interactionMenuOptionGap;

        yield return new WaitForSeconds(0.1f);

        // Display each option in a button, and make it visible
        int j = 0;
        foreach (var optionString in options) 
        {
            GameObject go = Instantiate(_speechBubble, canvas);
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
                if (selectedMenuOption != null)
                {
                    optionSelected = true;
                }
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

                if (selected < options.Count-1)
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
        
        // Re-enable movement input
        Global.input.controllers.maps.SetMapsEnabled(true, "Movement");

        selectedMenuOption(options[selected]);
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionComponent : MonoBehaviour
{
    public delegate void SelectedMenuOption(string option);
    public SelectedMenuOption selectedMenuOption;

    public bool bIsAbleToInteract = false;

    public List<string> menuOptions;

    private SpeechBubbleImage speechBubble = null;

    void Start()
    {
        speechBubble = Instantiate(Global.dialogueHandler.speechBubPfb, Global.dialogueHandler.playerSpeechHandler.transform).GetComponent<SpeechBubbleImage>();
    }

    void ShowInteract()
    {
        string interactString = "interact";
        speechBubble.SetSize((interactString.Length * Global.dialogueHandler.letterWidth) + Global.dialogueHandler.widthPadding*2f, 
                    (Global.dialogueHandler.letterHeight) + Global.dialogueHandler.heightPadding*2f);

        // Create objects
        float tempXLocation = Global.dialogueHandler.defaultInset.x;
        float tempYLocation = Global.dialogueHandler.defaultInset.y;
        foreach (char c in interactString)
        {
            DialogueCharacter dc = new DialogueCharacter();
            dc.character = c;

            float delay = 0.0f;
            speechBubble.AddText(DialogueUtils.CreateTextObject(Global.dialogueHandler.textPfb, dc, 
                                                                            speechBubble.transform, 
                                                                            new Vector2(tempXLocation, tempYLocation),  
                                                                            out delay));
            // Update X location
            tempXLocation += Global.dialogueHandler.letterWidth;

            // Add linebreak if necessary
            if (dc.isLineBreak)
            {
                // Update Y location
                tempYLocation -= Global.dialogueHandler.letterHeight;
                tempXLocation = Global.dialogueHandler.defaultInset.x;
            }
        }
        
        speechBubble.ShowBubble();
        speechBubble.GrowBubble();
    }

    void HideInteract()
    {
        speechBubble.KillTextElements();
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
            bIsAbleToInteract = true;

            ShowInteract();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.parent.gameObject.tag != "PlantPot")
        {
            return;
        }

        if (bIsAbleToInteract && !Global.dialogueHandler.inDialogue)
        {
            bIsAbleToInteract = false;

            HideInteract();
        }
    }

    public void SetUpMenu()
    {
        StartCoroutine(SetUpBubbles());
    }

    IEnumerator SetUpBubbles()
    {
        // Disable movement input during dialogue
        Global.input.controllers.maps.SetMapsEnabled(false, "Movement");

        // todo: account for multiple lines
        // Find out the width of the longest option
        float longestOption = 0f;
        foreach (string optionString in menuOptions)
        {
            if (optionString.Length > longestOption)
            {
                longestOption = optionString.Length;
            }
        }
        float longestWidth = (longestOption * Global.dialogueHandler.letterWidth) + (Global.dialogueHandler.widthPadding*2f);

        // Grab the handler for the UI side
        SpeechBubbleHandler friendSpeechHandler = Global.dialogueHandler.playerSpeechHandler;

        float offsetOption = ((Global.dialogueHandler.letterHeight + (Global.dialogueHandler.heightPadding*2f) + Global.dialogueHandler.optionOffset) * (menuOptions.Count));

        offsetOption -= (Global.dialogueHandler.letterHeight + (Global.dialogueHandler.heightPadding*2f) + Global.dialogueHandler.optionOffset);

        yield return new WaitForSeconds(0.1f);

        // Display each option in a button, and make it visible
        int j = 0;
        foreach (var optionString in menuOptions) 
        {
            GameObject speechgo = Instantiate(Global.dialogueHandler.speechBubPfb, friendSpeechHandler.transform);
            SpeechBubbleImage button = speechgo.GetComponent<SpeechBubbleImage>();

            friendSpeechHandler.buttons.Add(button);

            if (j == 0)
                button.SelectButton(false);
            else
                button.DeselectButton();

            string finalString = optionString.ToLower();

            // Grab the length of the contents
            int contentsLength = finalString.Length;

            float buttonWidth = (contentsLength * Global.dialogueHandler.letterWidth) + Global.dialogueHandler.widthPadding*2f;

            // Resize/reposition 
            button.SetSizeAndOffset(buttonWidth, (Global.dialogueHandler.letterHeight) + Global.dialogueHandler.heightPadding*2f, -((longestWidth-buttonWidth)/2f), offsetOption);
            button.ShowBubble();
            button.GrowBubble();
            
            float tempXLocation = Global.dialogueHandler.defaultInset.x;
            float tempYLocation = Global.dialogueHandler.defaultInset.y;

            foreach (char c in finalString)
            {
                DialogueCharacter dc = new DialogueCharacter();
                dc.character = c;

                float delay = 0f;
                button.AddText(DialogueUtils.CreateTextObject(Global.dialogueHandler.textPfb, dc, 
                                                                            button.transform, 
                                                                            new Vector2(tempXLocation, tempYLocation),  
                                                                            out delay));
                // Update X location
                tempXLocation += Global.dialogueHandler.letterWidth;
            }

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
                if (selectedMenuOption != null)
                {
                    selectedMenuOption(menuOptions[selected]);

                    optionSelected = true;
                }
            }
            else if (Global.input.GetButtonDown("UI|Up"))
            {
                friendSpeechHandler.buttons[selected].DeselectButton();

                if (selected > 0)
                {
                    selected--;
                }

                friendSpeechHandler.buttons[selected].SelectButton();
            }
            else if (Global.input.GetButtonDown("UI|Down"))
            {
                friendSpeechHandler.buttons[selected].DeselectButton();

                if (selected < menuOptions.Count-1)
                {
                    selected++;
                }

                friendSpeechHandler.buttons[selected].SelectButton();
            }

            yield return null;
        }
        
        // Re-enable movement input
        Global.input.controllers.maps.SetMapsEnabled(true, "Movement");
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionComponent : MonoBehaviour
{
    public bool bIsAbleToInteract = false;

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

    // IEnumerator SetUpBubble()
    // {
    //     // todo: account for multiple lines
    //     // Find out the width of the longest option
    //     float longestOption = 0f;
    //     foreach (string optionString in optionsCollection.options)
    //     {
    //         if (optionString.Length > longestOption)
    //         {
    //             longestOption = optionString.Length;
    //         }
    //     }
    //     float longestWidth = (longestOption * letterWidth) + (widthPadding*2f);

    //     // Grab the handler for the UI side
    //     SpeechBubbleHandler friendSpeechHandler = playerSpeechHandler;

    //     float offsetOption = ((letterHeight + (heightPadding*2f) + optionOffset) * (optionsCollection.options.Count));

    //     offsetOption -= (letterHeight + (heightPadding*2f) + optionOffset);

    //     yield return new WaitForSeconds(0.1f);

    //     // Display each option in a button, and make it visible
    //     int j = 0;
    //     foreach (var optionString in optionsCollection.options) 
    //     {
    //         GameObject speechgo = Instantiate(speechBubPfb, friendSpeechHandler.transform);
    //         SpeechBubbleImage button = speechgo.GetComponent<SpeechBubbleImage>();

    //         friendSpeechHandler.buttons.Add(button);

    //         if (j == 0)
    //             button.SelectButton(false);
    //         else
    //             button.DeselectButton();

    //         string finalString = optionString.ToLower();

    //         // Grab the length of the contents
    //         int contentsLength = finalString.Length;

    //         float buttonWidth = (contentsLength * letterWidth) + widthPadding*2f;

    //         // Resize/reposition 
    //         button.SetSizeAndOffset(buttonWidth, (letterHeight) + heightPadding*2f, -((longestWidth-buttonWidth)/2f), offsetOption);
    //         button.ShowBubble();
    //         button.GrowBubble();
            
    //         float tempXLocation = Global.dialogueHandler.defaultInset.x;
    //         float tempYLocation = Global.dialogueHandler.defaultInset.y;

    //         foreach (char c in finalString)
    //         {
    //             DialogueCharacter dc = new DialogueCharacter();
    //             dc.character = c;

    //             float delay = 0f;
    //             button.AddText(DialogueUtils.CreateTextObject(textPfb, dc, 
    //                                                                         button.transform, 
    //                                                                         new Vector2(tempXLocation, tempYLocation),  
    //                                                                         out delay));
    //             // Update X location
    //             tempXLocation += Global.dialogueHandler.letterWidth;
    //         }

    //         offsetOption -= (letterHeight + heightPadding + optionOffset);

    //         j++;

    //         yield return new WaitForSeconds(0.1f);
    //     }

    //     // Record that we're using it
    //     SetSelectedOption = optionChooser;

    //     int selected = 0;

    //     // Wait until the chooser has been used and then removed (see SetOption below)
    //     while (SetSelectedOption != null) 
    //     {
    //         if (Global.input.GetButtonDown("Talk"))
    //         {
    //             SetOption(selected);
    //         }
    //         else if (Global.input.GetButtonDown("UI|Up"))
    //         {
    //             friendSpeechHandler.buttons[selected].DeselectButton();

    //             if (selected > 0)
    //             {
    //                 selected--;
    //             }

    //             friendSpeechHandler.buttons[selected].SelectButton();
    //         }
    //         else if (Global.input.GetButtonDown("UI|Down"))
    //         {
    //             friendSpeechHandler.buttons[selected].DeselectButton();

    //             if (selected < optionsCollection.options.Count-1)
    //             {
    //                 selected++;
    //             }

    //             friendSpeechHandler.buttons[selected].SelectButton();
    //         }

    //         yield return null;
    //     }
    // }
}

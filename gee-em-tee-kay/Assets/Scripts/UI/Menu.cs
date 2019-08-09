using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public delegate void SelectedMenuOption(string selectedOption);
    public SelectedMenuOption onMenuOptionSelected;

    [SerializeField] private List<string> inputMapsToDisable;
    [SerializeField] private List<string> inputMapsToEnable;
    [SerializeField] private SpeechBubbleHandler bubbleHandler;

    public void OpenMenu(List<string> options)
    {
        StartCoroutine(SetUpBubbles(options));
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
        float longestWidth = (longestOption * Global.dialogueHandler.letterWidth) + (Global.dialogueHandler.widthPadding*2f);

        float offsetOption = ((Global.dialogueHandler.letterHeight + (Global.dialogueHandler.heightPadding*2f) - Global.dialogueHandler.optionOffset) * (options.Count));

        offsetOption -= (Global.dialogueHandler.letterHeight + (Global.dialogueHandler.heightPadding*2f) + Global.dialogueHandler.optionOffset);

        yield return new WaitForSeconds(0.1f);

        // Display each option in a button, and make it visible
        int j = 0;
        foreach (var optionString in options) 
        {
            GameObject speechgo = Instantiate(Global.dialogueHandler.speechBubPfb, bubbleHandler.transform);
            SpeechBubbleImage button = speechgo.GetComponent<SpeechBubbleImage>();

            bubbleHandler.buttons.Add(button);

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
                optionSelected = true;
            }
            else if (Global.input.GetButtonDown("UI|Up"))
            {
                bubbleHandler.buttons[selected].DeselectButton();

                if (selected > 0)
                {
                    selected--;
                }

                bubbleHandler.buttons[selected].SelectButton();
            }
            else if (Global.input.GetButtonDown("UI|Down"))
            {
                bubbleHandler.buttons[selected].DeselectButton();

                if (selected < options.Count-1)
                {
                    selected++;
                }

                bubbleHandler.buttons[selected].SelectButton();
            }

            yield return null;
        }

        // Hide all the buttons
        foreach (var button in bubbleHandler.buttons) 
        {
            button.DeselectButton();
            button.KillTextElements();
            button.ShrinkBubble();
            Destroy(button, 1.0f);
        }

        bubbleHandler.buttons.Clear();
        
        // Re-enable movement input
        Global.input.controllers.maps.SetMapsEnabled(true, "Movement");

        onMenuOptionSelected?.Invoke(options[selected]);
        
    }
}
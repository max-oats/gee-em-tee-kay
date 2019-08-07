using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /** ----- Balancing variables ----- */
    public float groundSpeed; /** The ground movement speed */
    public float carryingSpeed; /** The carrying movement speed */

    public SmoothDamper rotationSmoother;
    public SmoothDamper stompVelocitySmoother;

    public ParticleSystem sweatParticles;

    public GameObject stompParticle;
    public GameObject stompSound;

    public GameObject waterBottle;

    /** ----- Private variables ----- */
    private Vector3 localInput;

    // Caches
    private Entity entity;
    private Animator animator;
    private bool bIsCarrying = false;
    private InteractionComponent interactionComponent;
    private Holder holder;

    void Awake()
    {
        entity = GetComponent<Entity>();
        animator = GetComponentInChildren<Animator>();
        interactionComponent = GetComponent<InteractionComponent>();
        interactionComponent.selectedMenuOption += SelectedMenuOption;
        holder = GetComponent<Holder>();
        Global.dialogueHandler.dialogueEnd += DialogueEnd;
        Global.dayManager.dayStarted += ResetOnDay;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInput();

        UpdateMovement();

        UpdateInteract();

        UpdateFacing();
    }

    public void SetStompDesiredVelocity(float desiredVeloity)
    {
        stompVelocitySmoother.SetDesired(desiredVeloity);
    }

    public void ResetOnDay(int dayNo)
    {
        rotationSmoother.SetDesired(0);
        transform.position = new Vector3(0, -1.14f, -5.7f);

        animator.CrossFadeInFixedTime("IdleWalk", 0f);

        if (dayNo == 0 && !Global.debug.skipIntros)
        {
            FindObjectOfType<Yarn.Unity.DialogueRunner>().StartDialogue("Day1.Intro");
        }
        else if (dayNo == 4 && !Global.debug.skipIntros)
        {
            FindObjectOfType<Yarn.Unity.DialogueRunner>().StartDialogue("Day5.Intro");
        }

    }

    private void UpdateInteract()
    {
        if (Global.input.GetButtonDown("Interact") && !Global.dialogueHandler.inDialogue)
        {
            if (bIsCarrying)
            {
                PlacePot();
                SetCarrying(false);
            }
            else if (interactionComponent.bIsAbleToInteract)
            {
                Interact();
            }
        }
    }

    public void DialogueEnd()
    {
        interactionComponent.BumpCollider();
    }

    [Yarn.Unity.YarnCommand("phoneOn")]
    public void SwitchPhoneOn()
    {
        animator.CrossFadeInFixedTime("IdlePhone", 0.5f);
    }

    [Yarn.Unity.YarnCommand("phoneOff")]
    public void SwitchPhoneOff()
    {
        animator.CrossFadeInFixedTime("IdleWalk", 0.5f);
    }

    public IEnumerator Water()
    {
        Global.input.controllers.maps.SetMapsEnabled(false, "Movement");

        yield return new WaitForSeconds(0.1f);
        waterBottle.SetActive(true);

        animator.CrossFadeInFixedTime("Water", 0.1f);

        yield return new WaitForSeconds(2.0f);

        waterBottle.SetActive(false);

        Global.input.controllers.maps.SetMapsEnabled(true, "Movement");

        animator.CrossFadeInFixedTime("IdleWalk", 0.1f);

        interactionComponent.BumpCollider();
    }

    public void SelectedMenuOption(string selectedMenuOption)
    {
        if (selectedMenuOption == "talk")
        {
            PlantHealthData data = Global.plantHealthData;

            if (!Global.debug.skipDailyDialogue)
            {
                FindObjectOfType<Yarn.Unity.DialogueRunner>().StartDialogue(data.SelectDialogueNode());
            }
            data.Talk();
        }
        else if (selectedMenuOption == "water")
        {
            StartCoroutine(Water());
            Global.plantHealthData.Water();
        }
        else if (selectedMenuOption == "move")
        {
            SetCarrying(true);
        }
        else if (selectedMenuOption == "neglect")
        {
            interactionComponent.BumpCollider();
        }
        else if (selectedMenuOption == "bedtime")
        {
            // Re-enable movement input
            Global.input.controllers.maps.SetMapsEnabled(false, "Movement");
            rotationSmoother.SetDesired(180f);
            animator.CrossFadeInFixedTime("Sleep", 0.2f);

            Global.dayManager.EndDay();
        }
        else if (selectedMenuOption == "name plant")
        {
            Global.input.controllers.maps.SetMapsEnabled(false, "Movement");
            Global.dayManager.seedPlanted = true;

            StartCoroutine(NamePlant());
        }
    }

    public IEnumerator NamePlant()
    {
        GameObject go = Instantiate(Global.dialogueHandler.speechBubPfb, Global.dialogueHandler.playerSpeechHandler.transform);
        SpeechBubbleImage speechBubble = go.GetComponent<SpeechBubbleImage>();
        string interactString = "";
        int maxLength = 18;
        speechBubble.SetSize((maxLength * Global.dialogueHandler.letterWidth) + Global.dialogueHandler.widthPadding*2f,
                    (Global.dialogueHandler.letterHeight) + Global.dialogueHandler.heightPadding*2f);

        speechBubble.ShowBubble();
        speechBubble.GrowBubble();

        bool plantNamed = false;
        while (!plantNamed)
        {
            speechBubble.KillTextElements();

            foreach (char c in System.Text.RegularExpressions.Regex.Replace(Input.inputString, @"[^A-Za-z0-9 ]+", ""))
            {
                if (interactString.Length < maxLength)
                {
                    interactString += c;
                }
            }

            interactString = interactString.ToLower();

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

            yield return null;
        }

        Global.input.controllers.maps.SetMapsEnabled(true, "Movement");

        speechBubble.KillTextElements();
        speechBubble.ShrinkBubble();
        Destroy(speechBubble.gameObject, 1.0f);

        string nodeName = "Day1.NamePlant";

        foreach (SpecialName sn in Global.dialogueHandler.specialNames)
        {
            if (Global.plantName == sn.plantName)
            {
                nodeName += "." + sn.nodeName;
            }
        }

        FindObjectOfType<Yarn.Unity.DialogueRunner>().StartDialogue(nodeName);
    }

    private void Interact()
    {
        if (!Global.dayManager.seedPlanted)
        {
            List<string> strings = new List<string>();
            strings.Add("name plant");
            interactionComponent.SetUpMenu(strings);
        }
        else
        {
            interactionComponent.SetUpMenu(null);
        }
    }

    private void PlacePot()
    {
        float minDistance = 10000f;
        Vector3 potPosition = holder.heldObject.transform.position;

        Vector3 finalPos = Vector3.zero;
        PotPosition finalPosition = new PotPosition();
        foreach (PotPosition pP in Global.potPositionHolder.potPositions)
        {
            Vector3 v3 = pP.position;
            float checkDistance = (potPosition - v3).magnitude;

            if (checkDistance < minDistance)
            {
                minDistance = checkDistance;
                finalPos = v3;
                finalPosition = pP;
            }
        }

        holder.Drop(finalPos);

        GameObject go = Instantiate(stompParticle, finalPos, Quaternion.identity);
        Destroy(go, 1.0f);

        GameObject goaudio = Instantiate(stompSound);
        Destroy(goaudio, 1.0f);

        Global.cameraController.ScreenShake(0.2f);
        Global.plantHealthData.SetLightIncrementForToday(finalPosition.lightGainedHere);

        interactionComponent.BumpCollider();
    }

    private void SetCarrying(bool onOff)
    {
        bIsCarrying = onOff;

        if (bIsCarrying)
        {
            holder.heldObject = GameObject.Find("Plant Pot");

            animator.CrossFadeInFixedTime("HeavyWalk", 0.1f);
            ParticleSystem.EmissionModule em = sweatParticles.emission;
            em.enabled = true;
        }
        else
        {
            animator.CrossFadeInFixedTime("IdleWalk", 0.1f);
            ParticleSystem.EmissionModule em = sweatParticles.emission;
            em.enabled = false;
        }
    }

    private void UpdateInput()
    {
        // Create input vector
        Vector3 input = new Vector3(Global.input.GetAxis("Horizontal"), 0.0f, Global.input.GetAxis("Vertical"));

        // Normalize magnitude
        localInput = Vector3.ClampMagnitude(input, 1.0f);

        animator.SetFloat("Move.Velocity", localInput.magnitude);
    }

    private void UpdateMovement()
    {
        // Create position vectors
        Vector3 fromPos = transform.position;
        Vector3 toPos = fromPos;

        // Calculate horizontal movement vector
        Vector3 move;
        if (!bIsCarrying)
        {
            move = Time.deltaTime * groundSpeed * localInput;
        }
        else
        {
            float currentSpeed = stompVelocitySmoother.Smooth();
            move = Time.deltaTime * currentSpeed * localInput;
        }

        /** ----- HORIZONTAL ----- */
        // If there is a movement input, attempt to move
        if (move != Vector3.zero)
        {
            // Move while sliding up slopes/stairs and on XZ around obstructions
            entity.SlideMove(move, fromPos, ref toPos);
        }

        // Actually set position now that it's tested
        transform.position = toPos;
    }

    private void UpdateFacing()
    {
        // Update desired angle
        if (localInput.magnitude > 0.05f)
        {
            rotationSmoother.SetDesired(Vector3.SignedAngle(Vector3.forward, localInput, Vector3.up));
        }

        // Rotate player
        Vector3 eulerAngles = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(eulerAngles.x, rotationSmoother.Smooth(), eulerAngles.z);
    }
}

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

    [SerializeField] private GameObject phone;

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
    }

    void Start()
    {
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

        if ((dayNo == 4 || Global.debug.dayToPlayDialogueFor_1Indexed == 5) && !Global.debug.skipIntroDialogue)
        {
            Global.dialogueHandler.StartDialogue("Day5.Intro");
        }
        else if (dayNo == 0 && !Global.debug.skipIntroDialogue)
        {
            Global.dialogueHandler.StartDialogue("Day1.Intro");
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
            else if (interactionComponent.isAbleToInteract)
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

        StartCoroutine(AttachPhone(true));
    }

    [Yarn.Unity.YarnCommand("phoneOff")]
    public void SwitchPhoneOff()
    {
        animator.CrossFadeInFixedTime("IdleWalk", 0.5f);

        StartCoroutine(AttachPhone(false));
    }

    public IEnumerator AttachPhone(bool onOff)
    {
        yield return new WaitForSeconds(0.25f);

        phone.SetActive(onOff);
    }

    public IEnumerator Water()
    {
        Global.input.controllers.maps.SetMapsEnabled(false, "Movement");

        yield return new WaitForSeconds(0.1f);
        waterBottle.SetActive(true);

        animator.CrossFadeInFixedTime("Water", 0.1f);

        yield return new WaitForSeconds(3.0f);

        waterBottle.SetActive(false);

        Global.input.controllers.maps.SetMapsEnabled(true, "Movement");

        animator.CrossFadeInFixedTime("IdleWalk", 0.1f);

        interactionComponent.BumpCollider();
    }

    public void TurnTowards(Transform other)
    {
        Vector3 positionOfObject = other.position;

        Vector3 targetDirection = positionOfObject - (transform.position);

        // Find X angle
        rotationSmoother.SetDesired(Vector3.SignedAngle(Vector3.forward, targetDirection, Vector3.up));
    }

    public void SelectedMenuOption(string selectedMenuOption)
    {
        if (selectedMenuOption == "talk")
        {
            PlantHealthData data = Global.plantHealthData;

            if (!Global.debug.skipDailyDialogue)
            {
                Global.dialogueHandler.StartDialogue(data.SelectDialogueNode());
            }
            data.Talk();
        }
        else if (selectedMenuOption == "water")
        {
            TurnTowards(GameObject.Find("Plant Pot").transform);

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

            interactionComponent.NamePlant();
        }
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
            Vector3 v3 = pP.GetPosition();
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
        Global.plantHealthData.SetLightIncrementForToday(finalPosition.GetLightGainedHere());

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

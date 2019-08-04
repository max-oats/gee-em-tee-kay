using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum State
    {
        Ground,
        Jump,
        JumpFall,
    }

    /** ----- Balancing variables ----- */
    public float groundSpeed; /** The ground movement speed */
    public float carryingSpeed; /** The carrying movement speed */

    public float jumpPower; /** Jump impulse */
    public float gravity; /** Gravity default */

    public SmoothDamper rotationSmoother;

    public ParticleSystem sweatParticles;

    public GameObject stompParticle;

    /** ----- Private variables ----- */
    private const float onGroundSpeedY = -1f; /** The initial fall speed */
    private float speedY = onGroundSpeedY; /** The default vertical speed */
    private Vector3 localInput;
    private State state;
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

    // Update is called once per frame
    void Update()
    {
        UpdateInput();

        UpdateMovement();

        UpdateInteract();

        UpdateFacing();
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

    public void SelectedMenuOption(string selectedMenuOption)
    {
        if (selectedMenuOption == "talk")
        {
            Global.dayManager.Talk();
        }
        else if (selectedMenuOption == "water")
        {
            Global.dayManager.Water();
        }
        else if (selectedMenuOption == "move")
        {
            SetCarrying(true);
        }
        else if (selectedMenuOption == "neglect")
        {

        }
    }

    private void Interact()
    {
        interactionComponent.SetUpMenu();
    }

    private void PlacePot()
    {
        float minDistance = 10000f;
        Vector3 potPosition = holder.heldObject.transform.position;

        Vector3 finalPos = Vector3.zero;
        foreach (Vector3 v3 in Global.instance.potPositions)
        {
            float checkDistance = (potPosition - v3).magnitude;

            if (checkDistance < minDistance)
            {
                minDistance = checkDistance;
                finalPos = v3;
            }
        }

        holder.Drop(finalPos);

        GameObject go = Instantiate(stompParticle, finalPos, Quaternion.identity);
        Destroy(go, 1.0f);

        Global.cameraController.ScreenShake(0.2f);
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
            move = Time.deltaTime * groundSpeed * localInput;
        else
            move = Time.deltaTime * carryingSpeed * localInput;

        /** ----- HORIZONTAL ----- */
        // If there is a movement input, attempt to move
        if (move != Vector3.zero)
        {
            // Move while sliding up slopes/stairs and on XZ around obstructions
            entity.SlideMove(move, fromPos, ref toPos);
        }

        // Apply half of the gravity here
        if (state != State.Ground)
        {
            speedY += gravity * Time.deltaTime * .5f;
        }

        // Set first half of transform position
        transform.position = toPos;

        /** ----- VERTICAL ----- */
        // Reset from+to positions
        fromPos = transform.position;
        toPos = fromPos;

        // Calculate the vertical movement vector
        Vector3 moveY = Time.deltaTime * speedY * Vector3.up;

        // Test moving down
        if (!entity.MoveY(moveY, fromPos, ref toPos))
        {
            // If the vertical movement is not successful
            if (moveY.y < 0f)
            {
                // If not currently set to "on the ground"
                if (state != State.Ground)
                {
                    // Set state
                    SetState(State.Ground);
                }

                // Make sure we have a little bit of down speed set always for when we run off an edge next
                speedY = onGroundSpeedY;
            }
            else
            {
                // If movement is not successful and the speed
                if (speedY > 0f)
                {
                    speedY = 0f;
                }
            }
        }
        else
        {
            // if we're falling down, we must not be on the ground anymore
            if (moveY.y < 0f && state == State.Ground)
            {
                SetState(State.JumpFall);
            }
        }

        // Actually set position now that it's tested
        transform.position = toPos;

        // If not on ground, handle gravity
        if (state != State.Ground)
        {
            // Add vertical velocity
            speedY += gravity * Time.deltaTime * .5f;

            if (speedY < 0f)
            {
                // Begin falling
                SetState(State.JumpFall);
            }
        }
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

    private void SetState(State newState)
    {
        // Update state
        state = newState;

        // Set state to grounded
        if (state == State.Ground)
        {
            // animate
        }
        else if (state == State.Jump)
        {
            // animate
        }
        else if (state == State.JumpFall)
        {
            // animate
        }
    }
}

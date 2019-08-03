using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum State
    {
        Ground,
        Jump,
        JumpFall
    }

    /** ----- Balancing variables ----- */
    public float groundSpeed; /** The ground movement speed */
    
    public float jumpPower; /** Jump impulse */
    public float gravity; /** Gravity default */

    /** ----- Private variables ----- */
    private const float onGroundSpeedY = -1f; /** The initial fall speed */
    private float speedY = onGroundSpeedY; /** The default vertical speed */
    private Vector3 localInput;
    private State state;
    private Entity entity;

    void Awake()
    {
        entity = GetComponent<Entity>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInput();

        UpdateMovement();
    }

    private void UpdateInput()
    {
        // Create input vector
        Vector3 input = new Vector3(Global.input.GetAxis("Horizontal"), 0.0f, Global.input.GetAxis("Vertical"));

        // Normalize magnitude
        localInput = Vector3.ClampMagnitude(input, 1.0f);
    }

    private void UpdateMovement()
    {
        // Create position vectors
        Vector3 fromPos = transform.position;
        Vector3 toPos = fromPos;

        // Calculate horizontal movement vector
        Vector3 move = Time.deltaTime * groundSpeed * localInput;

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

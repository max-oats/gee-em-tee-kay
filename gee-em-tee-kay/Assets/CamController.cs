using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    public SmoothDamper cameraSmoother;
    public SmoothDamper cameraYSmoother;
    public Transform playerToFollow;
    public float closenessToCenter = 3f;
    public float dialogueUpBoost = 3f;
    public SmoothDamper screenShakeDamper;

    private Vector3 initialPosition;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float playerX = playerToFollow.position.x;

        cameraSmoother.SetDesired((playerX - initialPosition.x) / closenessToCenter);

        transform.position = new Vector3(cameraSmoother.Smooth(), initialPosition.y, initialPosition.z);

        if (Global.dialogueHandler.inDialogue)
        {
            cameraYSmoother.SetDesired(playerToFollow.position.z / closenessToCenter);
        }
        else
        {
            cameraYSmoother.SetDesired(0.0f);
        }
        
        transform.position = transform.position + (transform.forward * cameraYSmoother.Smooth());

        // Add screenshake
        Vector2 screenShake = Random.insideUnitCircle;
        float multiplier = screenShakeDamper.Smooth();

        transform.position = transform.position + (transform.up*screenShake.y*multiplier) + (transform.right*screenShake.x*multiplier);
    }

    public void ScreenShake(float strength = 0.1f)
    {
        screenShakeDamper.SetCurrent(strength);
    }
}

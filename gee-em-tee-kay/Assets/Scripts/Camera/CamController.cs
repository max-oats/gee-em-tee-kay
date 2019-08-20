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

    [SerializeField] private AnimationCurve cameraMoveCurve;
    [SerializeField] private Vector3 startMenuPosition;
    [SerializeField] private Vector3 startMenuRotation;
    [SerializeField] private Vector3 initialPosition;
    [SerializeField] private Vector3 initialRotation;

    private bool hasStarted = true;

    // Update is called once per frame
    void LateUpdate()
    {
        if (hasStarted)
        {
            float playerX = playerToFollow.position.x;

            cameraSmoother.SetDesired((playerX - initialPosition.x) / closenessToCenter);

            transform.position = new Vector3(cameraSmoother.Smooth(), initialPosition.y, initialPosition.z);

            if (Global.dialogueHandler.inDialogue)
            {
                cameraYSmoother.SetDesired(playerToFollow.position.z + 10.0f / closenessToCenter);
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
    }

    public void MoveCamera()
    {
        StartCoroutine(LerpCameraToOriginalPoint());
    }

    IEnumerator LerpCameraToOriginalPoint()
    {
        float timeCounter = 0f;

        while (timeCounter < cameraMoveCurve.keys[cameraMoveCurve.keys.Length-1].time)
        {
            transform.position = Vector3.Lerp(startMenuPosition, initialPosition, cameraMoveCurve.Evaluate(timeCounter));
            transform.eulerAngles = Vector3.Lerp(startMenuRotation, initialRotation, cameraMoveCurve.Evaluate(timeCounter));

            timeCounter += Time.deltaTime;

            yield return null;
        }

        transform.position = initialPosition;
        transform.eulerAngles = initialRotation;

        hasStarted = true;
    }

    public void ScreenShake(float strength = 0.1f)
    {
        screenShakeDamper.SetCurrent(strength);
    }

    public void SetBackgroundColour(Color newColor)
    {
        GetComponentInChildren<Camera>().backgroundColor = (newColor);
    }
}

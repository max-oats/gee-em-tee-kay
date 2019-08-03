using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    public SmoothDamper cameraSmoother;
    public Transform playerToFollow;
    public float closenessToCenter = 3f;

    private float initialPosition;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position.x;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float playerX = playerToFollow.position.x;

        cameraSmoother.SetDesired((playerX - initialPosition) / closenessToCenter);

        transform.position = new Vector3(cameraSmoother.Smooth(), transform.position.y, transform.position.z);
    }
}

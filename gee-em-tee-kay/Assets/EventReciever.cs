using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventReciever : MonoBehaviour
{
    public Transform rightFeet;
    public Transform leftFeet;
    public GameObject stompParticle;

    public void LeftStomp()
    {
        GameObject go = Instantiate(stompParticle, leftFeet);
        Destroy(go, 1.0f);

        Global.cameraController.ScreenShake();
    }

    public void RightStomp()
    {
        GameObject go = Instantiate(stompParticle, rightFeet);
        Destroy(go, 1.0f);

        Global.cameraController.ScreenShake();
    }
}
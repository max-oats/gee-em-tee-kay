using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventReciever : MonoBehaviour
{
    public Transform rightFeet;
    public Transform leftFeet;
    public Transform waterBottle;

    public GameObject rightAudio;
    public GameObject leftAudio;
    
    public GameObject rightStompAudio;
    public GameObject leftStompAudio;

    public GameObject stompParticle;
    public GameObject waterParticle;

    public void LeftStep()
    {
        GameObject go = Instantiate(leftAudio);
        Destroy(go, 1.0f);
    }

    public void RightStep()
    {
        GameObject go = Instantiate(rightAudio);
        Destroy(go, 1.0f);
    }

    public void LeftStomp()
    {
        GameObject go = Instantiate(stompParticle, leftFeet);
        Destroy(go, 1.0f);

        GameObject audiogo = Instantiate(leftStompAudio);
        Destroy(audiogo, 1.0f);

        Global.cameraController.ScreenShake();
    }

    public void RightStomp()
    {
        GameObject go = Instantiate(stompParticle, rightFeet);
        Destroy(go, 1.0f);

        GameObject audiogo = Instantiate(rightStompAudio);
        Destroy(audiogo, 1.0f);

        Global.cameraController.ScreenShake();
    }

    public void Spray()
    {
        GameObject go = Instantiate(waterParticle, waterBottle);
        Destroy(go, 1.0f);
    }
}
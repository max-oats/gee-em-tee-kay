using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PotPosition
{
    public Vector3 Position;
    public int LightGainedHere;
}

public class Global : MonoBehaviour
{
    public CamController camController;
    public DialogueHandler diaHandler;
    public DayManager theDayManager;

    public List<PotPosition> potPositions;

    public static Rewired.Player input;
    public static Global instance;
    public static DayManager dayManager;
    public static CamController cameraController;
    public static DialogueHandler dialogueHandler;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialize Input
            input = Rewired.ReInput.players.GetPlayer("Player0");

            cameraController = camController;
            dialogueHandler = diaHandler;
            dayManager = theDayManager;
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        foreach(PotPosition pP in potPositions)
        {
            Gizmos.DrawSphere(pP.Position, 0.1f);
        }
    }

    public static void Quit()
    {
        Application.Quit();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour
{
    public CamController camController;
    public DialogueHandler diaHandler;
    public DayManager theDayManager;

    public List<Vector3> potPositions;

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
        foreach(Vector3 v3 in potPositions)
        {
            Gizmos.DrawSphere(v3, 0.1f);
        }
    }

    public static void Quit()
    {
        Application.Quit();
    }
}

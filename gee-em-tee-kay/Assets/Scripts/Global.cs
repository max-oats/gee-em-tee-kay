using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour
{
    public CamController camController;
    public DialogueHandler diaHandler;

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

            // Grab daymanager
            //dayManager = GameObject.Find("DayManager").GetComponent<DayManager>();
        }
    }

    public static void Quit()
    {
        Application.Quit();
    }
}

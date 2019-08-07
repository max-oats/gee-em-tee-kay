using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour
{
    public CamController camController;
    public DialogueHandler diaHandler;
    public DayManager theDayManager;
    public PlantManager thePlantManager;
    public PlantHealthData thePlantHealthData;
    public PotPositionHolder thePotPositionHolder;
    public DebugStuff debugPublic;

    public static Rewired.Player input;
    public static Global instance;
    public static DayManager dayManager;
    public static CamController cameraController;
    public static DialogueHandler dialogueHandler;
    public static PlantManager plantManager;
    public static PlantHealthData plantHealthData;
    public static PotPositionHolder potPositionHolder;
    public static bool hasStarted;
    public static DebugStuff debug;

    public static string plantName;

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
            plantManager = thePlantManager;
            plantHealthData = thePlantHealthData;
            potPositionHolder = thePotPositionHolder;
            debug = debugPublic;

            hasStarted = true;
        }
    }

    public static void Quit()
    {
        Application.Quit();
    }
}

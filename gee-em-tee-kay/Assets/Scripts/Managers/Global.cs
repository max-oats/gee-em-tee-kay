using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Global : MonoBehaviour
{
    public static Rewired.Player input;
    public static Global instance;
    public static DayManager dayManager;
    public static CamController cameraController;
    public static DialogueHandler dialogueHandler;
    public static PlantManager plantManager;
    public static PlantHealthData plantHealthData;
    public static PotPositionHolder potPositionHolder;
    public static bool hasStarted = false;
    public static DebugStuff debug;

    public static string plantName;

    [SerializeField] private CamController camController;
    [SerializeField] private DialogueHandler diaHandler;
    [SerializeField] private DayManager theDayManager;
    [SerializeField] private PlantManager thePlantManager;
    [SerializeField] private PlantHealthData thePlantHealthData;
    [SerializeField] private PotPositionHolder thePotPositionHolder;
    [SerializeField] private DebugStuff debugPublic;

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

    public static void AddName()
    {
        var writer = new StreamWriter(Application.dataPath + "/namesList.txt", true);

        writer.WriteLine("- " + plantName);

        writer.Close();
    }

    public static void Quit()
    {
        Application.Quit();
    }
}

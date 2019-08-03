using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour
{
    public static Rewired.Player input;
    public static Global instance;
    public static DayManager dayManager;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialize Input
            input = Rewired.ReInput.players.GetPlayer("Player0");

            // Grab daymanager
            //dayManager = GameObject.Find("DayManager").GetComponent<DayManager>();
        }
    }

    public static void Quit()
    {
        Application.Quit();
    }
}

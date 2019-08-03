using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionComponent : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.gameObject.tag != "PlantPot")
        {
            return;
        }
        Debug.Log("Enter Plant Pot!");
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.parent.gameObject.tag != "PlantPot")
        {
            return;
        }
        Debug.Log("Exit Plant Pot!");
    }
}

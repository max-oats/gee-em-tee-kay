using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionComponent : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided");
        if (other.gameObject.tag != "PlantPot")
        {
            return;
        }

        Debug.Log("Plant Pot!");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionComponent : MonoBehaviour
{
    public bool bIsAbleToInteract = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.gameObject.tag != "PlantPot")
        {
            return;
        }
        
        bIsAbleToInteract = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.parent.gameObject.tag != "PlantPot")
        {
            return;
        }

        bIsAbleToInteract = false;
    }
}

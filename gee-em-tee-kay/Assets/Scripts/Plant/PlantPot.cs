using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantPot : MonoBehaviour
{
    public float distanceFromCenter = 1.0f;
    public float height = 1.0f;
    public float bottomOfPlantOffset = 0f;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + transform.up*(bottomOfPlantOffset + height/2f), new Vector3(distanceFromCenter*2, height, distanceFromCenter*2));
    }
}

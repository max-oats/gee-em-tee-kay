using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantPot : MonoBehaviour
{
    public float distanceFromCenter = 1.0f;
    public float height = 1.0f;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + transform.up*(height/2f), new Vector3(distanceFromCenter, height, distanceFromCenter));
    }
}

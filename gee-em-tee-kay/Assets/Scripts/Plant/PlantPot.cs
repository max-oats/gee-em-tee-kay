using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantPot : MonoBehaviour
{
    [SerializeField] private float distanceFromCenter;
    [SerializeField] private float height;
    [SerializeField] private float bottomOfPlantOffset;

    public float GetDistanceFromCenter()
    {
        return distanceFromCenter;
    }

    public float GetPlantHeightLimit()
    {
        return height;
    }

    public float GetBottomOfPlantOffset()
    {
        return bottomOfPlantOffset;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + transform.up*(bottomOfPlantOffset + height/2f), new Vector3(distanceFromCenter*2, height, distanceFromCenter*2));
    }
}

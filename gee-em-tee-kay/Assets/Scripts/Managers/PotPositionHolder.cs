using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PotPosition
{
    public Vector3 position;
    public int lightGainedHere;
}

public class PotPositionHolder : MonoBehaviour
{
    public List<PotPosition> potPositions;

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        foreach(PotPosition pP in potPositions)
        {
            Gizmos.DrawSphere(pP.position, 0.1f);
        }
    }
}

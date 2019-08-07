using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PotPosition
{
    [SerializeField] private Vector3 position;
    [SerializeField] private int lightGainedHere;

    public Vector3 GetPosition() { return position; }
    public int GetLightGainedHere() { return lightGainedHere; }
}

public class PotPositionHolder : MonoBehaviour
{
    public List<PotPosition> potPositions;

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        foreach(PotPosition pP in potPositions)
        {
            Gizmos.DrawSphere(pP.GetPosition(), 0.1f);
        }
    }
}

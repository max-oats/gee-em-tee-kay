using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf : MonoBehaviour
{
    public void SetColor(Color inColor)
    {
        transform.Find("LeafForStem").GetComponent<MeshRenderer>().material.color = inColor;
    }

    public void SetScale(float scale)
    {
        transform.Find("LeafForStem").transform.localScale *= scale;
    }
}

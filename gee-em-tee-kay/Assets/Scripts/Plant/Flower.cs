using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{
    [SerializeField] bool followsStem;

    public void SetColors(Color inPrimaryColor, Color inHighlightColor)
    {
        MeshRenderer meshRenderer = transform.Find("FlowerModel").GetComponent<MeshRenderer>();
        meshRenderer.materials[0].color = inPrimaryColor;
        if (meshRenderer.materials.Length > 0)
        {
            meshRenderer.materials[1].color = inHighlightColor;
        }
    }
}

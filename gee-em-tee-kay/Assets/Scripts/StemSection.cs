using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(LineRenderer))]
public class StemSection : MonoBehaviour
{
    public float height;
    public Vector3 startPoint;
    public Vector3 endPoint;
    public Vector3 startTangent;
    public Vector3 endTangent;

    public LineRenderer lineRenderer;

    public float startPointOffset = 0;
    public float endPointOffset = 0;

    private int layerOrder = 0;
    private int SEGMENT_COUNT = 50;

    public void SetMaterial(Material StemMaterial)
    {
        lineRenderer.material = StemMaterial;
    }

    void Start()
    {
        if (!lineRenderer)
        {
            lineRenderer = gameObject.GetComponent<LineRenderer>();
        }
        lineRenderer.sortingLayerID = layerOrder;
    }

    void Update()
    {
        DrawCurve();
    }

    void DrawCurve()
    {
        for (int i = 0; i <= SEGMENT_COUNT; i++)
        {
            float t = i / (float)SEGMENT_COUNT;
            Vector3 pixel = CalculateCubicBezierPoint(t, startPoint + new Vector3(startPointOffset,0,0), startTangent + new Vector3(startPointOffset,0,0), endTangent + new Vector3(endPointOffset,0,0), endPoint + new Vector3(endPointOffset,0,0));
            lineRenderer.positionCount = i+1;
            lineRenderer.SetPosition(i, pixel);
        }
    }

    Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;

        return p;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(LineRenderer))]
public class FlowerSection : MonoBehaviour
{
    public float height;
    public Vector3 startPoint;
    public Vector3 endPoint;
    public Vector3 startTangent;
    public Vector3 endTangent;

    public LineRenderer lineRenderer;

    private int layerOrder = 0;
    private int SEGMENT_COUNT = 50;


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
        for (int i = 1; i <= SEGMENT_COUNT; i++)
        {
            float t = i / (float)SEGMENT_COUNT;
            Vector3 pixel = CalculateCubicBezierPoint(t, startPoint, startTangent, endTangent, endPoint);
            lineRenderer.positionCount = i;
            lineRenderer.SetPosition(i - 1, pixel);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(LineRenderer))]
public class StemSection : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private GameObject startTangentMarker;
    [SerializeField] private GameObject endTangentMarker;
    [SerializeField] private GameObject endPointMarker;

    [SerializeField] private bool drawGizmos = false;

    private int layerOrder = 0;
    private int SEGMENT_COUNT = 5;

    public Transform GetStartPoint()
    {
        return transform.position;
    }

    public Transform GetStartTangent()
    {
        return startTangentMarker.transform.position;
    }

    public Transform GetEndTangent()
    {
        return endTangentMarker.transform.position;
    }

    public Transform GetEndPoint()
    {
        return endPointMarker.transform.position;
    }

    public void SetStartPoint(Vector3 inStartPoint)
    {
        transform.position = inStartPoint;
    }

    public void SetStartTangent(Vector3 inStartTangent)
    {
        startTangentMarker.transform.position = inStartTangent;
    }

    public void SetEndTangent(Vector3 inEndTangent)
    {
        endTangentMarker.transform.position = inEndTangent;
    }

    public void SetEndPoint(Vector3 inEndPoint)
    {
        endPointMarker.transform.position = inEndPoint;
    }

    public void SetColour(Color inColor)
    {
        lineRenderer.material.color = inColor;
    }

    void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 0.1f);
            Gizmos.DrawSphere(endPointMarker.transform.position, 0.1f);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(startTangentMarker.transform.position, 0.1f);
            Gizmos.DrawSphere(endTangentMarker.transform.position, 0.1f);
        }
    }

    void Awake()
    {
        startTangentMarker = new GameObject("startTangent");
        endTangentMarker = new GameObject("endTangent");
        endPointMarker = new GameObject("endPoint");

        startTangentMarker.transform.parent = transform;
        endTangentMarker.transform.parent = transform;
        endPointMarker.transform.parent = transform;
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
        Vector3 p0 = transform.position;
        Vector3 p1 = startTangentMarker.transform.position;
        Vector3 p2 = endTangentMarker.transform.position;
        Vector3 p3 = endPointMarker.transform.position;

        for (int i = 0; i <= SEGMENT_COUNT; i++)
        {
            float t = i / (float)SEGMENT_COUNT;
            Vector3 pixel = BezierUtils.CalculateCubicBezierPoint(t, p0, p1, p2, p3);
            lineRenderer.positionCount = i+1;
            lineRenderer.SetPosition(i, pixel);
        }
    }
}

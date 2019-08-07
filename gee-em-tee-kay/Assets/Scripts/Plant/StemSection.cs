using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(LineRenderer))]
public class StemSection : MonoBehaviour
{
    public Vector3 end;
    public Vector3 startTangent;
    public Vector3 endTangent;

    public Transform originPoint;
    public Transform endPoint;

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject endPointMarker;

    [SerializeField] private bool drawGizmos = false;

    private int layerOrder = 0;
    private int SEGMENT_COUNT = 50;

    public void SetColour(Color inColor)
    {
        lineRenderer.material.color = inColor;
    }

    public void Realign()
    {
        gameObject.transform.rotation = originPoint.rotation;
    }

    void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Vector3 p = transform.position;
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(p, 0.1f);
            Gizmos.DrawSphere(p + end, 0.1f);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(p + startTangent, 0.1f);
            Gizmos.DrawSphere(p + endTangent, 0.1f);
        }
    }

    void Awake()
    {
        endPointMarker = new GameObject("endPoint");
        endPointMarker.transform.parent = gameObject.transform;
        endPoint = endPointMarker.transform;
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
        endPoint.transform.position = gameObject.transform.position + end;
    }

    void DrawCurve()
    {
        Vector3 p = gameObject.transform.position;
        for (int i = 0; i <= SEGMENT_COUNT; i++)
        {
            float t = i / (float)SEGMENT_COUNT;
            Vector3 pixel = BezierUtils.CalculateCubicBezierPoint(t, p, p + startTangent, p + endTangent, p + end);
            lineRenderer.positionCount = i+1;
            lineRenderer.SetPosition(i, pixel);
        }
    }
}

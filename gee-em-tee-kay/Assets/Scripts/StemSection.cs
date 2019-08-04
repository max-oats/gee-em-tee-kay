using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(LineRenderer))]
public class StemSection : MonoBehaviour
{
    public float height;

    public Vector3 endPoint;
    public Vector3 startTangent;
    public Vector3 endTangent;

    public Transform EndPoint;

    public LineRenderer lineRenderer;

    public float startPointOffset = 0;
    public float endPointOffset = 0;

    private int layerOrder = 0;
    private int SEGMENT_COUNT = 50;

    [SerializeField] private bool DrawGizmos = false;

    [SerializeField] private GameObject endPointMarker;

    public void SetColour(Color inColor)
    {
        lineRenderer.material.color = inColor;
    }

    void OnDrawGizmos()
    {
        if (DrawGizmos)
        {
            Vector3 p = transform.position;
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(p, 0.1f);
            Gizmos.DrawSphere(p + endPoint, 0.1f);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(p + startTangent, 0.1f);
            Gizmos.DrawSphere(p + endTangent, 0.1f);
        }
    }

    void Awake()
    {
        endPointMarker = new GameObject("EndPoint");
        endPointMarker.transform.parent = gameObject.transform;
        EndPoint = endPointMarker.transform;
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
        EndPoint.transform.position = gameObject.transform.position + endPoint + new Vector3(endPointOffset,0,0);
    }

    void DrawCurve()
    {
        Vector3 p = gameObject.transform.position;
        for (int i = 0; i <= SEGMENT_COUNT; i++)
        {
            float t = i / (float)SEGMENT_COUNT;
            Vector3 pixel = BezierUtils.CalculateCubicBezierPoint(t, p + new Vector3(startPointOffset,0,0), p + startTangent + new Vector3(startPointOffset,0,0), p + endTangent + new Vector3(endPointOffset,0,0), p + endPoint + new Vector3(endPointOffset,0,0));
            lineRenderer.positionCount = i+1;
            lineRenderer.SetPosition(i, pixel);
        }
    }
}

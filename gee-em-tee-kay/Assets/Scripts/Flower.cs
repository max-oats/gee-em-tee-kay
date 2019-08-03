using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{
    public GameObject flowerSectionPrefab;
    public FlowerSection initialSection;

    public Vector3 nextEndPointTEST;

    public float segmentOffsetIncrease;

    private List<FlowerSection> sections;
    private float time = 0;

    void Start()
    {
        sections = new List<FlowerSection>();
        sections.Add(initialSection);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AddSection(nextEndPointTEST);
        }

        time += Time.deltaTime;
        float offset = 0;
        float multiplier = 0.2f;
        float segmentOffset = 0f;
        foreach (FlowerSection section in sections)
        {
            section.startPointOffset = offset;
            offset = multiplier * Mathf.Sin(time +segmentOffset);
            multiplier += 0.1f;
            segmentOffset += segmentOffsetIncrease;
            section.endPointOffset = offset;
        }
    }

    public void AddSection(Vector3 endPoint)
    {
        if (sections.Count == 0)
        {
            return;
        }

        FlowerSection lastSection = sections[sections.Count-1];
        FlowerSection newSection = Instantiate(flowerSectionPrefab, transform).GetComponent<FlowerSection>();
        Vector3 p0 = lastSection.endPoint;
        Vector3 p3 = endPoint;
        Vector3 d = (p0 - lastSection.endTangent).normalized;
        float scale = (p3 - p0).magnitude / 3;
        Vector3 p1 = p0 + scale * d;
        Vector3 m = (p1 + p3) / 2;
        Vector3 p2 = m + (p1-p0) / 2;

        newSection.startPoint = p0;
        newSection.startTangent = p1;
        newSection.endTangent = p2;
        newSection.endPoint = p3;

        sections.Add(newSection);
    }
}

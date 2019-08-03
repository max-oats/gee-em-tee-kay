using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{
    public GameObject flowerSectionPrefab;
    public FlowerSection initialSection;

    public Vector3 nextEndPointTEST;

    private List<FlowerSection> sections;

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
    }

    public void AddSection(Vector3 endPoint)
    {
        if (sections.Count == 0)
        {
            return;
        }

        FlowerSection lastSection = sections[sections.Count-1];
        FlowerSection newSection = Instantiate(flowerSectionPrefab, transform).GetComponent<FlowerSection>();
        newSection.startPoint = lastSection.endPoint;
        newSection.startTangent = lastSection.endTangent;
        newSection.endPoint = endPoint;
        sections.Add(newSection);
    }
}

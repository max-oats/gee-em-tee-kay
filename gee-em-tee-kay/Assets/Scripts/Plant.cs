using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    public Vector3 nextEndPointOffsetTEST;

    [SerializeField] private GameObject stemSectionPrefab;
    [SerializeField] private float maxHeightOfPlant;
    [SerializeField] private int maxNumberOfLeavesPerDay;

    [SerializeField] private float segmentOffsetIncrease;
    [SerializeField] private float initialMultiplier;
    [SerializeField] private float multiplierIncrement;

    [SerializeField] private Transform WindowLocation;

    private GameObject FlowerPrefab;
    private float FlowerHue;
    private GameObject LeafPrefab;
    private Material StemMaterial;

    private List<StemSection> sections;
    private float time = 0;

    void Start()
    {
        sections = new List<StemSection>();
    }

    public void Setup(GameObject inFlowerPrefab, float inFlowerHue, GameObject inLeafPrefab, Material inStemMaterial, Vector3 inRootPosition)
    {
        FlowerPrefab = inFlowerPrefab;
        FlowerHue = inFlowerHue;
        LeafPrefab = inLeafPrefab;
        StemMaterial = inStemMaterial;
        // TODO use Material for Stem

        StemSection initialSection = Instantiate(stemSectionPrefab, transform).GetComponent<StemSection>();
        initialSection.endPoint = inRootPosition;
        initialSection.endTangent = inRootPosition - new Vector3(0,1,0);
        initialSection.startTangent = inRootPosition - new Vector3(0,2,0);
        initialSection.startPoint = inRootPosition - new Vector3(0,3,0);
        initialSection.gameObject.GetComponent<LineRenderer>().enabled = false;
        sections.Add(initialSection);
    }

    public void AddSectionsForDay()
    {
        // TODO Implement
        Debug.Log("Adding Sections for Day");
    }

    void Update()
    {
        time += Time.deltaTime;
        float offset = 0;
        float multiplier = initialMultiplier;
        float segmentOffset = 0f;
        bool first = true;
        foreach (StemSection section in sections)
        {
            if (first)
            {
                first = false;
                continue;
            }
            section.startPointOffset = offset;
            offset = multiplier * Mathf.Sin(time +segmentOffset);
            multiplier += multiplierIncrement;
            segmentOffset += segmentOffsetIncrease;
            section.endPointOffset = offset;
        }
    }

    public void AddSection(Vector3 endPointOffset)
    {
        if (sections.Count == 0)
        {
            return;
        }

        StemSection lastSection = sections[sections.Count-1];
        StemSection newSection = Instantiate(stemSectionPrefab, transform).GetComponent<StemSection>();
        Vector3 p0 = lastSection.endPoint;
        Vector3 p3 = p0 + endPointOffset;
        Vector3 d = (p0 - lastSection.endTangent).normalized;
        float scale = (p3 - p0).magnitude / 3;
        Vector3 p1 = p0 + scale * d;
        Vector3 m = (p1 + p3) / 2;
        Vector3 p2 = m + (p1-p0) / 2;

        newSection.startPoint = p0;
        newSection.startTangent = p1;
        newSection.endTangent = p2;
        newSection.endPoint = p3;

        newSection.SetMaterial(StemMaterial);

        sections.Add(newSection);
    }
}

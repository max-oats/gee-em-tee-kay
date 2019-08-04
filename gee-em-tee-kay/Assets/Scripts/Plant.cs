using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    public Vector3 nextEndPointOffsetTEST;
    public int sectionsToSplitInto = 1;

    [SerializeField] private GameObject stemSectionPrefab;
    [SerializeField] private float maxHeightOfPlant;
    [SerializeField] private int maxNumberOfLeavesPerDay;
    [SerializeField] private Color DeadColor;

    [SerializeField] private float segmentOffsetIncrease;
    [SerializeField] private float initialMultiplier;
    [SerializeField] private float multiplierIncrement;

    [SerializeField] private Transform WindowLocation;

    private GameObject FlowerPrefab;
    private float FlowerHue;
    private GameObject LeafPrefab;
    private Color StemColor;

    private List<StemSection> sections;
    private float time = 0;

    void Start()
    {
        sections = new List<StemSection>();
    }

    public void Setup(GameObject inFlowerPrefab, float inFlowerHue, GameObject inLeafPrefab, Color inStemColor, Vector3 inRootPosition)
    {
        FlowerPrefab = inFlowerPrefab;
        FlowerHue = inFlowerHue;
        LeafPrefab = inLeafPrefab;
        StemColor = inStemColor;
        // TODO use Color for Stem

        StemSection initialSection = Instantiate(stemSectionPrefab, transform).GetComponent<StemSection>();
        initialSection.startTangent = new Vector3(0,0.1f,0);
        initialSection.endTangent = new Vector3(0,0.2f,0);
        initialSection.endPoint = new Vector3(0,0.3f,0);
        initialSection.gameObject.GetComponent<LineRenderer>().enabled = false;
        initialSection.gameObject.transform.parent = gameObject.transform;
        sections.Add(initialSection);
    }

    public void AddSectionsForDay()
    {
        // TODO Implement
        Debug.Log("Adding Sections for Day");

        DayManager dm = Global.dayManager;

        if (!dm.HasEverConversed())
        {
            return;
        }

        // Debug
        AddSection(nextEndPointOffsetTEST);
    }

    void SetColourBasedOnHealth(float param)
    {
        foreach (StemSection section in sections)
        {
            section.SetColour(Color.Lerp(StemColor, DeadColor, param));
        }
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

        SetColourBasedOnHealth(Mathf.PingPong(Time.time, 1));
    }

    public void AddSection(Vector3 endPointOffset)
    {
        if (sections.Count == 0)
        {
            return;
        }

        StemSection lastSection = sections[sections.Count-1];

        Vector3 op0 = lastSection.gameObject.transform.position;
        Vector3 op2 = op0 + lastSection.endTangent;
        Vector3 op3 = op0 + lastSection.endPoint;

        Vector3 p0 = op3;
        Vector3 p3 = p0 + endPointOffset;
        Vector3 d = (p0 - op2).normalized;
        float scale = (p3 - p0).magnitude / 3;
        Vector3 p1 = p0 + scale * d;
        Vector3 m = (p1 + p3) / 2;
        Vector3 p2 = m + (p1-p0) / 2;

        List<Vector3> ControlPointsToAdd = BezierUtils.SplitCubicBezierNWays(p0, p1, p2, p3, sectionsToSplitInto);
        if (ControlPointsToAdd.Count / 4 == sectionsToSplitInto)
        {
            Debug.Log("Correct num sections");
        }
        else
        {
            Debug.Log(string.Format("Expected {0} sections, got {1}", sectionsToSplitInto, ControlPointsToAdd.Count / 4));
            return;
        }

        for (int i = 0; i < sectionsToSplitInto; i++)
        {
            int initialIndex = i * 4;
            AddSection(lastSection.EndPoint, ControlPointsToAdd[initialIndex], ControlPointsToAdd[initialIndex+1], ControlPointsToAdd[initialIndex+2], ControlPointsToAdd[initialIndex+3]);
        }
    }

    private void AddSection(Transform Attachment, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        StemSection newSection = Instantiate(stemSectionPrefab, Attachment).GetComponent<StemSection>();

        newSection.gameObject.transform.position = p0;
        newSection.startTangent = p1 - p0;
        newSection.endTangent = p2 - p0;
        newSection.endPoint = p3 - p0;

        sections.Add(newSection);
    }
}

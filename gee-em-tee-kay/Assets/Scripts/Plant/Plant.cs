using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    // ~Begin Debug
    [SerializeField] private Vector3 nextEndPointOffsetTEST;
    [SerializeField] private int sectionsToSplitIntoTEST = 1;
    // ~End Debug

    [SerializeField] private float maxHorizontalDistance;

    [SerializeField] private GameObject stemSectionPrefab;
    [SerializeField] private int maxNumberOfLeavesPerDay;
    [SerializeField] private Color deadColor;

    [SerializeField] private Transform windowLocation;

    private GameObject flowerPrefab;
    private float flowerHue;
    private GameObject leafPrefab;
    private Color stemColor;

    private List<StemSection> sections;
    private List<Leaf> leaves;
    private float time = 0;

    void Start()
    {
        sections = new List<StemSection>();
        leaves = new List<Leaf>();
    }

    public void Setup(int seed, GameObject inFlowerPrefab, float inFlowerHue, GameObject inLeafPrefab, Color inStemColor, Vector3 inRootPosition)
    {
        Random.InitState(seed);
        flowerPrefab = inFlowerPrefab;
        flowerHue = inFlowerHue;
        leafPrefab = inLeafPrefab;
        stemColor = inStemColor;

        StemSection initialSection = Instantiate(stemSectionPrefab, transform).GetComponent<StemSection>();
        initialSection.originPoint = transform;
        initialSection.transform.position = inRootPosition - new Vector3(0, 0.3f, 0);
        initialSection.startTangent = new Vector3(0,0.1f,0);
        initialSection.endTangent = new Vector3(0,0.2f,0);
        initialSection.end = new Vector3(0,0.3f,0);
        initialSection.gameObject.GetComponent<LineRenderer>().enabled = false;
        initialSection.gameObject.transform.parent = gameObject.transform;

        GeneralSectionSetup(initialSection);
    }

    public void AddSectionsForDay()
    {
        if (nextEndPointOffsetTEST != Vector3.zero)
        {
            // Debug
            AddSection(nextEndPointOffsetTEST, sectionsToSplitIntoTEST, false);
            return;
        }

        PlantHealthData plantHealth = Global.plantHealthData;

        if (!plantHealth.HasEverConversed())
        {
            return;
        }

        if (plantHealth.PlantIsDead())
        {
            SetColourBasedOnHealth(plantHealth.CurrentHealthPercentage());
            return;
        }

        Vector3 toNextPoint = GetMaxHorizontalMovement();
        toNextPoint += new Vector3(0, plantHealth.GetMaxHeightOfSection(), 0);
        if (plantHealth.IsThirsty() || plantHealth.IsDrowning())
        {
            toNextPoint *= 0.5f;
        }

        int leavesToAdd = (int)Mathf.Lerp(0, maxNumberOfLeavesPerDay, plantHealth.CurrentHealthPercentage());

        AddSection(toNextPoint, leavesToAdd, plantHealth.HasTooMuchLight());

        SetColourBasedOnHealth(plantHealth.CurrentHealthPercentage());
    }

    private Vector3 GetMaxHorizontalMovement()
    {
        Vector3 e = GetLastSectionEndPos().position - transform.position;
        Vector3 w = windowLocation.position - GetLastSectionEndPos().position;
        w.y = 0;
        w.Normalize();

        float r = Global.plantHealthData.GetMaxDistanceFromPotCenter();
        float a = w.x*w.x + w.z*w.z;
        float b = 2*e.x*w.x + 2*e.z*w.z;
        float c = e.x*e.x + e.z*e.z - r*r;

        float d = Mathf.Sqrt(b*b - 4*a*c);

        float top;
        if (-b + d > 0)
        {
            top = -b + d;
        }
        else if (-b - d > 0)
        {
            top = -b - d;
        }
        else
        {
            top = 0;
        }

        float t = top / 2*a;

        float scale = Mathf.Min(t, maxHorizontalDistance);

        return w * scale;
    }

    void SetColourBasedOnHealth(float param)
    {
        foreach (StemSection section in sections)
        {
            section.SetColour(Color.Lerp(deadColor, stemColor, param));
        }
        foreach(Leaf leaf in leaves)
        {
            leaf.SetColor(Color.Lerp(deadColor, stemColor, param));
        }
    }

    void Update()
    {
        gameObject.transform.rotation = gameObject.transform.parent.rotation;
        foreach (StemSection section in sections)
        {
            section.Realign();
        }
    }

    public void AddSection(Vector3 endPointOffset, int leavesToAdd, bool leavesShouldBeSmall)
    {
        if (sections.Count == 0)
        {
            return;
        }

        int sectionsToSplitInto = leavesToAdd + 1;

        StemSection lastSection = sections[sections.Count-1];

        Vector3 op0 = lastSection.gameObject.transform.position;
        Vector3 op2 = op0 + lastSection.endTangent;
        Vector3 op3 = op0 + lastSection.end;

        Vector3 p0 = op3;
        Vector3 p3 = p0 + endPointOffset;
        Vector3 d = (p0 - op2).normalized;
        float scale = (p3 - p0).magnitude / 3;
        Vector3 p1 = p0 + scale * d;
        Vector3 m = (p1 + p3) / 2;
        Vector3 p2 = m + (p1-p0) / 2;

        List<Vector3> controlPointsToAdd = BezierUtils.SplitCubicBezierNWays(p0, p1, p2, p3, sectionsToSplitInto);

        for (int i = 0; i < sectionsToSplitInto; i++)
        {
            int initialIndex = i * 4;
            AddSection(lastSection.endPoint, controlPointsToAdd[initialIndex], controlPointsToAdd[initialIndex+1], controlPointsToAdd[initialIndex+2], controlPointsToAdd[initialIndex+3], i < leavesToAdd, leavesShouldBeSmall);
        }
    }

    private void AddSection(Transform attachment, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, bool shouldAddLeaf, bool leavesShouldBeSmall)
    {
        StemSection newSection = Instantiate(stemSectionPrefab, attachment).GetComponent<StemSection>();

        newSection.originPoint = attachment;
        newSection.gameObject.transform.position = p0;
        newSection.startTangent = p1 - p0;
        newSection.endTangent = p2 - p0;
        newSection.end = p3 - p0;

        if (shouldAddLeaf)
        {
            GameObject newLeaf = Instantiate(leafPrefab, newSection.endPoint);
            newLeaf.transform.position = newSection.endPoint.position;
            newLeaf.transform.rotation = Quaternion.Euler(0, Random.value * 360f, 0);

            Leaf leaf = newLeaf.GetComponent<Leaf>();
            if (leavesShouldBeSmall)
            {
                leaf.SetScale(0.5f);
            }
            leaves.Add(leaf);
        }

        GeneralSectionSetup(newSection);
    }

    private void GeneralSectionSetup(StemSection inSection)
    {
        inSection.SetColour(stemColor);
        sections.Add(inSection);
    }

    private Transform GetLastSectionEndPos()
    {
        StemSection lastSection = sections[sections.Count-1];
        return lastSection.endPoint;
    }
}

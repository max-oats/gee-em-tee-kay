using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    // ~Begin Debug
    [SerializeField] private Vector3 nextEndPointOffsetTEST;
    [SerializeField] private int sectionsToSplitIntoTEST = 1;
    [SerializeField] private bool debugShouldSpawnFlower = false;
    // ~End Debug

    [SerializeField] private float maxHorizontalDistance;

    [SerializeField] private GameObject stemSectionPrefab;
    [SerializeField] private int maxNumberOfLeavesPerDay;
    [SerializeField] private Color deadColor;

    [SerializeField] private Transform windowLocation;

    private GameObject flowerPrefab;
    private ColorHSV initialPrimaryFlowerColor;
    private ColorHSV initialHighlightFlowerColor;
    private GameObject leafPrefab;
    private Color stemColor;

    private List<StemSection> sections;
    private List<Leaf> leaves;

    void Start()
    {
        sections = new List<StemSection>();
        leaves = new List<Leaf>();
    }

    public void Setup(int seed, GameObject inFlowerPrefab, ColorHSV inFlowerPrimaryColor, ColorHSV inFlowerHighlightColor, GameObject inLeafPrefab, Color inStemColor, Vector3 inRootPosition)
    {
        flowerPrefab = inFlowerPrefab;

        initialPrimaryFlowerColor = inFlowerPrimaryColor;
        initialHighlightFlowerColor = inFlowerHighlightColor;

        leafPrefab = inLeafPrefab;
        stemColor = inStemColor;

        StemSection initialSection = Instantiate(stemSectionPrefab, transform).GetComponent<StemSection>();
        initialSection.SetStartPoint(inRootPosition - new Vector3(0,0.3f,0));
        initialSection.SetStartTangent(inRootPosition - new Vector3(0,0.2f,0));
        initialSection.SetEndTangent(inRootPosition - new Vector3(0,0.1f,0));
        initialSection.SetEndPoint(inRootPosition);
        initialSection.gameObject.GetComponent<LineRenderer>().enabled = false;
        initialSection.gameObject.transform.parent = transform;

        GeneralSectionSetup(initialSection);
    }

    public void AddSectionsForDay(bool shouldSpawnFlower)
    {
        if (nextEndPointOffsetTEST != Vector3.zero)
        {
            // Debug
            AddSection(nextEndPointOffsetTEST, sectionsToSplitIntoTEST, false, debugShouldSpawnFlower);
            return;
        }

        PlantHealthData plantHealth = Global.plantHealthData;

        if (!plantHealth.HasEverConversed())
        {
            return;
        }

        if (plantHealth.PlantIsDead())
        {
            SetColourBasedOnHealth(0);
            return;
        }

        Vector3 toNextPoint = GetMaxHorizontalMovement();
        toNextPoint += new Vector3(0, plantHealth.GetMaxHeightOfSection(), 0);
        if (plantHealth.IsThirsty() || plantHealth.IsDrowning())
        {
            toNextPoint *= 0.5f;
        }

        int leavesToAdd = (int)Mathf.Lerp(0, maxNumberOfLeavesPerDay, plantHealth.CurrentHealthPercentage());

        AddSection(toNextPoint, leavesToAdd, plantHealth.HasTooMuchLight(), shouldSpawnFlower);

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

    void SetStemColor(Color inColor)
    {
        foreach (StemSection section in sections)
        {
            section.SetColour(inColor);
        }
        foreach(Leaf leaf in leaves)
        {
            leaf.SetColor(inColor);
        }
    }

    void Update()
    {
        // DebugStuff
        float debugCycleSpeed = Global.debug.debugCycleSpeed;
        if (Global.debug.cycleStemColour)
        {
            float alpha = Mathf.PingPong(Time.time * debugCycleSpeed, 1);
            SetColourBasedOnHealth(alpha);
        }
        /*else if(Global.debug.cyclePossibleFlowerColours)
        {
            float hue = (Time.time * debugCycleSpeed) % 1f;
            Color newFlowerColor = Color.HSVToRGB(hue, initialFlowerPrimaryColorSat, initialFlowerPrimaryColorVal);
            SetStemColor(newFlowerColor);
        }
        else if (Global.debug.cycleFlowerColourSat)
        {
            float sat = Mathf.PingPong(Time.time * debugCycleSpeed, 1);
            Color newFlowerColor = Color.HSVToRGB(initialFlowerPrimaryColorHue, sat, initialFlowerPrimaryColorVal);
            SetStemColor(newFlowerColor);
        }
        else if (Global.debug.cycleFlowerColourValue)
        {
            float val = Mathf.PingPong(Time.time * debugCycleSpeed, 1);
            Color newFlowerColor = Color.HSVToRGB(initialFlowerPrimaryColorHue, initialFlowerPrimaryColorSat, val);
            SetStemColor(newFlowerColor);
        }*/
    }

    public void AddSection(Vector3 endPointOffset, int leavesToAdd, bool leavesShouldBeSmall, bool shouldSpawnFlower)
    {
        if (sections.Count == 0)
        {
            return;
        }

        int sectionsToSplitInto = leavesToAdd + 1;

        StemSection lastSection = sections[sections.Count-1];

        Vector3 op2 = lastSection.GetEndTangent().position;
        Vector3 op3 = lastSection.GetEndPoint().position;

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
            AddSection(lastSection.GetEndPoint(), controlPointsToAdd[initialIndex], controlPointsToAdd[initialIndex+1], controlPointsToAdd[initialIndex+2], controlPointsToAdd[initialIndex+3], i < leavesToAdd, leavesShouldBeSmall);
        }

        if (shouldSpawnFlower)
        {
            StemSection mostRecentSection = sections[sections.Count-1];
            GameObject flowerObject = Instantiate(flowerPrefab, mostRecentSection.GetEndPoint());
            Vector3 stemEndPoint = mostRecentSection.GetEndPoint().position;
            Vector3 stemEndTangent = mostRecentSection.GetEndTangent().position;
            Vector3 flowerPointDirection = stemEndPoint - stemEndTangent;
            Quaternion flowerRotation = Quaternion.LookRotation(flowerPointDirection, Vector3.up);
            flowerObject.transform.rotation = flowerRotation;

            Flower flower = flowerObject.GetComponent<Flower>();
            flower.SetColors(initialPrimaryFlowerColor.ToRGB(), initialHighlightFlowerColor.ToRGB());
        }
    }

    private void AddSection(Transform attachment, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, bool shouldAddLeaf, bool leavesShouldBeSmall)
    {
        StemSection newSection = Instantiate(stemSectionPrefab, attachment).GetComponent<StemSection>();

        newSection.SetStartPoint(p0);
        newSection.SetStartTangent(p1);
        newSection.SetEndTangent(p2);
        newSection.SetEndPoint(p3);

        if (shouldAddLeaf)
        {
            GameObject newLeaf = Instantiate(leafPrefab, newSection.GetEndPoint());
            newLeaf.transform.position = newSection.GetEndPoint().position;
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
        return lastSection.GetEndPoint();
    }
}

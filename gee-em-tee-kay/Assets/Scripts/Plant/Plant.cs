﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    // ~Begin Debug
    [SerializeField] private Vector3 nextEndPointOffsetTEST;
    [SerializeField] private int sectionsToSplitIntoTEST = 1;
    // ~End Debug

    [SerializeField] private float MaxHorizontalDistance;

    [SerializeField] private GameObject stemSectionPrefab;
    [SerializeField] private int maxNumberOfLeavesPerDay;
    [SerializeField] private Color DeadColor;

    [SerializeField] private Transform WindowLocation;

    private GameObject FlowerPrefab;
    private float FlowerHue;
    private GameObject LeafPrefab;
    private Color StemColor;

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
        FlowerPrefab = inFlowerPrefab;
        FlowerHue = inFlowerHue;
        LeafPrefab = inLeafPrefab;
        StemColor = inStemColor;

        StemSection initialSection = Instantiate(stemSectionPrefab, transform).GetComponent<StemSection>();
        initialSection.OriginPoint = transform;
        initialSection.transform.position = inRootPosition - new Vector3(0, 0.3f, 0);
        initialSection.startTangent = new Vector3(0,0.1f,0);
        initialSection.endTangent = new Vector3(0,0.2f,0);
        initialSection.endPoint = new Vector3(0,0.3f,0);
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

        Vector3 ToNextPoint = GetMaxHorizontalMovement();
        ToNextPoint += new Vector3(0, plantHealth.GetMaxHeightOfSection(), 0);
        if (plantHealth.IsThirsty() || plantHealth.IsDrowning())
        {
            ToNextPoint *= 0.5f;
        }

        int LeavesToAdd = (int)Mathf.Lerp(0, maxNumberOfLeavesPerDay, plantHealth.CurrentHealthPercentage());

        AddSection(ToNextPoint, LeavesToAdd, plantHealth.HasTooMuchLight());

        SetColourBasedOnHealth(plantHealth.CurrentHealthPercentage());
    }

    private Vector3 GetMaxHorizontalMovement()
    {
        Vector3 e = GetLastSectionEndPos().position - transform.position;
        Vector3 w = WindowLocation.position - GetLastSectionEndPos().position;
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

        float Scale = Mathf.Min(t, MaxHorizontalDistance);

        return w * Scale;
    }

    void SetColourBasedOnHealth(float param)
    {
        foreach (StemSection section in sections)
        {
            section.SetColour(Color.Lerp(DeadColor, StemColor, param));
        }
        foreach(Leaf leaf in leaves)
        {
            leaf.SetColor(Color.Lerp(DeadColor, StemColor, param));
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

    public void AddSection(Vector3 endPointOffset, int LeavesToAdd, bool LeavesShouldBeSmall)
    {
        if (sections.Count == 0)
        {
            return;
        }

        int sectionsToSplitInto = LeavesToAdd + 1;

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

        for (int i = 0; i < sectionsToSplitInto; i++)
        {
            int initialIndex = i * 4;
            AddSection(lastSection.EndPoint, ControlPointsToAdd[initialIndex], ControlPointsToAdd[initialIndex+1], ControlPointsToAdd[initialIndex+2], ControlPointsToAdd[initialIndex+3], i < LeavesToAdd, LeavesShouldBeSmall);
        }
    }

    private void AddSection(Transform Attachment, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, bool ShouldAddLeaf, bool LeavesShouldBeSmall)
    {
        StemSection newSection = Instantiate(stemSectionPrefab, Attachment).GetComponent<StemSection>();

        newSection.OriginPoint = Attachment;
        newSection.gameObject.transform.position = p0;
        newSection.startTangent = p1 - p0;
        newSection.endTangent = p2 - p0;
        newSection.endPoint = p3 - p0;

        if (ShouldAddLeaf)
        {
            GameObject newLeaf = Instantiate(LeafPrefab, newSection.EndPoint);
            newLeaf.transform.position = newSection.EndPoint.position;
            newLeaf.transform.rotation = Quaternion.Euler(0, Random.value * 360f, 0);

            Leaf leaf = newLeaf.GetComponent<Leaf>();
            if (LeavesShouldBeSmall)
            {
                leaf.SetScale(0.5f);
            }
            leaves.Add(leaf);
        }

        GeneralSectionSetup(newSection);
    }

    private void GeneralSectionSetup(StemSection inSection)
    {
        inSection.SetColour(StemColor);
        sections.Add(inSection);
    }

    private Transform GetLastSectionEndPos()
    {
        StemSection lastSection = sections[sections.Count-1];
        return lastSection.EndPoint;
    }
}

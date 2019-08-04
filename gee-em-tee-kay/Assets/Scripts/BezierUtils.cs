using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierUtils
{
    public static Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;

        return p;
    }

    public static List<Vector3> SplitCubicBezierNWays(Vector3 p0i, Vector3 p1i, Vector3 p2i, Vector3 p3i, int n)
    {
        List<Vector3> ControlPoints = new List<Vector3>();

        if (n == 1)
        {
            // degenerate case
            ControlPoints.Add(p0i);
            ControlPoints.Add(p1i);
            ControlPoints.Add(p2i);
            ControlPoints.Add(p3i);
            return ControlPoints;
        }

        // Points for the current curve being split
        Vector3 p0 = p0i;
        Vector3 p1 = p1i;
        Vector3 p2 = p2i;
        Vector3 p3 = p3i;

        // Create the split points
        List<float> Z = new List<float>();
        for (int i = 1; i < n; i++)
        {
            Z.Add(i/n);
        }

        while (Z.Count > 0)
        {
            float z = Z[0];
            float zi = 1-z;
            Z.RemoveAt(0);

            // Define points required to figure out new sections
            Vector3 q0 = zi*p0 + z*p1;
            Vector3 q1 = zi*p1 + z*p2;
            Vector3 q2 = zi*p2 + z*p3;
            Vector3 r0 = zi*q0 + z*q1;
            Vector3 r1 = zi*q1 + z*q2;
            Vector3 s0 = zi*r0 + z*r1;

            // First Section
            ControlPoints.Add(p0);
            ControlPoints.Add(q0);
            ControlPoints.Add(r0);
            ControlPoints.Add(s0);

            if (Z.Count == 0)
            {
                // Section section is last section!
                ControlPoints.Add(s0);
                ControlPoints.Add(r1);
                ControlPoints.Add(q2);
                ControlPoints.Add(p3);
            }

            // Setup points for next curve to be split
            p0 = s0;
            p1 = r1;
            p2 = q2;

            // Otherwise, need to rebalance other z params
            for (int index = 0; index < Z.Count; z++)
            {
                float ithZ = Z[index];
                Z[index] = (ithZ - z) / zi;
            }
        }

        return ControlPoints;
    }
}

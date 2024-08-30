using System.Collections.Generic;
using UnityEngine;

public class BezierEquations
{
	public static Vector3[] GenerateQuadraticBezierPoints(Vector3 p0, Vector3 p1, Vector3 p2, int numPoints)
    {
        Vector3[] points = new Vector3[numPoints];

        for (int i = 0; i < numPoints; i++)
        {
            float t = i / (float)(numPoints - 1);
            points[i] = CalculateQuadraticBezierPoint(t, p0, p1, p2);
        }

        return points;
    }

    public static Vector3[] GenerateApproximatelyEquidistantPoints(Vector3 p0, Vector3 p1, Vector3 p2, int numPoints)
    {
        List<Vector3> equidistantPoints = new List<Vector3>();
        float curveLength = EstimateCurveLength(p0, p1, p2);
        float desiredSpacing = curveLength / (numPoints - 1);

        float accumulatedLength = 0f;
        Vector3 previousPoint = p0;
        equidistantPoints.Add(p0);

        for (float t = 0.01f; t <= 1f; t += 0.01f)
        {
            Vector3 currentPoint = CalculateQuadraticBezierPoint(t, p0, p1, p2);
            float segmentLength = Vector3.Distance(previousPoint, currentPoint);
            accumulatedLength += segmentLength;

            if (accumulatedLength >= desiredSpacing)
            {
                equidistantPoints.Add(currentPoint);
                accumulatedLength = 0f;
            }

            previousPoint = currentPoint;
        }

        // Ensure the last point is added
        if (equidistantPoints.Count < numPoints)
        {
            equidistantPoints.Add(p2);
        }

        return equidistantPoints.ToArray();
    }

    private static float EstimateCurveLength(Vector3 p0, Vector3 p1, Vector3 p2, int samples = 500)
    {
        float length = 0f;
        Vector3 previousPoint = p0;

        for (int i = 1; i <= samples; i++)
        {
            float t = i / (float)samples;
            Vector3 currentPoint = CalculateQuadraticBezierPoint(t, p0, p1, p2);
            length += Vector3.Distance(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }

        return length;
    }

    static Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        return uu * p0 + 2 * u * t * p1 + tt * p2;
    }

    public static float[] GenerateCardRotations(Vector3 p0, Vector3 p1, Vector3 p2, int numCards, float baseAngle = 90f, float spreadAngle = 60f)
    {
        float[] rotations = new float[numCards];
        
        for (int i = 0; i < numCards; i++)
        {
            float t = i / (float)(numCards - 1);
            
            // Calculate the tangent to the curve at this point
            Vector3 tangent = CalculateQuadraticBezierTangent(t, p0, p1, p2);
            
            // Calculate the angle of the tangent
            float tangentAngle = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;
            
            // Adjust the angle to make cards point outwards
            float adjustedAngle = tangentAngle + baseAngle;
            
            // Apply a spread to fan out the cards
            float spreadFactor = (t - 0.5f) * spreadAngle;
            
            rotations[i] = adjustedAngle + spreadFactor;
        }
        
        return rotations;
    }

    private static Vector3 CalculateQuadraticBezierTangent(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        return 2 * (1 - t) * (p1 - p0) + 2 * t * (p2 - p1);
    }
}
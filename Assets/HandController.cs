using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class HandController : MonoBehaviour
{
    
    
    public Transform p0;
    public Transform p1;
    public Transform p2;
    public RectTransform movingBox;
    public float cardSpacing = 0.1f; // Added spacing between cards
    Vector2 cardDimensions;
    float t = 0;
    public int numCards = 8;
    void Start()
    {
        float cardWidth = Screen.width * 0.18f;
        cardDimensions = new Vector2(cardWidth, 7f/5 * cardWidth);
        movingBox.sizeDelta = cardDimensions;
        
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            foreach(Transform child in this.transform)
            {
                Destroy(child.gameObject);
            }
            Vector3[] cardPositions = GenerateApproximatelyEquidistantPoints(p0.position, p1.position, p2.position, numCards);
            float[] cardRotations = GenerateCardRotations(p0.position, p1.position, p2.position, numCards);

            for(int i = 0; i < cardPositions.Count(); ++i)
            {
                Transform box = Instantiate(movingBox);
                box.position = cardPositions[i];
                box.SetParent(this.transform);
                box.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Random.ColorHSV();
                box.rotation =  Quaternion.Euler(0, 0, 90 - cardRotations[i]);
            }
        }
    }

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

    void ArrangeChildrenAlongCurve()
    {
        // int childCount = transform.childCount;
        
        // if (childCount == 0) return;

        // float totalWidth = (childCount - 1) * (cardWidth + cardSpacing);
        // float startOffset = -totalWidth / 2f;

        // for (int i = 0; i < childCount; i++)
        // {
        //     // Calculate the t value for the current card
        //     float t;
        //     if (childCount == 1)
        //     {
        //         t = 0.5f; // Middle point for a single card
        //     }
        //     else
        //     {
        //         t = Mathf.Lerp(0.25f, 0.75f, (float)i / (childCount - 1));
        //     }

        //     Vector3 position = CalculateQuadraticBezierPoint(t, bottomLeft.position, topMiddle.position, bottomRight.position);
        //     Vector3 tangent = CalculateQuadraticBezierTangent(t, bottomLeft.position, topMiddle.position, bottomRight.position);

        //     Transform child = transform.GetChild(i);
            
        //     // Calculate the rotation to face along the curve
        //     float angle = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;
        //     Quaternion rotation = Quaternion.Euler(0, 0, angle);

        //     // Calculate the offset position
        //     Vector3 offset = rotation * Vector3.right * (startOffset + i * (cardWidth + cardSpacing));
            
        //     // Apply position and rotation
        //     child.position = position + offset;
        //     child.rotation = rotation;
        // }
    }

}

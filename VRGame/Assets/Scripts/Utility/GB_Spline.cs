using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Spline
{
	public static Vector3[] GetPoints(List<Transform> points, int interpolateSteps) {
		Vector3[] positions = new Vector3[points.Count];
		for (int i = 0; i < points.Count; i++) {
			positions[i] = points[i].position;
		}
		return GetPoints(positions, interpolateSteps);
	}

	public static Vector3[] GetPoints(Vector3[] points, int interpolateSteps)
	{
		if (points.Length < 4) {
			// No interpolation can be done
			return points;
		} 
		if (interpolateSteps == 0) {
			return points;
		}

		Vector3 point0, point1, point2, point3;
		List<Vector3> interpolatedPoints = new List<Vector3>();
		interpolatedPoints.Add(points[0]);
		// Generate points
		for (int i = 0; i < points.Length - 3; i++)
		{
			// Assign the 4 points needed for Catmull-Spline
			point0 = points[i];
			point1 = points[i + 1];
			point2 = points[i + 2];
			point3 = points[i + 3];

			// Interpolation
			for (int j = 0; j < interpolateSteps + 1; j++)
			{
				interpolatedPoints.Add(GetPoint(j / ((float)interpolateSteps + 1), point0, point1, point2, point3));
			}
		}
		interpolatedPoints.Add(GetPoint(1, points[points.Length - 4], points[points.Length - 3], points[points.Length - 2], points[points.Length - 1]));
		interpolatedPoints.Add(points[points.Length - 1]);
		return interpolatedPoints.ToArray();
	}

	public static float GetDistance(float t, Vector3 p0, Vector3 p1)
	{
		float a = Mathf.Pow((p1.x - p0.x), 2.0f) + Mathf.Pow((p1.y - p0.y), 2.0f) + Mathf.Pow((p1.z - p0.z), 2.0f);
		float b = Mathf.Pow(Mathf.Pow(a, 0.5f), 0.5f);

		return (b + t);
	}

	public static Vector3 GetPoint(float t, int index, Vector3[] points)
    {
        return GetPoint(t, points[index - 1], points[index], points[index + 1], points[index + 2]);
    }

    public static Vector3 GetTangent(float t, int index, Vector3[] points)
    {
		return GetTangent(t, points[index - 1], points[index], points[index + 1], points[index + 2]);
    }
    
	private static Vector3 GetPoint(float t, Vector3 point0, Vector3 point1, Vector3 point2, Vector3 point3)
	{
		return 0.5f * ((2f * point1) +
	        (-point0 + point2) * t +
            (2f * point0 - 5f * point1 + 4f * point2 - point3) * Mathf.Pow(t, 2) +
			(-point0 + 3f * point1 - 3f * point2 + point3) * Mathf.Pow(t, 3));
	}

	private static Vector3 GetTangent(float t, Vector3 point0, Vector3 point1, Vector3 point2, Vector3 point3)
	{
		return GetPoint(t + 0.01f, point0, point1, point2, point3) - GetPoint(t, point0, point1, point2, point3);
	}

}
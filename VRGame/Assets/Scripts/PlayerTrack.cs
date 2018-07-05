using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrack : MonoBehaviour {

    [Tooltip("The transforms of the various positions that define the track")]
	public List<Transform> GuidePoints = new List<Transform>();
    [Tooltip("The number of steps used in interpolation")]
	public int InterpolationSteps;
	[Tooltip("THe way points of the track")]
	[HideInInspector] public List<Transform> WayPoints;

	private void Start()
	{
		WayPoints = GenerateWayPoints();
	}

	public float GetWidth(int index, float t) {
		return 0;
	}

	public float GetLength(int index) {
		return 0;
	}

    /// <summary>
    /// Generates the way points from the guide points.
    /// </summary>
	private List<Transform> GenerateWayPoints() {
		// Container
		var wayPoints = new GameObject("[Generated] WayPoints");
		wayPoints.transform.SetParent(transform);

        // Create all the way points and store the transforms
		Vector3[] points = Spline.GetPoints(GuidePoints, InterpolationSteps);
		List<Transform> transforms = new List<Transform>();

        // Handle Building all waypoints and interpolating the widths
		var firstWayPoint = new GameObject("[Generated] WayPoint 0");
		firstWayPoint.transform.SetParent(wayPoints.transform);
		firstWayPoint.transform.position = GuidePoints[0].position;
		firstWayPoint.AddComponent<WayPoint>().width = GuidePoints[0].GetComponent<WayPoint>().width;
		for (int i = 1; i < points.Length - 1; i++) {
			GameObject wayPoint = new GameObject("[Generated] WayPoint " + i);
			wayPoint.transform.SetParent(wayPoints.transform);
			WayPoint script = wayPoint.AddComponent<WayPoint>();
			script.width = Mathf.Lerp(
				GuidePoints[(i - 2) / (InterpolationSteps + 1) + 1].GetComponent<WayPoint>().width, 
				GuidePoints[(i - 2) / (InterpolationSteps + 1) + 2].GetComponent<WayPoint>().width, 
				(((i - 2) % (InterpolationSteps + 1)) + 1) / (float) (InterpolationSteps + 1));
			wayPoint.transform.position = points[i];
			transforms.Add(wayPoint.transform);
		}
		var lastWayPoint = new GameObject("[Generated] WayPoint " + (points.Length - 1));
		lastWayPoint.transform.SetParent(wayPoints.transform);
		lastWayPoint.transform.position = GuidePoints[GuidePoints.Count - 1].position;
		lastWayPoint.AddComponent<WayPoint>().width = GuidePoints[GuidePoints.Count - 1].GetComponent<WayPoint>().width;

		return transforms;
	}
    
}

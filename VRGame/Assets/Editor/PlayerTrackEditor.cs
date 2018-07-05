using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerTrack))]
public class PlayerTrackEditor : Editor {

	public override void OnInspectorGUI()
	{
		PlayerTrack script = (PlayerTrack)target;
		base.OnInspectorGUI();
		if (GUILayout.Button("Add Transform...")) {
			var wayPoint = new GameObject("WayPoint");
			wayPoint.transform.SetParent(script.transform);
			wayPoint.AddComponent<WayPoint>();
			if (script.GuidePoints.Count > 1) {
				wayPoint.transform.position = script.GuidePoints[script.GuidePoints.Count - 1].position;
				wayPoint.transform.position += (script.GuidePoints[script.GuidePoints.Count - 1].position 
				                                - script.GuidePoints[script.GuidePoints.Count - 2].position).normalized;
			}
			script.GuidePoints.Add(wayPoint.transform);
		}
	}

	public void RenderTrackHandle(SceneView sceneview) {
		PlayerTrack script = (PlayerTrack)target;
		if (script.GuidePoints.Count > 1)
        {
			// Draw the track
			DrawSpline();
            
			Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
			Handles.color = Color.white;
			if (Selection.activeGameObject == script.gameObject) {
				// Draw Position Handles
                for (int i = 0; i < script.GuidePoints.Count; i++)
                {
					EditorGUI.BeginChangeCheck();
                    Vector3 newPosition = Handles.PositionHandle(script.GuidePoints[i].position, Quaternion.identity);
					if (EditorGUI.EndChangeCheck())
                    {
						Undo.RecordObject(script.GuidePoints[i], "Changed Way Point Position");
                        script.GuidePoints[i].position = newPosition;
                    }
					EditorGUI.BeginChangeCheck();
                    float newWidth = Handles.ScaleSlider(script.GuidePoints[i].GetComponent<WayPoint>().width, script.GuidePoints[i].position, Vector3.back, Quaternion.identity, HandleUtility.GetHandleSize(new Vector3(0.1f, 0.1f, 0.1f)), 0.01f);
					if (EditorGUI.EndChangeCheck())
                    {
						Undo.RecordObject(script.GuidePoints[i].GetComponent<WayPoint>(), "Changed Way Point Width");
                        script.GuidePoints[i].GetComponent<WayPoint>().width = newWidth;
                    }
                }
			}
        }
	}

	private void DrawSpline() {
		PlayerTrack script = (PlayerTrack)target;
		Vector3[] guides = new Vector3[script.GuidePoints.Count];
        for (int i = 0; i < guides.Length; i++)
        {
            guides[i] = script.GuidePoints[i].position;
        }
        Vector3[] points = Spline.GetPoints(script.GuidePoints, script.InterpolationSteps);

		// Generate segment indices
        int[] segmentIndices = new int[(points.Length - 1) * 2];
        for (int i = 0; i < points.Length - 1; i++)
        {
            segmentIndices[i * 2] = i;
            segmentIndices[i * 2 + 1] = i + 1;
        }

        // Generate left and right paths
        Vector3[] leftPath = new Vector3[points.Length];
        Vector3[] rightPath = new Vector3[points.Length];
        float firstWidth = script.GuidePoints[0].GetComponent<WayPoint>().width;
        leftPath[0] = points[0] + Vector3.Cross(points[1] - points[0], Vector3.up).normalized * -firstWidth / 2;
        rightPath[0] = points[0] + Vector3.Cross(points[1] - points[0], Vector3.up).normalized * firstWidth / 2;
        if (points.Length > 3)
        {
            float secondWidth = script.GuidePoints[1].GetComponent<WayPoint>().width;
            Vector3 tangent;
            Vector3 previousTangent = points[1] - points[0] + points[2] - points[1];
            if (script.InterpolationSteps > 0)
            {
                for (int i = 1; i < points.Length - 1; i++)
                {
                    float t = (((i - 2) % (script.InterpolationSteps + 1)) + 1) / (float)(script.InterpolationSteps + 1);
                    float width = Mathf.Lerp(
                        script.GuidePoints[(i - 2) / (script.InterpolationSteps + 1) + 1].GetComponent<WayPoint>().width,
                        script.GuidePoints[(i - 2) / (script.InterpolationSteps + 1) + 2].GetComponent<WayPoint>().width,
                        t);
                    tangent = Spline.GetTangent(t, (i - 2) / (script.InterpolationSteps + 1) + 1, guides);
                    if (Vector3.Dot(tangent, previousTangent) < 0)
                    {
                        tangent = previousTangent.normalized + tangent.normalized;
                    }
                    previousTangent = tangent;
                    leftPath[i] = points[i] + Vector3.Cross(tangent, Vector3.up).normalized * -width / 2;
                    rightPath[i] = points[i] + Vector3.Cross(tangent, Vector3.up).normalized * width / 2;
                }
            }
            else
            {
                for (int i = 1; i < points.Length - 1; i++)
                {
                    tangent = points[i] - points[i - 1] + points[i + 1] - points[i];
                    if (Vector3.Dot(tangent, previousTangent) < 0)
                    {
                        tangent = previousTangent.normalized + tangent.normalized;
                    }
                    previousTangent = tangent;
                    float width = script.GuidePoints[i].GetComponent<WayPoint>().width;
                    leftPath[i] = points[i] + Vector3.Cross(tangent, Vector3.up).normalized * -width / 2;
                    rightPath[i] = points[i] + Vector3.Cross(tangent, Vector3.up).normalized * width / 2;
                }
            }
        }
        else if (points.Length == 3)
        {
            float width = script.GuidePoints[1].GetComponent<WayPoint>().width;
            leftPath[1] = points[1] + Vector3.Cross(points[1] - points[0] + points[2] - points[1], Vector3.up).normalized * -width / 2;
            rightPath[1] = points[1] + Vector3.Cross(points[1] - points[0] + points[2] - points[1], Vector3.up).normalized * width / 2;
        }
        float finalWidth = script.GuidePoints[script.GuidePoints.Count - 1].GetComponent<WayPoint>().width;
        leftPath[points.Length - 1] = points[points.Length - 1] + Vector3.Cross(points[points.Length - 1] - points[points.Length - 2], Vector3.up).normalized * -finalWidth / 2;
        rightPath[points.Length - 1] = points[points.Length - 1] + Vector3.Cross(points[points.Length - 1] - points[points.Length - 2], Vector3.up).normalized * finalWidth / 2;
        // Render Lines
		Handles.color = Color.green;
        Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
        Handles.DrawLines(leftPath, segmentIndices);
        Handles.DrawLines(rightPath, segmentIndices);
		Handles.DrawLines(points, segmentIndices);
        
		Handles.color = Color.red;
        Handles.zTest = UnityEngine.Rendering.CompareFunction.Greater;
        Handles.DrawLines(leftPath, segmentIndices);
        Handles.DrawLines(rightPath, segmentIndices);
		Handles.DrawLines(points, segmentIndices);
	}

    void OnEnable()
    {
		SceneView.onSceneGUIDelegate -= RenderTrackHandle;
		SceneView.onSceneGUIDelegate += RenderTrackHandle;
    }
}

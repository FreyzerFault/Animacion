using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class ControlPoint : MonoBehaviour
{
	public Bezier bezier;

	// Start is called before the first frame update
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	
	}
}

// Editor personalizado para Bezier
[CustomEditor(typeof(ControlPoint))]
public class ControlPointEditor : Editor
{
	// Cuando el Punto de Control esta seleccionado
	public void OnSceneGUI()
	{

		ControlPoint cp = target as ControlPoint;
		Bezier bezier = cp.bezier;

		// CONTROL POINTS
		Vector3 p0 = bezier.ControlPoint0.transform.position;
		Vector3 p1 = bezier.ControlPoint1.transform.position;
		Vector3 p2 = bezier.ControlPoint2.transform.position;
		Vector3 p3 = bezier.ControlPoint3.transform.position;

		List<Vector3> controlPoints = new List<Vector3>() { p0, p1, p2, p3 };

		float resolucionBezier = 0.02f;

		bezier._points = bezier.CreateBezierPoints(controlPoints, resolucionBezier);

		// display an orange disc where the object is
		Handles.color = Color.yellow;
		for (int i = 0; i < bezier._points.Count - 1; i++)
		{
			Handles.DrawLine(bezier._points[i], bezier._points[i+1], 5.0f);
		}

		//Handles.DrawBezier(
		//	bezier.ControlPoint0.transform.position,
		//	bezier.ControlPoint3.transform.position,
		//	bezier.ControlPoint1.transform.position,
		//	bezier.ControlPoint2.transform.position,
		//	Color.blue, Texture2D.whiteTexture, 5.0f
		//	);

		// display object "value" in scene
		GUI.color = Color.yellow;
		Handles.Label(bezier._points[bezier._points.Count / 2] + Vector3.up, "Curva de Bezier");
	}
}
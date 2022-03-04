using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

[ExecuteAlways]
public class ControlPoint : DragObject
{
	public Bezier Bezier;

	void OnMouseDrag()
	{
		Bezier.UpdateControlPoints();
		Bezier.UpdateLineRenderer();
	}
}
// Editor personalizado para Bezier
[CustomEditor(typeof(ControlPoint))]
public class ControlPointEditor : Editor
{
	// Guarda la ultima posicion del punto de control, si cambia ya se que tengo que actualizar la curva
	private Vector3 _lastPosition;

	// Cuando el Punto de Control esta seleccionado
	public void OnSceneGUI()
	{
		ControlPoint cp = target as ControlPoint;
		if (cp != null)
		{
			Bezier bezier = cp.Bezier;

			if (_lastPosition != cp.transform.position)
				bezier.UpdateControlPoints();

			// BEZIER:
			Handles.color = Color.yellow;

			decimal t = 0;
			decimal resolution = 0.05m;
			while (t <= 1)
			{
				Handles.DrawLine(bezier.GetBezierPointT(t), bezier.GetBezierPointT(t += resolution), 5.0f);
			}

			// Unity Bezier:

			//Handles.DrawBezier(
			//	bezier.ControlPoint0.transform.position,
			//	bezier.ControlPoint3.transform.position,
			//	bezier.ControlPoint1.transform.position,
			//	bezier.ControlPoint2.transform.position,
			//	Color.blue, Texture2D.whiteTexture, 5.0f
			//	);

			// LABEL
			GUI.color = Color.yellow;
			Handles.Label(bezier.GetBezierPointT(0.5m) + Vector3.up * 20, "Curva de Bezier");

			_lastPosition = cp.transform.position;
		}
	}

}

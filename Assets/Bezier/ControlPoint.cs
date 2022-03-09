using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

[ExecuteAlways]
public class ControlPoint : MonoBehaviour
{
	void Awake()
	{
	}

	void Update()
	{
		// Si se mueve un punto de control actualizamos Bezier
		Bezier bezier = GetComponentInParent<Bezier>();
		if (transform.hasChanged && bezier)
		{
			bezier.UpdateControlPoints();
			bezier.UpdateLineRenderer();
		}
		if (!bezier)
			print("No hay Bezier asignada a " + this.ToString());

		if (Application.isEditor)
		    this.GetComponent<MeshRenderer>().enabled = true;
		    this.GetComponent<MeshRenderer>().enabled = bezier.renderControlPoints;
	}

	void OnMouseDrag()
	{
	}
}

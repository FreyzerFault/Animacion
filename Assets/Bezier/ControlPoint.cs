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

		if (!bezier)
			print("No hay Bezier asignada a " + this.ToString());

		if (transform.hasChanged && bezier)
		{
			bezier.UpdateControlPoints();
			bezier.UpdateLineRenderer();
			transform.hasChanged = false;
		}

		// Se controla si se esconde o no en el juego
		MeshRenderer mesh = this.gameObject.GetComponent<MeshRenderer>();
		if (Application.isPlaying)
			mesh.enabled = bezier.renderControlPoints;
		else
			mesh.enabled = true;
	}

	void OnMouseDrag()
	{
	}
}

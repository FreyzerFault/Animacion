using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObject : MonoBehaviour
{
	protected Vector3 offset;
	private float zCoord;

	void OnMouseDown()
	{
		// Z relativa a la camara
		zCoord = Camera.main.WorldToScreenPoint(transform.position).z;

		// Offset desde el mouse hasta la posicion del objeto
		offset = transform.position - GetMouseWorldPos();
	}

	protected Vector3 GetMouseWorldPos()
	{
		// Pixel Coord
		Vector3 mousePoint = Input.mousePosition;

		// z Coord of GameObject
		mousePoint.z = zCoord;

		// Devolvemos las Coords de la Pantalla a Coords Globales
		return Camera.main.ScreenToWorldPoint(mousePoint);
	}

	void OnMouseDrag()
	{
		// Mover el objeto hacia el cursor
		transform.position = GetMouseWorldPos() + offset;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PickAndPull : MonoBehaviour
{
	[Range(1,100)] public int width = 20;
	[Range(1, 100)] public int height = 20;

	[Space] public bool accumulative;

	[Space]

	[Range(0.5f, 50)] public float deformationStrength = 20f;
	[Range(1.5f, 500)] public float deformationRadius = 100f;
	[Range(-10, 10)] public float deformationScaleFactor= 0;

	[Space]

	[Range(0, 50)] public float maxHeight = 50f;
	[Range(-50, 0)] public float minHeight = -50f;

	[Space]

	public Gradient gradient;

	private MeshFilter meshFilter;
	private MeshRenderer meshRenderer;
	private MeshCollider meshCollider;
	private MeshData data;

	void Start()
	{
		meshFilter = GetComponent<MeshFilter>();
		meshRenderer = GetComponent<MeshRenderer>();
		meshCollider = GetComponent<MeshCollider>();
		
		data = MeshGenerator.generateMesh(width, height);
		UpdateMesh(data);
	}
	
	void Update()
	{
		Vector3 deformation = Vector3.zero;

		// Click Izquierdo: ARRIBA
		if (Input.GetMouseButton((int)MouseButton.LeftMouse))
		{
			deformation = Vector3.up;
		}
		// Click Derecho: ABAJO
		else if (Input.GetMouseButton((int)MouseButton.RightMouse))
		{
			deformation = Vector3.down;
		}

		if (Input.GetMouseButton((int)MouseButton.LeftMouse) || Input.GetMouseButton((int)MouseButton.RightMouse))
		{
			// Raycast para sacar el punto donde esta el raton, relativo al objeto
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.NameToLayer("DeformationMesh")))
			{
				// Punto relativo al objeto
				Vector3 hitPoint = hit.point - meshFilter.transform.position;

				// Hacemos un Pick&Pull (si es acumulativo se actualiza
				MeshData newData = data.PickAndPull(
					hitPoint, deformation * deformationStrength, deformationRadius,
					deformationScaleFactor, maxHeight, minHeight
				);

				// Si es acumulativo modificamos la propia malla original, si no, en el proximo frame volvera a su forma original
				if (accumulative)
					data = newData;

				UpdateMesh(newData);


				vertPos = transform.position + newData.vertices[newData.getNearVertex(hitPoint)];
				cursorPick = hit.point;
			}
		}
	}
	
	void UpdateMesh(MeshData newData)
	{
		meshFilter.mesh = newData.CreateMesh();

		meshRenderer.sharedMaterial.mainTexture = NoiseMapGenerator.GetTexture(newData.getHeightMap(), gradient);

		meshCollider.sharedMesh = meshFilter.mesh;
	}


	private Vector3 vertPos;
	private Vector3 cursorPick;
	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(cursorPick, vertPos);
		Gizmos.DrawSphere(vertPos, 0.1f);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
	public GameObject obstaclePrefab;

	public Vector3 regionTam = new Vector3(10, 0, 10);

	public float numItems = 10;

	private Vector3 heightOffset;

	public float offset = 5;

	void Awake()
	{
		List<Vector3> vertices = new List<Vector3>();
		MeshFilter filter = obstaclePrefab.GetComponent<MeshFilter>();
		Mesh mesh = filter.sharedMesh;
		mesh.GetVertices(vertices);

		// Ajustar la altura al modelo
		float minHeight = 10;
		foreach (Vector3 v in vertices)
		{
			if (minHeight > v.y)
				minHeight = v.y;
		}

		heightOffset = new Vector3(0, -minHeight, 0) * obstaclePrefab.transform.localScale.y;

	}

	// Start is called before the first frame update
	void Start()
	{
		for (int i = 0; i < numItems; i++)
		{
			Vector3 minPos = new Vector3(offset, 0, offset);
			Vector3 maxPos = new Vector3(regionTam.x - offset, 0, regionTam.z - offset);

			// Posicion random dentro del rango a la misma altura
			Vector3 pos = new Vector3(Random.Range(minPos.x, maxPos.x), Random.Range(minPos.y, maxPos.y), Random.Range(minPos.z, maxPos.z));

			GameObject obstacle = Instantiate(obstaclePrefab, transform);
			obstacle.transform.position += pos;
			obstacle.transform.Rotate(Vector3.up, Random.Range(0, 180));

			GiroPermanente giro = obstacle.GetComponent<GiroPermanente>();
			bool clockwise = Random.value < 0.5;
			float rotSpeed = Random.Range(30, 70);
			giro.rotationSpeed = clockwise ? rotSpeed : -rotSpeed;
		}
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}

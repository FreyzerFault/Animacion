using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ObstacleGenerator : MonoBehaviour
{
	public GameObject obstaclePrefab;

	public int level = 1;
	
	public float numItems = 10;
	
	public Vector3 offset = new Vector3(5, 0, 5);

	private Vector3 minPosition, maxPosition;



	void Awake()
	{
	}

	// Start is called before the first frame update
	void Start()
	{
		BoxCollider boxRegion = GetComponentInChildren<BoxCollider>();

		Bounds bounds = boxRegion.bounds;

		maxPosition = bounds.max;
		minPosition = bounds.min;
		maxPosition.y = minPosition.y;
		
		// Sumamos el offset al MINIMO y lo restamos al MAXIMO
		// para hacer unos margenes y que no spawnee medio objeto fuera
		minPosition += offset;
		maxPosition -= offset;

		for (int i = 0; i < numItems; i++)
		{
			// Posicion random dentro del rango a la misma altura
			Vector3 pos = new Vector3(Random.Range(minPosition.x, maxPosition.x), Random.Range(minPosition.y, maxPosition.y), Random.Range(minPosition.z, maxPosition.z));

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

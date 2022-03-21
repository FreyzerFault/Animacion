using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBox : MonoBehaviour
{
	public GameObject SpawnableItem;
	
	public float initialNumItems = 10;

	public Quaternion initialRotation = Quaternion.identity;

	public bool ignoreHeight = false;
	public bool initRandomRotation = false;


	public Vector3 offset = new Vector3(5, 0, 5);
	private Vector3 minPosition, maxPosition;

	// Start is called before the first frame update
	void Start()
	{
		Physics.IgnoreLayerCollision(0, 6);

		BoxCollider boxRegion = GetComponentInChildren<BoxCollider>();

		Bounds bounds = boxRegion.bounds;

		maxPosition = bounds.max;
		minPosition = bounds.min;

		// Usa un plano donde spawnear si ignora la altura
		if (ignoreHeight)
			maxPosition.y = minPosition.y;

		// Sumamos el offset al MINIMO y lo restamos al MAXIMO
		// para hacer unos margenes y que no spawnee medio objeto fuera
		minPosition += offset;
		maxPosition -= offset;

		// Spawn Objects
		for (int i = 0; i < initialNumItems; i++)
		{
			// Posicion random dentro del rango a la misma altura
			Vector3 pos = new Vector3(Random.Range(minPosition.x, maxPosition.x), Random.Range(minPosition.y, maxPosition.y), Random.Range(minPosition.z, maxPosition.z));

			GameObject item = Instantiate(SpawnableItem, this.transform);
			item.transform.position = pos;
			if (initRandomRotation)
				item.transform.Rotate(Vector3.up, Random.Range(0, 180));
		}
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}

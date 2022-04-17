using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Utiliza POOLING
// Spawnea objetos en la posicion dada

public class Spawner : MonoBehaviour
{
	public GameObject ObjectPrefab;

	public int initialNumItems = 0;
	
	// POOLING
	protected Stack<GameObject> pool = new Stack<GameObject>();

	// Start is called before the first frame update
	protected virtual void Awake()
	{
		// Puebla la Pool con un numero inicial de items
		LoadPool(initialNumItems);
	}

	// Carga la pool con un numero de objetos inicial
	public void LoadPool(int numObjects)
	{
		// Spawn Objects
		for (int i = 0; i < numObjects; i++)
		{
			Generate();
		}
	}

	// Genera el Objeto pero sin activarlo
	public GameObject Generate()
	{
		GameObject obj = Instantiate(ObjectPrefab, transform);
		obj.SetActive(false);
		pool.Push(obj);

		return obj;
	}

	// Guarda el objeto en la pool para volverlo a spawnear luego
	// Esto debe llamarlo el propio objeto que debe heredar de Spawneable
	public void Pool(GameObject item)
	{
		item.SetActive(false);
		pool.Push(item);
	}

	// Destruye todos los objetos del pool
	public void Clear()
	{
		for (int i = 0; i < pool.Count; i++)
		{
			Destroy(pool.Pop());
		}
	}

	// Spawnea el objeto sacandolo de la Pool
	public virtual GameObject Spawn(
		Vector3? position = null,
		Quaternion? rotation = null
		)
	{
		// Si esta vacio creamos otro
		if (pool.Count == 0)
			Generate();

		// Sacar de la Pool
		GameObject item = pool.Pop();

		// Si no se han asignado posicion o rotacion por defecto sera en el centro y su rotacion por defecto
		item.transform.SetPositionAndRotation(
			position ??= transform.position,
			rotation ??= ObjectPrefab.transform.rotation
		);

		item.SetActive(true);

		return item;
	}


	// Posicion Random en una region
	protected Vector3 GetRandomPos(Vector3 min, Vector3 max)
	{
		return new Vector3(
			Random.Range(min.x, max.x),
			Random.Range(min.y, max.y),
			Random.Range(min.z, max.z)
		);
	}

	// Rotacion random sobre el eje Y
	protected Quaternion GetRandomRot()
	{
		return Quaternion.Euler(0, Random.Range(0, 180), 0);
	}
}

using System.Collections.Generic;
using UnityEngine;

// Caja de Colision que solo sirve para definir una region con su Caja
// Spawnea objetos en esa region

[RequireComponent(typeof(BoxCollider))]
public class SpawnerBox : MonoBehaviour
{
	public GameObject SpawnableObject;

	public int initialNumItems = 0;
	public bool instantSpawn = false;

	public bool ignoreHeight = false;

	public Vector3 offset = new Vector3(0, 0, 0);

	private BoxCollider box;

	// POOLING
	private Stack<GameObject> pool = new Stack<GameObject>();

	// Start is called before the first frame update
	void Awake()
	{
		box = GetComponent<BoxCollider>();

		// Ignora las colisiones de cualquier objeto
		Physics.IgnoreLayerCollision(0, gameObject.layer);
		
		// Puebla la Pool con un numero inicial de items
		LoadPool(initialNumItems);

		if (instantSpawn)
		{
			for (int i = 0; i < initialNumItems; i++)
			{
				SpawnRandom();
			}
		}
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
		GameObject obj = Instantiate(SpawnableObject, transform);
		obj.SetActive(false);
		pool.Push(obj);

		return obj;
	}

	// Spawnea el objeto de forma random dentro de la caja
	public GameObject SpawnRandom(bool randomRotation = true)
	{
		return Spawn(GetRandomPos(), randomRotation ? GetRandomRot() : (Quaternion?)null);
	}

	// Spawnea el objeto sacandolo de la Pool
	public GameObject Spawn(Vector3? position = null, Quaternion? rotation = null)
	{
		// Si esta vacio creamos otro
		if (pool.Count == 0)
			pool.Push(Generate());

		// Sacar de la Pool
		GameObject item = pool.Pop();
		item.SetActive(true);

		// Si no se han asignado posicion o rotacion por defecto sera en el centro y su rotacion por defecto
		position ??= box.center;
		rotation ??= SpawnableObject.transform.rotation;

		item.transform.SetPositionAndRotation((Vector3)position, (Quaternion)rotation);

		// Ignora colisiones entre el item y la caja del Spawner
		Physics.IgnoreCollision(item.GetComponent<Collider>(), GetComponent<Collider>());

		return item;
	}

	// Spawnea el objeto disparandolo con una fuerza, orientado o no en la direccion de la fuerza
	public GameObject Shoot(Vector3 force, bool oriented = true, bool random = true)
	{
		GameObject item = random ? SpawnRandom() : Spawn();

		if (item.GetComponent<Rigidbody>())
			item.GetComponent<Rigidbody>().AddForce(force);
		else
			print("Se ha intentado disparar el objeto que no tiene RigidBody: " + item);

		if (oriented)
			item.transform.rotation = Quaternion.LookRotation(force.normalized);

		return item;
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



	// Posicion Random en la Bounding Box pero aplicando un padding con offset
	private Vector3 GetRandomPos()
	{
		Vector3 maxPosition = box.bounds.max;
		Vector3 minPosition = box.bounds.min;

		// Sumamos el offset al MINIMO y lo restamos al MAXIMO
		// para hacer unos margenes y que no spawnee medio objeto fuera
		minPosition += offset;
		maxPosition -= offset;

		// Usa el suelo de la caja si ignora la altura
		if (ignoreHeight)
			maxPosition.y = minPosition.y;

		return new Vector3(
			Random.Range(minPosition.x, maxPosition.x),
			Random.Range(minPosition.y, maxPosition.y),
			Random.Range(minPosition.z, maxPosition.z)
		);
	}
	
	// Rotacion random sobre el eje Y
	private Quaternion GetRandomRot()
	{
		return Quaternion.Euler(0, Random.Range(0, 180), 0);
	}

	public Vector3 getCenter()
	{
		BoxCollider b = GetComponent<BoxCollider>();
		return b.center + transform.position;
	}
}

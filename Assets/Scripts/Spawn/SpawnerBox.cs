using System.Collections.Generic;
using UnityEngine;

// U

[RequireComponent(typeof(BoxCollider))]
public class SpawnerBox : Spawner
{
	public bool instantSpawn = false;

	public bool ignoreHeight = false;

	public Vector3 offset = new Vector3(0, 0, 0);

	private BoxCollider box;

	// Start is called before the first frame update
	protected override void Awake()
	{
		base.Awake();

		// Ignora las colisiones de cualquier objeto de tipo Spawner
		Physics.IgnoreLayerCollision(0, gameObject.layer);

		box = GetComponent<BoxCollider>();

		if (instantSpawn)
		{
			for (int i = 0; i < initialNumItems; i++)
			{
				SpawnRandom();
			}
		}
	}

	// Spawnea el objeto de forma random dentro de la caja
	public GameObject SpawnRandom(bool randomRotation = true)
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

		return Spawn(
			GetRandomPos(minPosition, maxPosition),
			randomRotation
				? GetRandomRot()
				: (Quaternion?)null
			);
	}

	// Spawnea el objeto sacandolo de la Pool
	public override GameObject Spawn(Vector3? position = null, Quaternion? rotation = null)
	{
		// Lo spawnwea en el centro si no se ha elegido posicion
		GameObject obj = base.Spawn(position ?? getCenter(), rotation);

		// Ignora colisiones entre el item y la caja del Spawner
		Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Spawner"), 0);

		return obj;
	}

	public Vector3 getCenter()
	{
		BoxCollider b = GetComponent<BoxCollider>();
		return b.center + transform.position;
	}
}

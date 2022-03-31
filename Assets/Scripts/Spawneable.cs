using UnityEngine;

// Objeto que puede usarse en un Pool
// Se puede spawnear (setActive = true)
// Se puede despawner (setActive = false)

public abstract class Spawneable : MonoBehaviour
{
	protected SpawnerBox spawner;

	protected void Awake()
	{
		if (GetComponentInParent<SpawnerBox>())
			spawner = GetComponentInParent<SpawnerBox>();
	}

	public abstract void OnEnable();

	public abstract void OnDisable();

	// Cuando se destruye el objeto se devuelve a la pool
	protected void Destroy()
	{
		if (spawner)
			spawner.Pool(gameObject);
		else
			print("El objeto no esta volviendo a su pool: " + ToString());
	}
}

using UnityEngine;

// Objeto que puede usarse en un Pool
// Se puede spawnear (setActive = true)
// Se puede despawner (setActive = false)

public abstract class Spawneable : MonoBehaviour
{
	protected Spawner spawner;

	protected virtual void Awake()
	{
		if (GetComponentInParent<Spawner>())
			spawner = GetComponentInParent<Spawner>();
	}

	protected abstract void OnEnable();

	protected virtual void OnDisable()
	{
		Destroy();
	}

	// Cuando se destruye el objeto se devuelve a la pool
	public virtual void Destroy()
	{
		if (spawner)
			spawner.Pool(gameObject);
		else
			print("El objeto no esta volviendo a su pool: " + ToString());
	}
}

using UnityEngine;

// Permite volver a crear el objeto de 0 pero funciona como un Singleton
public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
{
	public static T Instance { get; private set; }
	protected virtual void Awake() => Instance = this as T;

	protected virtual void OnApplicationQuit()
	{
		Instance = null;
		Destroy(gameObject);
	}
}

// Singleton se elimina al cambiar de escena
public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour
{
	protected override void Awake()
	{
		if (Instance != null)
			Destroy(gameObject);
		base.Awake();
	}
}

// SingletonPersistent se guarda entre Escenas
public abstract class SingletonPersistent<T> : Singleton<T> where T : MonoBehaviour
{
	protected override void Awake()
	{
		if (Instance != null)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);

		base.Awake();
	}
}

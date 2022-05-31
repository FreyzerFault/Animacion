using UnityEngine;

// Permite volver a crear el objeto de 0 pero funciona como un Singleton
public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
{
	public static T Instance { get; protected set; }

	//protected StaticInstance()
	//{
	//	Instance = this as T;
	//}
	protected virtual void Awake()
	{
		if (Instance != null)
			if (Application.isPlaying)
				Destroy(Instance);
			else
				DestroyImmediate(Instance);
		
		Instance = this as T;
	}

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
			if (Application.isPlaying)
				Destroy(gameObject);
			else
				DestroyImmediate(gameObject);
		else
			Instance = this as T;
	}
}

// SingletonPersistent se guarda entre Escenas
public abstract class SingletonPersistent<T> : Singleton<T> where T : MonoBehaviour
{
	protected override void Awake()
	{
		base.Awake();

		DontDestroyOnLoad(Instance);
	}
}

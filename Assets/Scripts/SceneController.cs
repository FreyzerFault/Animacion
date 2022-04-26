using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SceneController : SingletonPersistent<SceneController>
{
	// Modo de Juego Inicial de cada Escena
	private static Dictionary<int, GameMode> SceneInitGameMode = new Dictionary<int, GameMode>()
	{
		{0, GameMode.Default },
		{1, GameMode.Default },
		{2, GameMode.Default },
		{3, GameMode.Default },
		{4, GameMode.Default },
		{5, GameMode.Default },
		{6, GameMode.Default },
		{7, GameMode.Default },
	};

	protected override void Awake()
	{
		base.Awake();

		SceneManager.sceneLoaded += SceneController.OnSceneLoaded;
	}

	// Funcion que se ejecutara cada vez que se cargue una Escena
	public static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		// Cambia el modo de juego al propio inicial de la escena
		GameManager.Mode =
			SceneInitGameMode.ContainsKey(scene.buildIndex)
				? SceneInitGameMode[scene.buildIndex]
				: GameMode.Default;
	}

	public static void SetSceneInitGameMode(int i, GameMode mode)
	{
		if (SceneInitGameMode.ContainsKey(i))
			SceneInitGameMode[i] = mode;
		else
			SceneInitGameMode.Add(i, mode);
	}

	public static void ReloadScene()
	{
		LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public static void SwitchScene(int i)
	{
		if (i != SceneManager.GetActiveScene().buildIndex)
		{
			LoadScene(i);
		}
	}

	private static void LoadScene(int i)
	{
		SceneManager.LoadScene(i);
	}
}

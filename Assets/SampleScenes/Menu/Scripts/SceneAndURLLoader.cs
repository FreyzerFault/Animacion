using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneAndURLLoader : MonoBehaviour
{
	private PauseMenu m_PauseMenu;


	private void Awake ()
	{
		m_PauseMenu = GetComponentInChildren <PauseMenu> ();
	}


	public void SceneLoad(string sceneName)
	{
		m_PauseMenu.Pause(false);
		SceneManager.LoadScene(sceneName);
	}


	public void LoadURL(string url)
	{
		Application.OpenURL(url);
	}
}


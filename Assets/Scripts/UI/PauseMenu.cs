using System;
using UnityEngine;
using UnityEngine.UI;

// SINGLETON

public class PauseMenu : SingletonPersistent<PauseMenu>
{
	public Toggle m_MenuToggle;
	public bool m_Paused;

	public GameObject buttonPanel;

	public GameMode lastGameMode;

	protected override void Awake()
	{
		base.Awake();

		m_MenuToggle = GetComponent<Toggle>();
	}

	public void OnValueChanged ()
	{
		m_Paused = m_MenuToggle.isOn;

		// Cambio de Modo de Juego
		if (m_Paused)
		{
			lastGameMode = GameManager.Mode; // se guarda el ultimo Modo
			GameManager.Mode = GameMode.PauseMenu;
		}
		else
		{
			GameManager.Mode = lastGameMode;
		}

		// Activar/Desactivar los botones
		buttonPanel.SetActive(m_Paused);
	}

	
	void Update()
	{
		if(Input.GetKeyUp(KeyCode.Escape))
			m_MenuToggle.isOn = !m_MenuToggle.isOn;
	}

	public void Pause( bool pause = true )
	{
		m_MenuToggle.isOn = pause;
	}

	public static void QuitGame()
	{
		GameManager.QuitGame();
	}

	public static void SwitchScene(int i)
	{
		Debug.Log("Scene changed to " + i);
		Instance.m_MenuToggle.isOn = false;
		SceneController.SwitchScene(i);
	}
}

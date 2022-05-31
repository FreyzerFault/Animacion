using System;
using UnityEngine;
using UnityStandardAssets.Cameras;

[Serializable]
public enum GameMode
{
	Default,
	ThirdPerson,
	FirstPerson,
	PauseMenu,
}

public class GameManager : Singleton<GameManager>
{

	public GameObject FirstPersonChar;
	public GameObject ThirdPersonChar;

	public Camera FirstPersonCamera;
	public Camera ThirdPersonCamera;

	public FreeLookCam freeLookCam;

	private GameObject _player;
	private Camera _mainCamera;
	public GameMode _mode;

	public float _TimeScaleRef = 1;
	public float _VolumeRef = 1;

	public static GameObject Player
	{
		get {
			if (Instance._player != null)
				return Instance._player;
			return Instance._player = GameObject.FindGameObjectWithTag("Player");
		}
		set {
			if (Instance._player != null)
				Instance._player.SetActive(false);

			value.SetActive(true);
			Instance._player = value;
		}
	}


	public static Camera MainCamera
	{
		get {
			if (Instance._mainCamera != null)
				return Instance._mainCamera;
			return Instance._mainCamera = Camera.main;
		}
		set {
			if (Instance._mainCamera != null) Instance._mainCamera.gameObject.SetActive(false);

			value.gameObject.SetActive(true);
			Instance._mainCamera = value;
		}
	}

	public static GameMode Mode
	{
		get => Instance._mode;
		set {
			// Salir del Modo anterior
			switch (Instance._mode)
			{
				case GameMode.ThirdPerson:
					Instance.freeLookCam.gameObject.SetActive(true);
					break;
				case GameMode.FirstPerson:

					break;
				case GameMode.PauseMenu:
					Time.timeScale = Instance._TimeScaleRef;
					AudioListener.volume = Instance._VolumeRef;
					break;
				default:
					break;
			}

			// Entrar en el Modo
			switch (value)
			{
				case GameMode.ThirdPerson:
					Cursor.lockState = CursorLockMode.Locked;
					Cursor.visible = false;

					Player = Instance.ThirdPersonChar;
					MainCamera = Instance.ThirdPersonCamera;
					Instance.freeLookCam.gameObject.SetActive(true);

					break;

				case GameMode.FirstPerson:
					Cursor.lockState = CursorLockMode.Locked;
					Cursor.visible = false;

					Player = Instance.FirstPersonChar;
					MainCamera = Instance.FirstPersonCamera;

					break;

				case GameMode.PauseMenu:
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;

					Instance._TimeScaleRef = Time.timeScale;
					Time.timeScale = 0f;

					Instance._VolumeRef = AudioListener.volume;
					AudioListener.volume = 0f;
					break;

				case GameMode.Default:
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
					break;
			}

			Instance._mode = value;
		}
	}

	public static void QuitGame()
	{
		Application.Quit();
	}

	protected override void Awake()
	{
		base.Awake();
	}

	void Update()
	{
		if (FirstPersonChar != null && ThirdPersonChar != null)
		{
			if (Input.GetKeyUp(KeyCode.Tab))
			{
				Mode = Mode switch
				{
					GameMode.FirstPerson => GameMode.ThirdPerson,
					GameMode.ThirdPerson => GameMode.FirstPerson,
					_ => Mode
				};
			}

			if (Mode == GameMode.ThirdPerson)
				FirstPersonChar.transform.SetPositionAndRotation(
					ThirdPersonChar.transform.position,
					ThirdPersonChar.transform.rotation
					);
			else if (Mode == GameMode.FirstPerson)
				ThirdPersonChar.transform.SetPositionAndRotation(
					FirstPersonChar.transform.position,
					FirstPersonChar.transform.rotation
				);
		}
	}
}

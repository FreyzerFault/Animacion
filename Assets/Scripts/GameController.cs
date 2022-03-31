using UnityEngine;

public static class GameController
{
	private static GameObject _player;

	public static GameObject Player
	{
		get
		{
			if (_player != null)
				return _player;
			return _player = GameObject.FindGameObjectWithTag("Player");
		}
		set
		{
			if (_player != null && _player.tag == "Player")
				_player.gameObject.tag = "Untagged";

			value.tag = "Player";
			_player = value;
		}
	}

	private static Camera _mainCamera;

	public static Camera MainCamera
	{
		get
		{
			if (_mainCamera != null)
				return _mainCamera;
			return _mainCamera = Camera.main;
		}
		set
		{
			if (_mainCamera != null)
				_mainCamera.gameObject.SetActive(false);
			
			value.gameObject.SetActive(true);
			_mainCamera = value;
		}
	}
}

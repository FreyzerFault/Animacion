using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainCameraButton : MonoBehaviour
{
	private Camera mainCamera;
	public Camera upCamera;
	public Camera sideCamera;

	public Button ButtonMainCamera;
	public Button ButtonUpCamera;
	public Button ButtonSideCamera;

	public void Awake()
	{
		mainCamera = Camera.main;
	}

	public void onClick()
	{
		mainCamera.gameObject.SetActive(true);
		upCamera.gameObject.SetActive(false);
		sideCamera.gameObject.SetActive(false);
		ButtonMainCamera.gameObject.SetActive(false);
		ButtonUpCamera.gameObject.SetActive(true);
		ButtonSideCamera.gameObject.SetActive(true);
	}
}

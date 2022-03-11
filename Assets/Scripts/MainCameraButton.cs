using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainCameraButton : MonoBehaviour
{
	public Camera mainCamera;
	public Camera upCamera;
	public Camera sideCamera;

	public GameObject ButtonMainCamera;
	public GameObject ButtonUpCamera;
	public GameObject ButtonSideCamera;

	public void Awake()
	{
	}

	public void onClick()
	{
		mainCamera.enabled = true;
		upCamera.enabled = false;
		sideCamera.enabled = false;
		//ButtonMainCamera.SetActive(false);
		//ButtonUpCamera.SetActive(true);
		//ButtonSideCamera.SetActive(true);
	}
}

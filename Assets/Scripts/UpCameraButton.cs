using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpCameraButton : MonoBehaviour
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
		mainCamera.enabled = false;
		upCamera.enabled = true;
		sideCamera.enabled = false;
		//ButtonMainCamera.SetActive(true);
		//ButtonUpCamera.SetActive(false);
		//ButtonSideCamera.SetActive(true);
	}

	

}

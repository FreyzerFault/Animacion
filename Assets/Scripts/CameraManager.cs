using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
	private Dictionary<int, Camera> cameras;
	private Camera activeCamera;

	void Awake()
	{
		cameras = new Dictionary<int, Camera>();
		List<Camera> camList = new List<Camera>(GetComponentsInChildren<Camera>());

		for (int i = 0; i < camList.Count; i++)
		{
			cameras.Add(i, camList[i]);
			camList[i].gameObject.SetActive(i == 0);
		}

		activeCamera = cameras[0];
	}

	public void switchCamera(int i)
	{
		if (i >= 0 && i < cameras.Count)
		{
			activeCamera.gameObject.SetActive(false);
			activeCamera = cameras[i];
			activeCamera.gameObject.SetActive(true);
		}
		else
			print("Camera " + i + " no existe");
	}
}

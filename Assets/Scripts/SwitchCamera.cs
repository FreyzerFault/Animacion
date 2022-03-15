using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCamera : MonoBehaviour
{
	public int camIndex;

	private CameraManager camManager;

	void Awake()
	{
		camManager = GameObject.FindObjectOfType<CameraManager>();
	}

	public void onClick()
	{
		camManager.switchCamera(camIndex);
	}
}

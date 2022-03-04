using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{
	}

	// Update is called once per frame
	void LateUpdate()
	{
		Vector3 camPos = Camera.main.transform.position;
		Vector3 targetPos = transform.position;
		Quaternion camRotation = Camera.main.transform.rotation;
		Camera.main.transform.rotation = Quaternion.Slerp(camRotation, Quaternion.LookRotation(targetPos - camPos), Time.deltaTime * 5);
	}
}

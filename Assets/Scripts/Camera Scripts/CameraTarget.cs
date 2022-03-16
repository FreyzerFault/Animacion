using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
	public GameObject target;

	// Start is called before the first frame update
	void Start()
	{
	}

	// Update is called once per frame
	void LateUpdate()
	{
		if (target)
		{
			Vector3 camPos = transform.position;
			Quaternion camRotation = transform.rotation;
			Vector3 targetPos = target.transform.position;
			transform.rotation = Quaternion.Slerp(camRotation, Quaternion.LookRotation(targetPos - camPos),
				Time.deltaTime * 5);
		}
	}
}

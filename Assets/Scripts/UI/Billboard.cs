using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Un objeto Billboard va estar siempre mirando a la camara
public class Billboard : MonoBehaviour
{
	public Transform cam;

	void Start()
	{
		cam = Camera.main.transform;
	}

	// Update is called once per frame
	void LateUpdate()
	{
		// Apunta a la camara
		transform.LookAt(cam.forward + transform.position);
	}
}

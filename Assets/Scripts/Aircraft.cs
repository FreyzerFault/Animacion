using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aircraft : MonoBehaviour
{
	public Bezier bezier;

	public float speed;
	private float espacioRecorrido = 0;

	// Start is called before the first frame update
	void Start()
	{
		transform.position = bezier.GetBezierPointT(0);
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		// Reset a la posicion inicial si se ha completado el recorrido
		if (espacioRecorrido > bezier.GetLenght())
		{
			transform.position = bezier.GetBezierPointT(0);
			espacioRecorrido = 0;
		}
		
		float deltaTime = Time.fixedDeltaTime; // Tiempo de frame
		float espacio = speed * deltaTime; // Espacio que tiene que recorrer
		espacioRecorrido += espacio;

		// Espacio normalizado a t
		float t = bezier.GetT(espacioRecorrido);
		
		transform.position = bezier.GetBezierPointT(t);

		print("t = " + ((t - bezier.GetT(espacioRecorrido - espacio))) + "; espacio = " + espacio);
		//print(bezier.GetLenght());
	}
}

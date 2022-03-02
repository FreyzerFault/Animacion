using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aircraft : MonoBehaviour
{
	public Bezier bezier;

	public float speed;

	// Start is called before the first frame update
	void Start()
	{
		transform.position = bezier.GetBezierPointT(0);
	}
	/*
	// Update is called once per frame
	void FixedUpdate()
	{
		// Reset a la posicion inicial si se ha completado el recorrido
		if (espacioAcumulado > bezier.GetLenght())
		{
			Reset();
		}
		
		float deltaTime = Time.fixedDeltaTime; // Tiempo de frame
		MoveInBezier(speed, deltaTime);
	}
	*/

	void Update()
	{
		// Reset a la posicion inicial si se ha completado el recorrido
		if (_espacioAcumulado > bezier.GetLenght())
		{
			Reset();
		}

		float deltaTime = Time.deltaTime; // Tiempo de frame
		MoveInBezier(speed, deltaTime);
	}

	// Espacio recorrido en la Curva Bezier
	private float _espacioAcumulado = 0;

	private void Reset()
	{
		transform.position = bezier.GetBezierPointT(0);
		_espacioAcumulado = 0;
	}

	// Ecuacion del espacio = velocidad * tiempo
	private float MoveInBezier(float velocidad, float tiempo)
	{
		float espacio = velocidad * tiempo; // Espacio que tiene que recorrer
		_espacioAcumulado += espacio;

		// Espacio normalizado a t (parametro t en la curva con esa longitud con el punto inicial en un margen de error)
		float t = bezier.GetT(_espacioAcumulado, 0.00001f);

		// Mueve el objecto a la posicion de t en la curva
		transform.position = bezier.GetBezierPointT(t);

		//print("Intervalo t = " + ((t - bezier.GetT(_espacioAcumulado - espacio))) + "; Espacio recorrido = " + espacio);

		return t;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierMovable : MonoBehaviour
{
	public Bezier bezier;

	public float speed;

	private decimal _espacioAcumulado = 0;

	public bool inBezier = true;

	// Start is called before the first frame update
	void Start()
	{
		transform.position = bezier.GetBezierPointT(0);
	}

	// FixedUpdate is called 50 times per second
	protected void Update()
	{
		// Reset a la posicion inicial si se ha completado el recorrido
		if (_espacioAcumulado > bezier.GetLenght())
		{
			Reset();
		}

		if (inBezier)
			MoveInBezier(speed, Time.deltaTime);
	}

	private void Reset()
	{
		transform.position = bezier.GetBezierPointT(0);
		_espacioAcumulado = 0;
	}

	// Ecuacion del espacio = velocidad * tiempo
	private decimal MoveInBezier(float velocidad, float tiempo)
	{
		decimal espacio = (decimal)velocidad * (decimal)tiempo; // Espacio que tiene que recorrer
		_espacioAcumulado += espacio;

		// Espacio normalizado a t (parametro t en la curva con esa longitud con el punto inicial en un margen de error)
		decimal t = bezier.GetT(_espacioAcumulado);

		// Mueve el objecto a la posicion de t en la curva
		transform.position = bezier.GetBezierPointT(t);

		//print("Intervalo t = " + ((t - bezier.GetT(_espacioAcumulado - espacio))) + "; Espacio recorrido = " + espacio);

		return t;
	}

	void RotateTowardsCurve()
	{

	}
}

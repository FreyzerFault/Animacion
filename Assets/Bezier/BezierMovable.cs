using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierMovable : MonoBehaviour
{
	public Bezier Bezier;

	public bool InBezier = true;
	public bool easeInOutActivated = false;
	
	public float InitSpeed = 0;
	public float Acceleration = 0;

	// EASE IN + EASE OUT
	public float easeInAcceleration = 0;
	public float easeOutDecceleration = 0;
	// Fraccion de la curva con Ease in Ease out [0,1]
	public float easeIn = 0; 
	public float easeOut = 0;

	private decimal _espacioAcumulado = 0;

	private float timeInBezier = 0;

	// Start is called before the first frame update
	private void Start()
	{
		if (Bezier)
			ResetToInit();

		if (easeIn + easeOut >= 1)
		{
			print("Ease In y Ease Out no son coherentes (superan la fraccion de curva total)");
			easeIn = 0;
			easeOut = 0;
		}
	}

	private bool outOfEase = false;
	private decimal espacioIntermedio = 0;

	// FixedUpdate is called 50 times per second
	protected void Update()
	{
		// Reset a la posicion inicial si se ha completado el recorrido
		if (_espacioAcumulado > Bezier.GetLenght())
			ResetToInit();

		if (InBezier)
		{
			if (easeInOutActivated)
			{
				float fraccionRecorrida = (float)_espacioAcumulado / (float)Bezier.GetLenght();
				if (fraccionRecorrida <= easeIn)
				{
					MoveInBezier(easeInAcceleration, InitSpeed, Time.deltaTime);
				}
				if (fraccionRecorrida > easeIn && fraccionRecorrida < 1 - easeOut)
				{
					if (!outOfEase)
					{
						outOfEase = true;
						InitSpeed = easeInAcceleration * timeInBezier;
						espacioIntermedio = _espacioAcumulado;
					}
					MoveInBezier(0, InitSpeed, Time.deltaTime, espacioIntermedio);
				}

				if (fraccionRecorrida >= 1 - easeOut)
				{
					if (outOfEase)
					{
						outOfEase = false;
						espacioIntermedio = _espacioAcumulado;
					}
					MoveInBezier(-easeOutDecceleration, InitSpeed, Time.deltaTime, espacioIntermedio);
				}
			}
			else
			{
				MoveInBezier(Acceleration, InitSpeed, Time.deltaTime);
			}
		}
	}

	private void ResetToInit()
	{
		transform.position = Bezier.GetBezierPointT(0);
		_espacioAcumulado = 0;
		timeInBezier = 0;
	}

	// Ecuacion del espacio = velocidad * tiempo
	private decimal MoveInBezier(float a, float v0, float deltaTime, decimal s0 = 0)
	{
		timeInBezier += deltaTime;

		// s = v0 · t + (a · t^2) / 2
		_espacioAcumulado = s0 + (decimal)(v0 * timeInBezier + (a * Mathf.Pow(timeInBezier, 2)) / 2);

		// Espacio normalizado a t (parametro t en la curva con esa longitud con el punto inicial en un margen de error)
		decimal t = Bezier.GetT(_espacioAcumulado);

		// Mueve el objecto a la posicion de t en la curva
		transform.position = Bezier.GetBezierPointT(t);

		//print("Intervalo t = " + ((t - bezier.GetT(_espacioAcumulado - espacio))) + "; Espacio recorrido = " + espacio);

		return t;
	}

	void RotateTowardsCurve()
	{

	}
}

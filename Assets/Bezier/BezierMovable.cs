using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierMovable : MonoBehaviour
{
	public Bezier Bezier;
	public bool InBezier = true;

	public float InitialSpeed = 0;
	public float InitialAcceleration = 0;

	// EASE IN + EASE OUT
	public bool easeInOutActivated = false;

	public float easeInAcceleration = 0;
	public float easeOutDeceleration = 0;
	// Fraccion de la curva con Ease in Ease out [0,1]
	public float EaseInSection = 0; 
	public float EaseOutSection = 0;

	private float _espacioAcumulado = 0;


	// Start is called before the first frame update
	private void Start()
	{
		if (Bezier)
			ResetToInit();

		if (EaseInSection + EaseOutSection >= 1)
		{
			print("Ease In y Ease Out no son coherentes (superan la fraccion de curva total)");
			EaseInSection = 0;
			EaseOutSection = 0;
		}
	}

	private float timeInSection = 0;
	private float lastSectionSpace = 0; // Espacio recorrido localmente a un trazo de la curva
	private float lastSectionSpeed = 0;
	private bool onEase = true;

	// FixedUpdate is called 50 times per second
	protected void Update()
	{
		anotherUpdate();
	}

	private void anotherUpdate()
	{
		// Reset a la posicion inicial si se ha completado el recorrido
		if (_espacioAcumulado > Bezier.GetLenght())
			ResetToInit();

		if (InBezier)
		{
			timeInSection += Time.deltaTime;

			if (easeInOutActivated)
			{
				float fraccionRecorrida = _espacioAcumulado / Bezier.GetLenght();
				if (fraccionRecorrida < EaseInSection)
				{
					_espacioAcumulado = MoveEaseIn(timeInSection, easeInAcceleration, InitialSpeed);
				}
				if (fraccionRecorrida >= EaseInSection && fraccionRecorrida <= 1 - EaseOutSection)
				{
					// Sale del EaseIn
					if (onEase)
					{
						onEase = false;
						// Calcula Velocidad al principio del tramo (a*t + v0)
						lastSectionSpeed = easeInAcceleration * timeInSection + InitialSpeed;
						// Reinicia tiempo
						timeInSection = Time.deltaTime;
						// Acumular el espacio en el tramo anterior
						lastSectionSpace = _espacioAcumulado;
					}
					_espacioAcumulado = MoveConstantSpeed(timeInSection, lastSectionSpeed, lastSectionSpace);
				}

				if (fraccionRecorrida > 1 - EaseOutSection)
				{
					// Entra en el Ease Out
					if (!onEase)
					{
						onEase = true;
						// La velocidad inicial del tramo es la misma
						// Reinicia el tiempo
						timeInSection = Time.deltaTime;
						// Acumular el espacio en el tramo anterior
						lastSectionSpace = _espacioAcumulado;
					}
					_espacioAcumulado = MoveEaseOut(timeInSection, easeOutDeceleration, lastSectionSpeed, lastSectionSpace);
				}
			}
			else // Sin Ease In Ease Out
			{
				MoveConstantSpeed(Time.deltaTime, InitialSpeed, InitialAcceleration);
			}
		}
	}

	private void ResetToInit()
	{
		transform.position = Bezier.GetBezierPointT(0);
		_espacioAcumulado = 0;
		timeInSection = 0;
		lastSectionSpace = 0;
		lastSectionSpeed = InitialSpeed;
	}

	// Ecuacion del espacio = velocidad * tiempo
	private float MoveInBezier(float time, float initialSpeed, float initialSpace = 0, float acceleration = 0)
	{
		// s = v0 · t + (a · t^2) / 2
		float espacio = (initialSpeed * time + (acceleration * Mathf.Pow(time, 2)) / 2) + initialSpace;
		//print("Espacio Acumulado = " + espacio + " = " + initialSpeed + "·" + time + " + "
		//	+ "1/2 · " + acceleration + "·" + time + "^2" + " + " + initialSpace);

		// Espacio normalizado a t (parametro t en la curva con esa longitud con el punto inicial en un margen de error)
		decimal t = Bezier.GetT((decimal)espacio);

		// Mueve el objecto a la posicion de t en la curva
		transform.position = Bezier.GetBezierPointT(t);

		//print("Intervalo t = " + ((t - bezier.GetT(_espacioAcumulado - espacio))) + "; Espacio recorrido = " + espacio);

		return espacio;
	}

	private float MoveEaseIn(float time, float acceleration, float initialSpeed = 0)
	{
		return MoveInBezier(time, initialSpeed, 0, acceleration);
	}

	private float MoveConstantSpeed(float time, float speed, float initialSpace = 0)
	{
		return MoveInBezier(time, speed, initialSpace);
	}

	private float MoveEaseOut(float time, float deceleration, float initialSpeed, float initialSpace = 0)
	{
		return MoveInBezier(time, initialSpeed, initialSpace, -deceleration);
	}

	void RotateTowardsCurve()
	{

	}
}

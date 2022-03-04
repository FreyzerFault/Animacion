using System;
using System.Numerics;
using TMPro;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class BezierMovable : MonoBehaviour
{
	public Bezier Bezier;
	public bool InBezier = true;
	public bool RotationActivated = false;

	public float rotationSpeed = 5;

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

	// Update each frame
	protected void Update()
	{
		// Reset a la posicion inicial si se ha completado el recorrido
		if (_espacioAcumulado >= Bezier.GetLenght())
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
				_espacioAcumulado = MoveConstantSpeed(timeInSection, InitialSpeed, InitialAcceleration);
			}
		}
	}

	private void ResetToInit()
	{
		// Devolver al inicio con su transformacion y rotacion iniciales
		transform.position = Bezier.GetBezierPointT(0);

		if (RotationActivated)
			transform.rotation = GetInitialRotation();

		// Variables dependientes de la posicion de la curva reseteadas
		_espacioAcumulado = 0;
		timeInSection = 0;
		lastSectionSpace = 0;
		lastSectionSpeed = InitialSpeed;
	}

	// Ecuacion del espacio = velocidad * tiempo
	private float MoveInBezier(float time, float initialSpeed, float initialSpace = 0, float acceleration = 0)
	{
		// s = v0 � t + (a � t^2) / 2
		float espacio = (initialSpeed * time + (acceleration * Mathf.Pow(time, 2)) / 2) + initialSpace;
		//print("Espacio Acumulado = " + espacio + " = " + initialSpeed + "�" + time + " + "
		//	+ "1/2 � " + acceleration + "�" + time + "^2" + " + " + initialSpace);

		// Espacio normalizado a t (parametro t en la curva con esa longitud con el punto inicial en un margen de error)
		decimal t = Bezier.GetT((decimal)espacio);

		// Mueve el objecto a la posicion de t en la curva
		transform.position = Bezier.GetBezierPointT(t);

		// Rotates it
		if (RotationActivated)
			RotateTowardsCurve(t, rotationSpeed);

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

	private Quaternion RotateTowardsCurve(decimal t, float rotateSpeed = 1, decimal precision = 0.01m)
	{
		decimal tAnterior;

		// Primera Direccion: Cogemos el primer segmento
		if (t <= 0)
		{
			return transform.rotation = GetInitialRotation();
		}
		// Ultima direccion: El ultimo segmento
		if (t >= 1)
		{
			tAnterior = 1 - precision;
			t = 1;
		}
		else
			tAnterior = t - precision;

		// Puntos referencia:
		Vector3 initPoint = Bezier.GetBezierPointT(tAnterior);
		Vector3 finalPoint = Bezier.GetBezierPointT(t);

		// Direccion de Inicio-Final normalizada
		Vector3 direction = (finalPoint - initPoint).normalized;
		var up = direction == Vector3.up ? Vector3.back : Vector3.up;

		print("Init: " + initPoint.ToString("F8"));
		print("Final: " + finalPoint.ToString("F8"));
		print("Direction: " + direction.ToString("F8"));
		print("t0: " + tAnterior.ToString("F8"));
		print("t1: " + t.ToString("F8"));

		// No es Smooth
		//GetComponent<Rigidbody>().MoveRotation(Quaternion.LookRotation(direction));

		// Slerp() = Smooth
		GetComponent<Rigidbody>().MoveRotation(
			Quaternion.Slerp(
				transform.rotation,
				Quaternion.LookRotation(direction, up),
				(Time.inFixedTimeStep ? Time.fixedDeltaTime : Time.deltaTime) * rotateSpeed
				)
			);
		return transform.rotation /*= Quaternion.Slerp(
			transform.rotation,
			Quaternion.LookRotation(direction),
			(Time.inFixedTimeStep ? Time.fixedDeltaTime : Time.deltaTime) * rotateSpeed
		)*/;
	}

	private Quaternion GetInitialRotation()
	{
		// Puntos referencia:
		var initPoint = Bezier.GetBezierPointT(0);
		var finalPoint = Bezier.GetBezierPointT(0.01m);

		// Direccion de Inicio-Final normalizada
		Vector3 direction = (finalPoint - initPoint).normalized;
		Vector3 up = Vector3.up;
		if (Math.Abs((direction - Vector3.up).magnitude) < 0.01f)
			up = Vector3.back;

		//transform.rotation = Quaternion.LookRotation(direction);
		return transform.rotation = Quaternion.LookRotation(direction, up);
	}
}

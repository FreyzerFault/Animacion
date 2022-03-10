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

	public float AnimationTime = 5; // in seconds

	// EASE IN + EASE OUT
	public bool easeInOutActivated = false;

	// Fraccion de la curva con Ease in Ease out [0,1]
	public float EaseInSection = 0;
	public float EaseOutSection = 0;

	private bool onEase = true;

	[SerializeField] private float t;
	
	[SerializeField] private float timeInBezier = 0;
	[SerializeField] private float acceleration;
	[SerializeField] private float lastSectionSpeed = 0;


	// Start is called before the first frame update
	protected void Start()
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

	// Update each frame
	protected void Update()
	{
		float deltaTime = Time.inFixedTimeStep ? Time.fixedDeltaTime : Time.deltaTime;

		// Tiempo que lleva en una de las secciones de la curva para hacer ease In / Out
		timeInBezier += deltaTime;

		// Reset a la posicion inicial si se ha completado el recorrido
		//if (timeInBezier >= AnimationTime)
		//	ResetToInit();

		if (InBezier)
		{
			if (easeInOutActivated)
				t = GetTeaseInOut(AnimationTime, EaseInSection, 1 - EaseOutSection, timeInBezier);

			else // Sin Ease In Ease Out
				t = GetTConstantSpeed(AnimationTime, timeInBezier);

			MoveInBezier(t);
		}
	}

	private void ResetToInit()
	{
		// Devolver al inicio con su transformacion y rotacion iniciales
		transform.position = Bezier.GetBezierPointT(0);

		if (RotationActivated)
			transform.rotation = RotateTowardsCurve(0);

		// Variables dependientes de la posicion de la curva reseteadas
		t = 0;
		timeInBezier = 0;
	}

	[SerializeField] private float d;

	private float GetTeaseInOut(float animationTime, float t1, float t2, float time)
	{
		// Velocidad intermedia a partir del tiempo de animacion acotado al tramo intermedio
		//float v0 = 1 / (animationTime * 0.9f);
		float v0 = 0.5f;

		d = 0;

		float timeNormalized = Mathf.InverseLerp(0, animationTime, time);

		// Ease IN
		if (timeNormalized < t1)
			d = v0 * timeNormalized * timeNormalized / 2 / t1;

		// Ease Middle
		if (timeNormalized >= t1 && timeNormalized <= t2)
			d = v0 * t1 / 2 + v0 * (timeNormalized - t1);

		// Ease OUT
		if (timeNormalized > t2)
			d = v0 * t1 / 2 + v0 * (t2 - t1) + (v0 - (v0 * (timeNormalized - t2) / (1 - t2)) / 2) * (timeNormalized - t2);

		return (float)Bezier.GetT(d * Bezier.GetLenght());
	}

	private float GetTConstantSpeed(float animationTime, float time, float t0 = 0, float t1 = 1)
	{
		// Tiempo de animacion en el tramo
		float easeAnimationTime = animationTime * (t1 - t0);
		float timeInSection = time - t0 * animationTime;

		// Punto inicial y final del tramo
		float s0 = Bezier.GetDist((decimal)t0);
		float s1 = Bezier.GetDist((decimal)t1);

		float speed = (s1 - s0) / easeAnimationTime;

		float espacio = (speed * timeInSection) + s0;

		// Vuelve al inicio
		if (espacio > Bezier.GetLenght())
			espacio -= Bezier.GetLenght();

		// Espacio normalizado a t (parametro t en la curva con esa longitud con el punto inicial en un margen de error)
		t = (float)Bezier.GetT(espacio);

		return t;
	}

	private float GetTinEase(float animationTime, float time, float t0, float t1,  float initialSpeed = 0)
	{
		// Tiempo de animacion en el ease
		float easeAnimationTime = animationTime * (t1 - t0);
		float timeInSection = time - t0 * animationTime;

		// Punto inicial y final del ease
		float s0 = Bezier.GetDist((decimal)t0);
		float s1 = Bezier.GetDist((decimal)t1);

		// Despejando la aceleracion de su ecuacion se puede calcular a partir del tiempo
		acceleration = (s1 - s0 - initialSpeed * easeAnimationTime) * 2 / easeAnimationTime/easeAnimationTime;

		float espacio = (initialSpeed * timeInSection + (acceleration * timeInSection*timeInSection) / 2) + s0;

		t = (float)Bezier.GetT(espacio);

		MoveToT(t);

		// Rotates it
		if (RotationActivated)
			RotateTowardsCurve(t);

		return t;
	}

	// Ecuacion del espacio = velocidad * tiempo
	private void MoveInBezier(float t)
	{
		// Mueve el objecto a la posicion de t en la curva
		MoveToT(t);

		// Rotates it
		if (RotationActivated)
			RotateTowardsCurve(t);
	}

	private Vector3 MoveToT(float t)
	{
		return transform.position = Bezier.GetBezierPointT((decimal)t);
	}

	private Quaternion RotateTowardsCurve(float t)
	{
		Vector3 curveVelocity = Bezier.GetVelocity(t).normalized;
		Vector3 up = Vector3.Cross(Vector3.Cross(Bezier.GetAcceleration(t).normalized, curveVelocity), curveVelocity);

		// Apunta en direccion de la Tangente de la Curva (Derivada)
		Quaternion tangentQuat = Quaternion.LookRotation(curveVelocity, up);

		if (GetComponent<Rigidbody>())
			GetComponent<Rigidbody>().MoveRotation(
				Quaternion.Slerp(
					transform.rotation,
					tangentQuat,
					(Time.inFixedTimeStep ? Time.fixedDeltaTime : Time.deltaTime) * Bezier.GetAcceleration(t).magnitude
				)
			);
		else
			transform.rotation = Quaternion.Slerp(
				transform.rotation,
				tangentQuat,
				(Time.inFixedTimeStep ? Time.fixedDeltaTime : Time.deltaTime) * Bezier.GetAcceleration(t).magnitude
			);

		return tangentQuat;
	}
}

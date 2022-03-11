using System;
using System.Numerics;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class BezierMovable : MonoBehaviour
{
	public Bezier Bezier;
	[Space]
	public bool InBezier = true;
	[Space]
	public bool RotationActivated = false;

	[Space]
	public float TotalAnimationTime = 5; // in seconds

	// EASE IN + EASE OUT
	[Header("Ease In / Out")]
	public bool easeInOutActivated = false;

	// Fraccion de la curva con Ease in Ease out [0,1]
	[Range(0, 1)] public float EaseInSection;
	[Range(0, 1)] public float EaseOutSection;

	
	[Header("Movement")]
	[SerializeField] private float t;
	[SerializeField][InspectorName("Distance")] private float d;
	[SerializeField] private float animationTime;
	[SerializeField] private float speed;
	[SerializeField] private float acceleration;


	// Start is called before the first frame update
	protected void Start()
	{
		if (Bezier)
			ResetToInit();

	}

	// Update each frame
	protected void Update()
	{
		float deltaTime = Time.inFixedTimeStep ? Time.fixedDeltaTime : Time.deltaTime;
		
		// Tiempo que lleva en una de las secciones de la curva para hacer ease In / Out
		animationTime += deltaTime;

		// Reset a la posicion inicial si se ha completado el recorrido
		if (animationTime >= TotalAnimationTime)
			ResetToInit();

		if (InBezier)
		{
			// Chekea que sean coherentes
			if (EaseInSection + EaseOutSection > 1)
			{
				if (EaseInSection > EaseOutSection)
					EaseOutSection = 1 - EaseInSection;
				else
					EaseInSection = 1 - EaseOutSection;
			}

			if (easeInOutActivated)
				t = GetTeaseInOut(TotalAnimationTime, EaseInSection, 1 - EaseOutSection, animationTime);

			else // Sin Ease In Ease Out
				t = GetTConstantSpeed(TotalAnimationTime, animationTime);

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
		animationTime = 0;
	}

	private float GetTeaseInOut(float animationTime, float t1, float t2, float time)
	{
		// Velocidad normalizada despejada de la ecuacion del ease out [0,1]
		//float v0 = 1 / (-t1/2 + 1 - (1-t2)/2);
		float v0 = 1 / (-t1/2 + 1 - (1-t2)/2);

		// Tiempo = [0,1] siendo 1 = segundos de animacion
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

		if (timeNormalized < t1)
		{
			acceleration = v0 / t1;
			speed = v0 * timeNormalized / t1;
		}
		if (timeNormalized >= t1 && timeNormalized <= t2)
		{
			acceleration = 0;
			speed = v0;
		}
		if (timeNormalized > t2)
		{
			acceleration = -v0 / (1 - t2);
			speed = v0 * (1 - (timeNormalized - t2) / (1 - t2));
		}

		// Interpolacion lineal de [0,1] a [0, distancia total de la curva]
		d *= Bezier.GetLenght();

		return (float)Bezier.GetT(d);
	}

	private float GetTConstantSpeed(float animationTime, float time, float t0 = 0, float t1 = 1)
	{
		// Si el tramo es la curva entera se calcula directamente como (dTotal / tTotal * time)
		if (t0 == 0 && t1 == 1)
			return (float)Bezier.GetT((Bezier.GetLenght() / animationTime * time));

		// Tiempo de animacion en el tramo
		float easeAnimationTime = animationTime * (t1 - t0);
		float timeInSection = time - t0 * animationTime;

		// Punto inicial y final del tramo
		float s0 = Bezier.GetDist((decimal)t0);
		float s1 = Bezier.GetDist((decimal)t1);

		speed = (s1 - s0) / easeAnimationTime;
		acceleration = 0;

		float espacio = (speed * timeInSection) + s0;

		// Vuelve al inicio
		if (espacio > Bezier.GetLenght())
			espacio -= Bezier.GetLenght();

		// Espacio normalizado a t (parametro t en la curva con esa longitud con el punto inicial en un margen de error)
		return (float)Bezier.GetT(espacio);
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
		// Los cuerpos con masa dan problemas con la rotacion
		Rigidbody rb = GetComponent<Rigidbody>();
		if (rb)
			rb.mass = 0;

		Vector3 curveVelocity = Bezier.GetVelocity(t).normalized;
		Vector3 up = Vector3.Cross(Vector3.Cross(Bezier.GetAcceleration(t).normalized, curveVelocity), curveVelocity);

		// Apunta en direccion de la Tangente de la Curva (Derivada)
		Quaternion tangentQuat = Quaternion.LookRotation(curveVelocity, up);

		if (rb)
			rb.MoveRotation(
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

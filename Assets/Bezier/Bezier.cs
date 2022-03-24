using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


public static class ExtensionMethods
{
	// Extension de Float para el REMAP con Interpolacion Lineal
	// Pasa un valor de [in0, in1] a [out0, out1]
	public static float Remap(this float value, float in0, float in1, float out0, float out1)
	{
		return (value - in0) / (in1 - in0) * (out1 - out0) + out0;
	}
	
	public static decimal Remap(this decimal value, decimal in0, decimal in1, decimal out0, decimal out1)
	{
		return (value - in0) / (in1 - in0) * (out1 - out0) + out0;
	}
}
[ExecuteAlways]
public class Bezier : MonoBehaviour
{
	public Vector3[] cpPositions;
	public decimal BezierResolution = 0.01m;

	[Space]
	public GameObject ObjectMoving;
	private bool moving = false;

	[Space]
	public bool RenderLine = true;
	public bool RenderControlPoints = true;

	private LineRenderer lineRenderer;



	[Serializable]
	public class MovementSettings
	{
		[Space]
		public bool RotationActivated = false;
		public bool upRollRotationActivated = false;

		[Space]
		public float TotalAnimationTime = 5; // in seconds

		[Space]
		public AnimationCurve spaceCurve;

		// EASE IN + EASE OUT
		[Header("Ease In / Out")]
		// Fraccion de la curva con Ease in Ease out [0,1]
		[Range(0, 1)] public float EaseInSection;
		[Range(0, 1)] public float EaseOutSection;

		[Space]
		[Header("Parameters:")]
		public float AnimationTime;
		public float TiempoNormalizado;
		public float T;
		public float Distance;
		[Range(0, 100)] public float Speed;
		public float Acceleration;

		public AnimationCurve tCurve;
	}

	[Space]
	public MovementSettings Move;

	private float deltaTime;


	// Look Up Table con los puntos de la curva (Vec3) segun la t
	private readonly Dictionary<decimal, Vector3> LUTpuntosT = new Dictionary<decimal, Vector3>();

	// Look Up Table de Distancia Acumulada para cada t de la curva
	// t => distancia
	private readonly Dictionary<decimal, float> LUTdistanceByT = new Dictionary<decimal, float>();
	// distancia => t
	private readonly Dictionary<float, decimal> LUTtByDistance = new Dictionary<float, decimal>();

	// Start is called before the first frame update

	void Awake()
	{
		lineRenderer = GetComponent<LineRenderer>();
		
		// Actualiza la curva, comprobando los puntos de control, los puntos y asignandoselos a el LineRenderer
		UpdateControlPoints();

		UpdateLineRenderer();
	}


	void Update()
	{
		// line Renderer activado & esta en PlayMode
		lineRenderer.enabled = RenderLine && Application.isPlaying;

		// MOVIMIENTO POR LA CURVA:
		if (moving)
		{
			deltaTime = Time.inFixedTimeStep ? Time.fixedDeltaTime : Time.deltaTime;

			if (AnimationFinished())
				ResetToInit();

			// Tiempo que lleva en una de las secciones de la curva para hacer ease In / Out
			Move.AnimationTime += deltaTime;

			// Chekea que sean coherentes
			if (Move.EaseInSection + Move.EaseOutSection > 1)
			{
				if (Move.EaseInSection > Move.EaseOutSection)
					Move.EaseOutSection = 1 - Move.EaseInSection;
				else
					Move.EaseInSection = 1 - Move.EaseOutSection;
			}

			if (easeInOutActivated())
				Move.T = GetTeaseInOut(Move.TotalAnimationTime, Move.EaseInSection, 1 - Move.EaseOutSection, Move.AnimationTime);
			else
			{	// Sin Ease In Ease Out

				// Segun la Curva de Velocidad
				if (Move.spaceCurve != null)
					Move.T = GetTinCurve(Move.spaceCurve, Move.TotalAnimationTime, Move.AnimationTime);
				// Velocidad Constante
				else
					Move.T = GetTConstantSpeed(Move.TotalAnimationTime, Move.AnimationTime);
			}

			MoveInBezier(Move.T);
		}
		
	}




	public ControlPoint getControlPoint(int index)
	{
		ControlPoint[] cps = GetComponentsInChildren<ControlPoint>();
		if (cps[index])
			return cps[index];

		return null;
	}


	public Vector3 GetBezierPointT(decimal t)
	{
		if (t <= 0)
			return cpPositions[0];
		if (t >= 1)
			return cpPositions[cpPositions.Length - 1];

		if (LUTpuntosT.ContainsKey(t))
			return LUTpuntosT[t];

		// Grado de la curva de Bezier
		int n = cpPositions.Length - 1;

		// Hay que calcular la Sumatoria de la "Influencia" de cada Punto de Control en t
		Vector3 p = Vector3.zero;

		for (int i = 0; i < cpPositions.Length; ++i)
		{
			float comb = Combinatoria(n, i);

			// GRADO n ==> p(t) = SUM[i=0,n] ( Pi * comb(n,i) * (1-t)^(n-i) * t^i )
			p += (cpPositions[i] * (comb * Mathf.Pow((float)(1 - t), n - i) * Mathf.Pow((float)t, i)));
		}
		return p;
	}

	public Vector3 GetVelocity(float t)
	{
		Vector3 derivada = Vector3.zero;

		derivada += cpPositions[0] * (-3 *t*t +6 *t - 3);
		derivada += cpPositions[1] * ( 9 *t*t -12*t + 3);
		derivada += cpPositions[2] * (-9 *t*t +6 *t    );
		derivada += cpPositions[3] * ( 3 *t*t          );
		
		return derivada;
	}
	public Vector3 GetAcceleration(float t)
	{
		Vector3 derivada = Vector3.zero;

		derivada += cpPositions[0] * (- 6 *t + 6);
		derivada += cpPositions[1] * ( 18 *t - 12);
		derivada += cpPositions[2] * (-18 *t + 6);
		derivada += cpPositions[3] * (  6 *t    );
		
		return derivada;
	}

	// Saca el espacio que hay en un segmento de la curva con una precision (derivada) de delta
	private float GetLengthIncrementoT(decimal t, decimal delta)
	{
		Vector3 p0 = LUTpuntosT.ContainsKey(t) ? LUTpuntosT[t] : GetBezierPointT(t);
		Vector3 p1 = LUTpuntosT.ContainsKey(t + delta) ? LUTpuntosT[t + delta] : GetBezierPointT(t + delta);

		return (p1 - p0).magnitude;
	}

	// Longitud de la Curva
	public float GetLenght()
	{
		float length = LUTdistanceByT[1];
		return length;
	}


	// Distancia acumulada en la curva para un punto t en ella
	public float GetDist(decimal t)
	{
		if (t <= 0)
			return 0;

		if (t >= 1)
			return GetLenght();
		
		if (LUTdistanceByT.ContainsKey(t))
			return LUTdistanceByT[t];

		// Si no esta registrado en la LUT buscamos el t mas cercano
		for (decimal t0 = 0; t0 <= 1; t0 += BezierResolution)
		{
			if (t >= t0)
			{
				decimal t1 = t0 + BezierResolution;
				// Remapeado del rango [t0,t1] a [d0,d1] por Interpolacion Lineal
				return (float)t.Remap(t0, t1, (decimal)LUTdistanceByT[t0], (decimal)LUTdistanceByT[t1]);
			}
		}

		// Si no interpolamos con distancias continuas a las malas
		return (float)t * GetLenght();
	}

	// Devuelve el Parámetro t para un espacio recorrido en la curva
	public decimal GetT(float distance)
	{
		// Casos Triviales:
		// Si se sale de la curva extrapolar
		if (distance <= 0 || distance >= GetLenght())
		{
			return (decimal)(distance / GetLenght());
		}

		// Last distance
		float s0 = 0;
		// Buscar por toda la tabla de distancias hasta que encaje en un hueco y interpolar la t del segmento
		foreach (float s1 in LUTtByDistance.Keys)
		{
			if (distance <= s1 && distance >= s0)
			{
				decimal t0 = LUTtByDistance[s0];
				decimal t1 = LUTtByDistance[s1];
				
				// INTERPOLACION entre los dos puntos t (% deltaS => % deltaT)
				// t = (fraccion de distancia que sobrepasa) * (segmento t) + t0
				return ((decimal)distance).Remap((decimal)s0, (decimal)s1, t0, t1);
			}
			s0 = s1;
		}

		// Interpolamos si no encuentra en la tabla un hueco
		return (decimal)Mathf.InverseLerp(0, GetLenght(), distance);
	}

	public void UpdateLineRenderer()
	{
		// Draw LineRenderer
		lineRenderer.positionCount = LUTpuntosT.Count;
		lineRenderer.SetPositions(LUTpuntosT.Values.ToArray());
	}

	// Actualiza los puntos de control cuando se muevan ingame
	public void UpdateControlPoints()
	{
		ControlPoint[] controlPoints = GetComponentsInChildren<ControlPoint>();
		cpPositions = new Vector3[controlPoints.Length];
		for (int i = 0; i < controlPoints.Length; i++)
		{
			cpPositions[i] = controlPoints[i].transform.position;
		}

		UpdateBezierPoints();
	}

	private void UpdateBezierPoints()
	{
		// Vaciamos las LUT
		LUTpuntosT.Clear();
		LUTdistanceByT.Clear();
		LUTtByDistance.Clear();

		// El parametro de la linea t va incrementado segun la resolucion de la curva
		decimal t = 0;

		// Inicio
		LUTdistanceByT[0] = 0;
		LUTtByDistance[0] = 0;
		LUTpuntosT[0] = cpPositions[0];

		while (t <= 1)
		{
			decimal tAnterior = t;
			t += BezierResolution;

			Vector3 point = GetBezierPointT(t);
			
			LUTpuntosT.Add(t, point);

			// distancia(t1) = ( Distancia entre p(t0) y p(t1) ) + distancia(t0)
			float distancia = (point - LUTpuntosT[tAnterior]).magnitude + LUTdistanceByT[tAnterior];
			LUTdistanceByT.Add(t, distancia);
			if (!LUTtByDistance.ContainsKey(distancia))
				LUTtByDistance.Add(distancia, t);
		}
	}

	// COMBINATORIA (usa memoization)
	// Cache con una lista de resultados porque se van a repetir mucho
	private readonly Dictionary<Tuple<float, float>, float> _combCache = new Dictionary<Tuple<float, float>, float>();

	private float Combinatoria(float n, float k)
	{
		Tuple<float, float> key = new Tuple<float, float>(n, k);
		if (_combCache.ContainsKey(key))
			return _combCache[key];

		if (k.Equals(0))
			return 1;

		if (k.Equals(1))
			return n;

		// Combinatoria = n(n-1)(n-2)...(n - k+1) / k!
		float comb = n;
		for (int i = 1; i < k; ++i)
			comb *= n - i;

		// Factorial de k = 1*2*3*4*...*k
		float factorial = 1;
		for (int i = 1; i <= k; ++i)
			factorial *= i;

		_combCache.Add(key, comb / factorial);
		return _combCache[key];
	}



	// =========================== Animaciones ===========================

	private float GetTeaseInOut(float animationTime, float t1, float t2, float time)
	{
		// Velocidad normalizada despejada de la ecuacion del ease out [0,1]
		float v0 = 1 / (-t1 / 2 + 1 - (1 - t2) / 2);

		// Tiempo = [0,1] siendo 1 = segundos de animacion
		float timeNormalized = Mathf.InverseLerp(0, animationTime, time);

		float d = 0;

		// Ease IN
		if (timeNormalized < t1)
			d = v0 * timeNormalized * timeNormalized / 2 / t1;

		// Ease Middle
		if (timeNormalized >= t1 && timeNormalized <= t2)
			d = v0 * t1 / 2 + v0 * (timeNormalized - t1);

		// Ease OUT
		if (timeNormalized > t2)
			d = v0 * t1 / 2 + v0 * (t2 - t1) + (v0 - (v0 * (timeNormalized - t2) / (1 - t2)) / 2) * (timeNormalized - t2);

		Move.TiempoNormalizado = timeNormalized;
		Move.Distance = d;
		if (timeNormalized < t1)
		{
			Move.Acceleration = v0 / t1;
			Move.Speed = v0 * timeNormalized / t1;
		}
		if (timeNormalized >= t1 && Move.TiempoNormalizado <= t2)
		{
			Move.Acceleration = 0;
			Move.Speed = v0;
		}
		if (timeNormalized > t2)
		{
			Move.Acceleration = -v0 / (1 - t2);
			Move.Speed = v0 * (1 - (timeNormalized - t2) / (1 - t2));
		}

		// Interpolacion lineal de [0,1] a [0, distancia total de la curva]
		d *= GetLenght();

		return (float)GetT(d);
	}

	private float GetTConstantSpeed(float animationTime, float time, float t0 = 0, float t1 = 1)
	{
		// Si el tramo es la curva entera se calcula directamente como (dTotal / tTotal * time)
		if (t0 <= 0 && t1 >= 1)
			return (float)GetT((GetLenght() / animationTime * time));

		// Tiempo de animacion en el tramo
		float easeAnimationTime = animationTime * (t1 - t0);
		float timeInSection = time - t0 * animationTime;

		// Punto inicial y final del tramo
		float s0 = GetDist((decimal)t0);
		float s1 = GetDist((decimal)t1);

		float speed = (s1 - s0) / easeAnimationTime;

		float espacio = (Move.Speed * timeInSection) + s0;

		// Vuelve al inicio
		if (espacio > GetLenght())
			espacio -= GetLenght();


		Move.Speed = speed;
		Move.Acceleration = 0;
		Move.Distance = espacio;
		Move.AnimationTime = timeInSection;

		// Espacio normalizado a t (parametro t en la curva con esa longitud con el punto inicial en un margen de error)
		return (float)GetT(espacio);
	}

	private float GetTinCurve(AnimationCurve spaceCurve, float animationTime, float time, float t0 = 0, float t1 = 1)
	{
		float espacio = 0;
		// Calculamos la t segun el Espacio en el Grafico de la Curva de Movimiento
		if (t0 <= 0 && t1 >= 1)
			espacio = Mathf.Lerp(0, GetLenght(),
				spaceCurve.Evaluate(
					Mathf.InverseLerp(0, animationTime, time)
				)
			);
		else
		{
			// Tiempo de animacion en el tramo
			float totalAnimationTime = animationTime * (t1 - t0);
			float timeInSection = time - t0 * animationTime;

			// Punto inicial y final del tramo
			float s0 = GetDist((decimal)t0);
			float s1 = GetDist((decimal)t1);

			// Espacio de la Curva [0,1]
			espacio = spaceCurve.Evaluate(Mathf.InverseLerp(0, totalAnimationTime, timeInSection)) + s0;

			// Interpolado
			espacio = Mathf.Lerp(s0, s1, espacio);
		}

		// Vuelve al inicio
		if (espacio > GetLenght())
			espacio -= GetLenght();

		float t = (float)GetT(espacio);

		Move.tCurve.AddKey(time, t);

		float deltaSpace = espacio - Move.Distance;
		float deltaSpeed = deltaSpace / deltaTime - Move.Speed;
		Move.Distance = espacio;
		Move.Speed = deltaSpace / deltaTime;
		Move.Acceleration = deltaSpeed / deltaTime;

		// Espacio normalizado a t (parametro t en la curva con esa longitud con el punto inicial en un margen de error)
		return t;
	}


	public void SetObjectMoving(GameObject obj) { ObjectMoving = obj; }

	// Ultimo valor de la Gravedad del Objeto
	private bool gravityWasActive;
	// Activa el movimiento si hay un objeto asociado y le quita la gravedad
	public bool StartMoving()
	{
		if (ObjectMoving)
		{
			ResetToInit();

			// Desactivamos la gravedad para que no haga cosas raras
			Rigidbody rb = ObjectMoving.GetComponent<Rigidbody>();
			if (rb)
			{
				gravityWasActive = rb.useGravity;
				if (rb && gravityWasActive)
					rb.useGravity = false;
			}
			moving = true;
			return true;
		}

		return false;
	}

	// Cuando acaba la animacion desactiva el movimiento y devuelve los parametros de gravedad al objeto
	public void StopMoving()
	{
		// Volvemos a activar la gravedad si estaba activa
		Rigidbody rb = ObjectMoving.GetComponent<Rigidbody>();
		rb.useGravity = gravityWasActive;

		ObjectMoving = null;
		moving = false;
	}

	// Ha acabado el tiempo de Animacion
	public bool AnimationFinished() { return Move.AnimationTime >= Move.TotalAnimationTime; }


	private void ResetToInit()
	{
		if (Move.RotationActivated)
			ObjectMoving.transform.rotation = RotateTowardsCurve(0);

		// Variables dependientes de la posicion de la curva reseteadas
		Move.AnimationTime = 0;
	}

	private void MoveInBezier(float t)
	{
		// Mueve el objecto a la posicion de t en la curva
		MoveToT(t);

		// Rotates it
		if (Move.RotationActivated)
			RotateTowardsCurve(t);
	}

	private Vector3 MoveToT(float t)
	{
		return ObjectMoving.transform.position = GetBezierPointT((decimal)t);
	}

	private Quaternion RotateTowardsCurve(float t)
	{
		// Los cuerpos con masa dan problemas con la rotacion
		Rigidbody rb = ObjectMoving.GetComponent<Rigidbody>();

		Vector3 curveVelocity = GetVelocity(t).normalized;

		Vector3 up = Vector3.up;
		if (Move.upRollRotationActivated)
			up = Vector3.Cross(Vector3.Cross(GetAcceleration(t).normalized, curveVelocity), curveVelocity);

		// Apunta en direccion de la Tangente de la Curva (Derivada)
		Quaternion tangentQuat = Quaternion.LookRotation(curveVelocity, up);

		if (rb)
			rb.MoveRotation(
				Quaternion.Slerp(
					ObjectMoving.transform.rotation,
					tangentQuat,
					(Time.inFixedTimeStep ? Time.fixedDeltaTime : Time.deltaTime) * GetAcceleration(t).magnitude
				)
			);
		else
			ObjectMoving.transform.rotation = Quaternion.Slerp(
				ObjectMoving.transform.rotation,
				tangentQuat,
				(Time.inFixedTimeStep ? Time.fixedDeltaTime : Time.deltaTime) * GetAcceleration(t).magnitude
			);

		return tangentQuat;
	}

	public bool easeInOutActivated()
	{
		return Move.EaseInSection > 0 || Move.EaseOutSection > 0;
	}

	// =========================== GIZMO ===========================
	private void OnDrawGizmos()
	{
		if (LUTpuntosT.Count == 0)
			UpdateControlPoints();

		// Curve
		Gizmos.color = Color.blue;
		decimal t = 0;
		decimal resolution = 0.01m;
		if (LUTpuntosT.Count != 0)
			while (t < 1)
			{
				Gizmos.DrawLine(GetBezierPointT(t), GetBezierPointT(t += resolution));
			}

		// Ray Control Points
		Gizmos.color = Color.white;

		float tangent1 = (cpPositions[1] - cpPositions[0]).magnitude;
		float tangent2 = (cpPositions[3] - cpPositions[2]).magnitude;

		float redT = Mathf.InverseLerp(0, GetLenght(), tangent1);
		Gizmos.color = new Color(redT, 1-redT, 0);
		
		Gizmos.DrawLine(cpPositions[0], cpPositions[1]);

		redT = Mathf.InverseLerp(0, GetLenght(), tangent2);
		Gizmos.color = new Color(redT, 1-redT, 0);
		Gizmos.DrawLine(cpPositions[2], cpPositions[3]);
	}
}
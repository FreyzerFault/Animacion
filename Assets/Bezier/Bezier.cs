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
}
[ExecuteAlways]
public class Bezier : MonoBehaviour
{
	public LineRenderer lineRenderer;
	public bool renderLine = true;
	public bool renderControlPoints = true;
	
	public Vector3[] cpPositions;

	public decimal BezierResolution = 0.01m;

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

	// Update is called once per frame
	void Update()
	{
		// line Renderer activado & esta en PlayMode
		lineRenderer.enabled = renderLine && Application.isPlaying;
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
		return LUTdistanceByT[1];
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
				return ((float)t).Remap((float)t0, (float)t1, LUTdistanceByT[t0], LUTdistanceByT[t1]);
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
			return (decimal)(distance / GetLenght());

		// Last distance
		float s0 = 0;
		// Buscar por toda la tabla de distancias hasta que encaje en un hueco y interpolar la t del segmento
		foreach (float s1 in LUTtByDistance.Keys)
		{
			if (distance <= s1 && distance >= s0)
			{
				float t0 = (float)LUTtByDistance[s0];
				float t1 = (float)LUTtByDistance[s1];
				
				// INTERPOLACION entre los dos puntos t (% deltaS => % deltaT)
				// t = (fraccion de distancia que sobrepasa) * (segmento t) + t0
				return (decimal)distance.Remap(s0, s1, t0, t1);
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
		LUTpuntosT.Clear();

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

	// =========================== GIZMO ===========================
	private void OnDrawGizmos()
	{
		if (LUTpuntosT.Count == 0)
			UpdateControlPoints();

		// Curve
		Gizmos.color = Color.yellow;
		decimal t = 0;
		decimal resolution = 0.01m;
		if (LUTpuntosT.Count != 0)
			while (t < 1)
			{
				Gizmos.DrawLine(GetBezierPointT(t), GetBezierPointT(t += resolution));
			}

		// Ray Control Points
		Gizmos.color = Color.white;
		Gizmos.DrawLine(cpPositions[0], cpPositions[1]);
		Gizmos.DrawLine(cpPositions[2], cpPositions[3]);
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

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
	private readonly Dictionary<decimal, decimal> LUTdistanceByT = new Dictionary<decimal, decimal>();
	// distancia => t
	private readonly Dictionary<decimal, decimal> LUTtByDistance = new Dictionary<decimal, decimal>();

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

	public Vector3 GetVelocity(decimal t)
	{
		Vector3 derivada = Vector3.zero;

		derivada += cpPositions[0] * (float)(-3 *t*t +6 *t - 3);
		derivada += cpPositions[1] * (float)( 9 *t*t -12*t + 3);
		derivada += cpPositions[2] * (float)(-9 *t*t +6 *t    );
		derivada += cpPositions[3] * (float)( 3 *t*t          );
		
		return derivada;
	}
	public Vector3 GetAcceleration(decimal t)
	{
		Vector3 derivada = Vector3.zero;

		derivada += cpPositions[0] * (float)(- 6 *t + 6);
		derivada += cpPositions[1] * (float)( 18 *t - 12);
		derivada += cpPositions[2] * (float)(-18 *t + 6);
		derivada += cpPositions[3] * (float)(  6 *t    );
		
		return derivada;
	}

	// Saca el espacio que hay en un segmento de la curva con una precision (derivada) de delta
	public decimal GetLengthIncrementoT(decimal t, decimal delta)
	{
		Vector3 p0 = LUTpuntosT.ContainsKey(t) ? LUTpuntosT[t] : GetBezierPointT(t);
		Vector3 p1 = LUTpuntosT.ContainsKey(t + delta) ? LUTpuntosT[t + delta] : GetBezierPointT(t + delta);

		return (decimal)(p1 - p0).magnitude;
	}


	// Distancia acumulada en la curva para un punto t en ella
	private decimal GetLengthAcumuladaT(decimal t)
	{
		if (t <= 0)
			return 0;

		if (t >= 1)
			return (decimal)GetLenght();
		
		if (LUTdistanceByT.ContainsKey(t))
			return LUTdistanceByT[t];

		// Si no esta registrado en la LUT buscamos el t mas cercano
		for (decimal t0 = 0; t0 <= 1; t0 += BezierResolution)
		{
			if (t >= t0)
			{
				// Distancia del t encontrado + el intervalo con t calculado que falta
				return LUTdistanceByT[t0] + GetLengthIncrementoT(t0, t - t0);
			}
		}

		// Si no interpolamos con distancias continuas a las malas
		return t * (decimal)GetLenght();
	}

	// Longitud de la Curva
	public float GetLenght()
	{
		return (float)LUTdistanceByT[1];
	}

	// Devuelve el Parámetro t para un espacio recorrido en la curva
	public decimal GetT(decimal distance)
	{
		// Casos Triviales:
		// Si se sale de la curva extrapolar
		if (distance <= 0 || distance >= (decimal)GetLenght())
			return distance / (decimal)GetLenght();

		// Last distance
		decimal s0 = 0;
		// Buscar por toda la tabla de distancias hasta que encaje en un hueco y interpolar la t del segmento
		foreach (decimal s in LUTtByDistance.Keys)
		{
			if (distance <= s && distance >= s0)
			{
				decimal t0 = LUTtByDistance[s0];
				decimal t1 = LUTtByDistance[s];
				
				// INTERPOLACION entre los dos puntos t (% deltaS => % deltaT)
				// t = (fraccion de distancia que sobrepasa) * (segmento t) + t0
				return (distance - s0) / (s - s0) * (t1 - t0) + t0;
			}
			s0 = s;
		}

		// Interpolamos si no encuentra en la tabla un hueco
		return distance / (decimal)GetLenght();
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
			decimal distancia = (decimal)(point - LUTpuntosT[tAnterior]).magnitude + LUTdistanceByT[tAnterior];
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
				if (LUTpuntosT.ContainsKey(t) && LUTpuntosT.ContainsKey(t + resolution))
					Gizmos.DrawLine(LUTpuntosT[t], LUTpuntosT[t += resolution]);
			}

		// Ray Control Points
		Gizmos.color = Color.white;
		Gizmos.DrawLine(cpPositions[0], cpPositions[1]);
		Gizmos.DrawLine(cpPositions[2], cpPositions[3]);
	}
}
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class Bezier : MonoBehaviour
{
	public LineRenderer lineRenderer;

	public GameObject ControlPoint0;
	public GameObject ControlPoint1;
	public GameObject ControlPoint2;
	public GameObject ControlPoint3;

	public List<Vector3> controlPoints;
	public List<Vector3> points = new List<Vector3>();

	public decimal BezierResolution = 0.01m;

	private float length = 0;

	// Start is called before the first frame update
	void Start()
	{
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.alignment = LineAlignment.View;
		lineRenderer.startColor = Color.yellow;
		lineRenderer.endColor = Color.yellow;
		lineRenderer.loop = false;
		lineRenderer.numCapVertices = 10;
		lineRenderer.numCornerVertices = 0;
		lineRenderer.useWorldSpace = true;

		// Crea los Puntos de Control
		UpdateControlPoints();

		// Crea los puntos por los que pasa la Curva de Bezier
		UpdateBezierPoints();

		// Draw LineRenderer
		lineRenderer.positionCount = points.Count;
		lineRenderer.SetPositions(points.ToArray());
	}

	// Update is called once per frame
	void Update()
	{
	}

	// Almacen/Tabla con los puntos de la curva (Vec3) segun la t porque se llama a esta funcion MUCHO
	private readonly Dictionary<decimal, Vector3> tablaPuntosT = new Dictionary<decimal, Vector3>();

	public Vector3 GetBezierPointT(decimal t)
	{
		if (t <= 0)
			return controlPoints[0];
		if (t >= 1)
			return controlPoints[controlPoints.Count - 1];

		if (tablaPuntosT.ContainsKey(t))
			return tablaPuntosT[t];

		// Grado de la curva de Bezier
		int n = controlPoints.Count - 1;

		// Hay que calcular la Sumatoria de la "Influencia" de cada Punto de Control en t
		Vector3 p = Vector3.zero;

		for (int i = 0; i < controlPoints.Count; ++i)
		{
			float comb = Combinatoria(n, i);

			// GRADO n ==> p(t) = SUM[i=0,n] ( Pi * comb(n,i) * (1-t)^(n-i) * t^i )
			p += (controlPoints[i] * (comb * Mathf.Pow((float)(1 - t), n - i) * Mathf.Pow((float)t, i)));
		}

		tablaPuntosT[t] = p;
		return p;
	}

	// Saca el espacio que hay en un segmento de la curva con una precision (derivada) de delta
	public decimal GetLengthIncrementoT(decimal t, decimal delta)
	{
		Vector3 p0 = GetBezierPointT(t);
		Vector3 p1 = GetBezierPointT(t + delta);

		return (decimal)(p1 - p0).magnitude;
	}

	// Tabla de Espacio Acumulado para cada t de la curva
	private readonly Dictionary<decimal, decimal> tablaEspacioAcumulado = new Dictionary<decimal, decimal>();

	// longitud acumulada en la curva para un punto t en ella
	// RECURSIVIDAD + MEMOIZATION
	public decimal GetLengthAcumuladaT(decimal t, decimal intervaloT = 0.001m)
	{
		if (t <= 0)
			return 0;

		if (t >= 1)
			return (decimal)GetLenght();

		// MEMOIZATION
		if (tablaEspacioAcumulado.ContainsKey(t))
			return tablaEspacioAcumulado[t];

		// t Anterior:
		decimal tAnterior = t - intervaloT;
		// Acumulamos el espacio acumulado del t anterior al t actual
		// Con RECURSIVIDAD
		tablaEspacioAcumulado.Add(t, GetLengthAcumuladaT(tAnterior) + GetLengthIncrementoT(t, intervaloT));

		return tablaEspacioAcumulado[t];
	}

	// Longitud de la Curva
	public float GetLenght(decimal precision = 0.001m)
	{
		// Si ya esta precalculada no se calcula
		return this.length > 0 ? this.length : UpdateLenght(precision);
	}

	// Recalcula la Longitud de la Curva
	private float UpdateLenght(decimal precision = 0.001m)
	{
		// Recorre la curva en intervalos de [precision]
		decimal s = 0;
		for (decimal i = 0; i <= 1; i += precision)
		{
			// Acumula derivadas
			s += GetLengthIncrementoT(i, precision);
		}
		return length = (float)s; // derivadas acumuladas
	}

	private int maxIteraciones = 50;

	// Devuelve el Parámetro t para un espacio recorrido en la curva
	public decimal GetT(decimal s, int iteraciones = 0, decimal espacioAcumulado = 0, decimal t = 0, decimal intervaloT = 0.1m)
	{
		// un margen de error tal que pueda subdividirse la curva en 1000 cachitos
		decimal margenError = (decimal)GetLenght() / 1000;
		// Casos Triviales:
		// Si s es 0 es el primer punto de la curva
		if (s <= 0)
			return 0;
		if (s >= (decimal)GetLenght())
			return 1;

		// Si la s es cercana a la longitud total de la curva (con un margen de error) devolvemos el final de la curva
		if (Math.Abs(s - (decimal)GetLenght()) < margenError)
			return 1;

		// Si se ha acercado a s lo suficiente obtenemos ese t
		if (Math.Abs(espacioAcumulado - s) < margenError)
		{
			//print("Termina en " + iteraciones + " iteraciones" + "\nt: " + t + "\n"
			//      + "Recorrido: " + espacioAcumulado + "\n" + "Objetivo: " + s
			//      + "\nIntervaloT: " + intervaloT);
			return t;
		}
		
		//print("Intervalo: " + intervaloT + "\nT: " + t
		//	+ "\nRecorrido: " + espacioAcumulado + "\nObjetivo: " + s);

		// Si es negativa la diferencia
		// Cogemos un intervalo mas pequeño entre el t anterior y este t
		// Ej: 0.2 sale negativo, volver a 0.1, intervalo / 10 = 0.01, seguimos 0.11, 0.12, 0.13...
		if (s - espacioAcumulado < 0)
		{
			t -= intervaloT;
			intervaloT /= 10;
		}

		t += intervaloT; // Aumentamos al siguiente intervalo
		
		iteraciones++;
		if (iteraciones >= maxIteraciones)
		{
			print("!!!! Termina en " + iteraciones + " iteraciones !!!!!\n" + "t: " + t + "\n" 
			      + "Recorrido: " + espacioAcumulado + "\n" + "Objetivo: " + s
			      + "\nIntervaloT: " + intervaloT);
			return t;
		}

		return GetT(s, iteraciones, GetLengthAcumuladaT(t, 0.001m), t, intervaloT);

		/*
		for (t = 0; t < 1; t += intervaloT)
		{
			// Espacio de cada intervalo chikito
			espacioAcumulado += GetLengthIncrementoT(t, intervaloT);

			// Continua aumentando hasta que supere el espacio que necesita
			if (espacioAcumulado >= s)
				break;
		}

		// Caso trivial: s esta al final de la curva
		if (t >= 1) return t;

		// Si esta en el margen de error, es ese el punto
		if (Math.Abs(espacioAcumulado - s) < margenError)
			return t;

		// Por si acaso dejamos un maximo de intentos a que acote
		int maxIteraciones = 20;
		int iteraciones = 0;

		// Acotamos arriba y abajo hasta que no se supere el margen de error
		while (Math.Abs(espacioAcumulado - s) > margenError && iteraciones < maxIteraciones)
		{
			// Acoto el intervalo a la mitad
			intervaloT /= 2;
			if (espacioAcumulado - s > margenError) // Disminuyo la mitad si es positivo
			{
				t -= intervaloT;
				espacioAcumulado -= GetLengthIncrementoT(t, intervaloT);
			}
			else // Aumento la mitad
			{
				t += intervaloT;
				espacioAcumulado += GetLengthIncrementoT(t - intervaloT, intervaloT);
			}

			iteraciones++;
		}
		return t;
		*/
	}

	// Actualiza los puntos de control cuando se muevan ingame
	public void UpdateControlPoints()
	{
		controlPoints = new List<Vector3>()
		{
			ControlPoint0.transform.position,
			ControlPoint1.transform.position,
			ControlPoint2.transform.position,
			ControlPoint3.transform.position
		};

		UpdateBezierPoints();
	}

	public void UpdateBezierPoints()
	{
		points.Clear();

		Vector3 inicio = controlPoints[0];
		Vector3 final = controlPoints[controlPoints.Count - 1];
		
		points.Add(inicio);

		// El parametro de la linea t va incrementado segun la resolucion de la curva
		decimal t = BezierResolution;
		
		while (t <= 1)
		{
			points.Add(GetBezierPointT(t));

			t += BezierResolution;
		}

		points.Add(final);

		// Actualizar la longitud
		UpdateLenght();
	}

	// COMBINATORIA (usa memoization)
	// Cache con una lista de resultados porque se van a repetir mucho
	private readonly Dictionary<Tuple<float, float>, float> _combCache = new Dictionary<Tuple<float, float>, float>();

	public float Combinatoria(float n, float k)
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
}


/* UI con Handles que dibuja un Gizmo de la linea al seleccionar el Objeto con el Script

// A tiny custom editor for ExampleScript component
[CustomEditor(typeof(Bezier))]
public class ShowCurveInEditor : Editor
{
	// Custom in-scene UI for when ExampleScript
	// component is selected.
	public void OnSceneGUI()
	{
		Bezier bezier = target as Bezier;
		Transform tr = bezier.transform;
		Vector3 pos = tr.position;

		// CONTROL POINTS
		Vector3 p0 = bezier.ControlPoint0.transform.position;
		Vector3 p1 = bezier.ControlPoint1.transform.position;
		Vector3 p2 = bezier.ControlPoint2.transform.position;
		Vector3 p3 = bezier.ControlPoint3.transform.position;

		List<Vector3> controlPoints = new List<Vector3>() { p0, p1, p2, p3 };

		float resolucionBezier = 0.02f;

		bezier._points = bezier.CreateBezierPoints(controlPoints, resolucionBezier);

		// display an orange disc where the object is
		Handles.color = Color.yellow;
		for (int i = 0; i < bezier._points.Count - 1; i++)
		{
			Handles.DrawLine(bezier._points[i], bezier._points[i+1], 5.0f);
		}

		//Handles.DrawBezier(
		//	bezier.ControlPoint0.transform.position,
		//	bezier.ControlPoint3.transform.position,
		//	bezier.ControlPoint1.transform.position,
		//	bezier.ControlPoint2.transform.position,
		//	Color.blue, Texture2D.whiteTexture, 5.0f
		//	);

		// display object "value" in scene
		GUI.color = Color.yellow;
		Handles.Label(bezier._points[bezier._points.Count / 2] + Vector3.up, "Curva de Bezier");
	}
}
*/
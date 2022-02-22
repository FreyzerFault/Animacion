using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Bezier
{
	public class Bezier : MonoBehaviour
	{
		public GameObject ControlPoint0;
		public GameObject ControlPoint1;
		public GameObject ControlPoint2;
		public GameObject ControlPoint3;

		public GameObject aircraft;

		public List<Vector3> _points = new List<Vector3>();

		static int frame = 0;

		// Start is called before the first frame update
		void Awake()
		{
		}

		// Update is called once per frame
		void Update()
		{
			float deltaTime = Time.deltaTime;

			Vector3 p0 = ControlPoint0.transform.position;
			Vector3 p1 = ControlPoint1.transform.position;
			Vector3 p2 = ControlPoint2.transform.position;
			Vector3 p3 = ControlPoint3.transform.position;

			List<Vector3> controlPoints = new List<Vector3>() { p0, p1, p2, p3 };

			float resolucionBezier = 0.02f;

			if (_points.Count == 0)
				CreateBezierPoints(controlPoints, resolucionBezier);

			Vector3 point = _points[frame];
			Vector3 vector = new Vector3();

			if (frame != _points.Count - 1)
				vector = _points[frame + 1] - _points[frame];


			aircraft.transform.position = aircraft.transform.position + vector;

			frame = (frame + 1) % _points.Count;
		}

		public List<Vector3> CreateBezierPoints(List<Vector3> controlPoints, float incrementoT)
		{
			List<Vector3> points = new List<Vector3>();

			Vector3 inicio = controlPoints[0];
			Vector3 final = controlPoints[controlPoints.Count - 1];

			// Grado n
			int n = controlPoints.Count - 1;

			// Aumentamos un incremento de t a partir del primer punto
			Vector3 puntoIncrementado = inicio;
			float t = incrementoT;

			points.Add(inicio);

			while (t <= 1)
			{
				// Punto por cada incremento de t
				Vector3 p = new Vector3(0.0f, 0.0f, 0.0f);

				for (int i = 0; i <= n; ++i)
				{
					float comb = Combinatoria(n, i);

					Vector3 pi = controlPoints[i];
				
					// GRADO n ==> p(t) = SUM[i=0,n] ( comb(n , i) * Pi * (1-t)^(n-i) * t^i )
					p = p + (pi * (comb * Mathf.Pow(1 - t, n - i) * Mathf.Pow(t, i)));
				}
				points.Add(p);

				t += incrementoT;
			}

			points.Add(final);
		
			return points;
		}

		// COMBINATORIA
		float Combinatoria(float n, float k)
		{
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

			return comb / factorial;
		}
	}

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
}
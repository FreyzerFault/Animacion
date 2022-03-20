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
	public Bezier[] Beziers;

	private int bezierIndex;
	public bool InBezier = true;


	// Start is called before the first frame update
	protected void Start()
	{
		if (Beziers.Length > 0)
			Bezier = Beziers[0];
	}

	private void OnEnable()
	{
		StartBezier();
	}

	// Update each frame
	protected void Update()
	{
		if (InBezier && Beziers.Length > 0 && Bezier.AnimationFinished())
		{
			// Cuando ha terminado la animacion si hay varias bezier conectadas, se desactiva la anterior
			FinishBezier();

			// Se activa la siguiente
			bezierIndex = (bezierIndex + 1) % Beziers.Length;
			Bezier = Beziers[bezierIndex];
			StartBezier();
		}
	}

	public void StartBezier()
	{
		Bezier.SetObjectMoving(gameObject);
		Bezier.StartMoving();
	}

	public void FinishBezier()
	{
		Bezier.StopMoving();
	}
}

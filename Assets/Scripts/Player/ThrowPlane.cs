using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowPlane : MonoBehaviour
{
	public GameObject Plane;
	public GameObject Hand;

	private Animator animator;

	private float time = 0;

	private bool planeThrowed = false;

	// Start is called before the first frame update
	void Start()
	{
		animator = GetComponent<Animator>();

		// Deshabilita la curva para que no se mueva
		if (Plane)
			Plane.GetComponent<BezierMovable>().enabled = false;
	}

	// Update is called once per frame
	void Update()
	{
		time += Time.deltaTime;

		if (time > 3)
		{
			// Activa la animacion de lanzar el avion
			animator.SetBool("throwPlane", true);
			
			if (!planeThrowed && time > 3.5)
			{
				planeThrowed = true;
				// Se lanza por la Curva
				if (Plane)
				{
					BezierMovable bm = Plane.GetComponent<BezierMovable>();
					bm.enabled = true;

					ControlPoint cp0 = Plane.GetComponent<BezierMovable>().Bezier.getControlPoint(0);
					if (cp0)
						cp0.transform.position = Hand.transform.position;

					bm.StartBezier();
				} 
			}
		}
		else if (!planeThrowed)
		{
			// Se mueve hacia adelante
			transform.position += transform.forward * Time.deltaTime;

			// Fija la posicion del avion a la mano
			if (Plane && Hand)
				Plane.transform.position = Hand.transform.position + Vector3.down * 0.1f;
		}
	}
}

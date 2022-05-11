using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragPendulum : DragObject
{
	private Vector3 target;
	private Vector3 origin;
	private Pendulum p1;
	private Pendulum p2;
	private DoublePendulum doublePendulum;

	new void OnMouseDrag()
	{
		// Mover el objeto hacia el cursor
		target = GetMouseWorldPos() + offset;

		p2 = GetComponent<Pendulum>();
		p1 = p2.transform.parent.GetComponent<Pendulum>();
		doublePendulum = p1.transform.parent.GetComponent<DoublePendulum>();

		if (!p1 || !p2 || !doublePendulum)
			return;

		origin = doublePendulum.transform.position;

		float x = (target - origin).x;
		float y = (target - origin).y;
		float l1 = p1.ropeLength;
		float l2 = p2.ropeLength;

		if ((target - origin).magnitude >= l1 + l2)
		{
			target = origin + (target - origin).normalized * (l1 + l2);
			x = (target - origin).x;
			y = (target - origin).y;
		}

		float theta2 = Mathf.Acos(
			(x*x + y*y - l1*l1 - l2*l2) / (2*l1*l2)
			);

		float theta1 = Mathf.Atan(
				(
					-(l2 * Mathf.Sin(theta2)) * x
					+ (l1 + l2 * Mathf.Cos(theta2)) * y
				) 
				/ 
				(
					(l2 * Mathf.Sin(theta2)) * y
					+ (l1 + l2 * Mathf.Cos(theta2)) * x
				)
			);

		print("Theta1: " + theta1 + "; Theta2: " + theta2);

		p1.angle = Mathf.Rad2Deg * theta1 + 90;
		p2.angle = Mathf.Rad2Deg * theta2;

		// Actualiza la posicion de los pendulos segun los angulos calculados
		doublePendulum.updatePendulumPosition();
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawSphere(target, .1f);

		Gizmos.color = Color.red;
		Gizmos.DrawSphere(origin, .1f);
	}
}

using UnityEngine;

public class DragPendulum : DragObject
{
	private Vector3 _target;
	private Vector3 _origin;
	private Pendulum _p1;
	private Pendulum _p2;
	private DoublePendulum _doublePendulum;

	private new void OnMouseDrag()
	{
		// Mover el objeto hacia el cursor
		_target = GetMouseWorldPos() + offset;

		_p2 = GetComponent<Pendulum>();
		_p1 = _p2.transform.parent.GetComponent<Pendulum>();
		_doublePendulum = _p1.transform.parent.GetComponent<DoublePendulum>();

		if (!_p1 || !_p2 || !_doublePendulum)
			return;

		_origin = _doublePendulum.transform.position;

		var l1 = _p1.ropeLength;
		var l2 = _p2.ropeLength;

		if ((_target - _origin).magnitude >= l1 + l2)
			_target = _origin + (_target - _origin).normalized * (l1 + l2);
		
		_target.x = Mathf.Clamp(_target.x, 0, l1+l2);

		var x = (_target - _origin).x;
		var y = (_target - _origin).y;

		var theta2 = Mathf.Acos(
			(x*x + y*y - l1*l1 - l2*l2) / (2*l1*l2)
			);

		var theta1 = Mathf.Atan(
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

		_p1.angle = Mathf.Rad2Deg * theta1 + 90;
		_p2.angle = Mathf.Rad2Deg * theta2;

		// Actualiza la posicion de los pendulos segun los angulos calculados
		_doublePendulum.UpdatePendulumPosition();
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawSphere(_target, .1f);

		Gizmos.color = Color.red;
		Gizmos.DrawSphere(_origin, .1f);
	}
}

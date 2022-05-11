using System;
using UnityEngine;

public class Pendulum : MonoBehaviour
{
	public float AnimationSpeed = .01f;
	
	public float angle = 60;
	public float ropeLength = 1;

	public float angularVel = 0;
	public float angularForce = 1f;
	

	public void resetMovement()
	{
		Vector3 ropeDir = transform.position - transform.parent.position;
		ropeLength = ropeDir.magnitude;
		angle = Mathf.Acos(Vector3.Dot(ropeDir.normalized, new Vector3(0, ropeDir.y, 0).normalized));
		angularVel = 0;
		angularForce = 1f;
	}

	public void updateAngle(float force)
	{
		angularForce = force;
		angularVel += angularForce;
		angle += angularVel;
	}

	public void updatePosition()
	{
		transform.localPosition =
			new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle), -Mathf.Cos(Mathf.Deg2Rad * angle), 0);
		transform.localPosition *= ropeLength;
	}

	float getSinglePendulumForce()
	{
		Rigidbody rb = GetComponent<Rigidbody>();

		float m = rb.mass;
		float g = Physics.gravity.magnitude;

		return -m * g * Mathf.Sin(Mathf.Deg2Rad * angle) * Time.fixedDeltaTime * AnimationSpeed;
	}
}

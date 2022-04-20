using System;
using UnityEngine;

public class Pendulum : MonoBehaviour
{
	public float speed = .01f;
	
	public float angle;
	public Vector3 ropeDir;

	public float angularVel = 0;
	public float angularForce = 0.1f;

	void Start()
	{
		ropeDir = transform.position - transform.parent.position;
		angle = Mathf.Acos(Vector3.Dot(ropeDir.normalized, new Vector3(0, ropeDir.y, 0).normalized));
	}

	void Update()
	{
		ropeDir = transform.position - transform.parent.position;
		
		//angularForce = getSinglePendulumForce();

		angularVel += angularForce;
		angle += Mathf.Deg2Rad * angularVel;

		transform.localPosition = new Vector3(Mathf.Sin(angle), -Mathf.Cos(angle), 0);
		transform.localPosition *= ropeDir.magnitude;
	}

	float getSinglePendulumForce()
	{
		Rigidbody rb = GetComponent<Rigidbody>();

		float m = rb.mass;
		float g = Physics.gravity.magnitude;

		return -m * g * Mathf.Sin(angle) * Time.deltaTime * speed;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(transform.parent.position, transform.position);
		
		Gizmos.color = Color.black;
		Gizmos.DrawLine(transform.position, transform.position + GetComponent<Rigidbody>().velocity);
	}
}

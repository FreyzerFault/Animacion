using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DoublePendulum : MonoBehaviour
{
	public Pendulum p1;
	public Pendulum p2;

	public float initAngle1 = 100;
	public float initAngle2 = -30;

	public float ropeLength1 = 5;
	public float ropeLength2 = 5;

	public float animationSpeed = 1;

	public Color color;

	public float minAngularVelocity = -10;
	public float maxAngularVelocity = 10;

	private LineRenderer lineRenderer;

	public bool active = false;

	void Start()
	{
		if (GetComponent<LineRenderer>() != null)
		{
			lineRenderer = GetComponent<LineRenderer>();
			lineRenderer.startWidth = transform.lossyScale.x * .1f;
			lineRenderer.endWidth = transform.lossyScale.x * .1f;
		}


		// Cambiar el Color del trazo
		ParticleSystem ps = p2.GetComponent<ParticleSystem>();
		ParticleSystem.TrailModule psTrails = ps.trails;
		psTrails.inheritParticleColor = false;
		psTrails.colorOverLifetime = color;

		init();
		updatePendulumPosition();
	}

	void Update()
	{
		if (lineRenderer != null)
		{
			lineRenderer.SetPosition(0, transform.position);
			lineRenderer.SetPosition(1, p1.transform.position);
			lineRenderer.SetPosition(2, p2.transform.position);
		}

		// Con espacio toggle entre activo e inactivo
		if (Input.GetKeyDown(KeyCode.Space))
		{
			active = !active;
			reset();
		}
	}
	
	void FixedUpdate()
	{
		if (!active)
		{
			return;
		}

		move();
		updatePendulumPosition();
	}

	void move()
	{
		float g = Physics.gravity.magnitude * Time.fixedDeltaTime * Time.fixedDeltaTime;
		float m1 = p1.GetComponent<Rigidbody>().mass;
		float m2 = p2.GetComponent<Rigidbody>().mass;

		float l1 = p1.ropeLength;
		float l2 = p2.ropeLength;

		float a1 = Mathf.Deg2Rad * p1.angle;
		float a2 = Mathf.Deg2Rad * p2.angle;
		float v1 = p1.angularVel;
		float v2 = p2.angularVel;

		p1.angularForce = -g * (2 * m1 + m2) * Mathf.Sin(a1)
		                  - m2 * g * Mathf.Sin(a1 - 2 * a2)
		                  - 2 * Mathf.Sin(a1 - a2)
		                      * m2 * (v2 * v2 * l2 + v1 * v1 * l1 * Mathf.Cos(a1 - a2))
		                  / (l1 * (2 * m1 + m2 - m2 * Mathf.Cos(2 * a1 - 2 * a2)));
		p2.angularForce = 2 * Mathf.Sin(a1 - a2)
		                    * (v1 * v1 * l1 * (m1 + m2) + g * (m1 + m2) * Mathf.Cos(a1)
		                                                + v2 * v2 * l2 * m2 * Mathf.Cos(a1 - a2))
		                  / (l2 * (2 * m1 + m2 - m2 * Mathf.Cos(2 * a1 - 2 * a2)));

		p1.angularForce *= Time.fixedDeltaTime * animationSpeed;
		p2.angularForce *= Time.fixedDeltaTime * animationSpeed;

		p1.angularForce = Mathf.Clamp(p1.angularForce, -1, 1);
		p2.angularForce = Mathf.Clamp(p2.angularForce, -1, 1);


		p1.angularVel += p1.angularForce;
		p2.angularVel += p2.angularForce;

		// Limit Velocity:
		p1.angularVel = Mathf.Clamp(p1.angularVel, minAngularVelocity, maxAngularVelocity);
		p2.angularVel = Mathf.Clamp(p2.angularVel, minAngularVelocity, maxAngularVelocity);

		p1.angle += p1.angularVel;
		p2.angle += p2.angularVel;
	}

	public void updatePendulumPosition()
	{
		p1.transform.position = transform.position 
		                        + Quaternion.Euler(0, 0, p1.angle) 
		                        * Vector3.down 
		                        * p1.ropeLength;

		p2.transform.position = p1.transform.position 
		                        + Quaternion.Euler(0, 0, p2.angle) 
		                        * (p1.transform.position - transform.position).normalized
		                        * p2.ropeLength;
		
	}

	void reset()
	{
		Vector3 rope1 = p1.transform.localPosition;
		Vector3 rope2 = p2.transform.localPosition;
		p1.ropeLength = rope1.magnitude;
		p2.ropeLength = rope2.magnitude;

		// ArcoCoseno de la proyeccion de la cuerda sobre su eje cuyo angulo = 0
		p1.angle = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(rope1.normalized, Vector3.down));
		p2.angle = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(rope2.normalized, rope1.normalized));

		if (Vector3.Dot(rope1.normalized, Vector3.right) < 0)
			p1.angle = -p1.angle;
		if (Vector3.Dot(rope2.normalized, Vector3.Cross(rope1.normalized, Vector3.forward)) > 0)
			p2.angle = -p2.angle;

		p1.angularVel = 0;
		p2.angularVel = 0;
	}

	void init()
	{
		p1.ropeLength = ropeLength1;
		p2.ropeLength = ropeLength2;

		p1.angle = initAngle1;
		p2.angle = initAngle2;
	}

	void OnDrawGizmos()
	{
		Vector3 rope1 = p1.transform.localPosition;
		Vector3 rope2 = p2.transform.localPosition;

		Gizmos.color = Color.blue;
		Gizmos.DrawLine(transform.position, transform.position + rope1);
		Gizmos.DrawLine(p1.transform.position, p1.transform.position + rope2);

		// ArcoCoseno de la proyeccion de la cuerda sobre su eje cuyo angulo = 0
		float angle1 = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(rope1.normalized, Vector3.down));
		float angle2 = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(rope2.normalized, rope1.normalized));
		
		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, transform.position + Vector3.down * Vector3.Dot(rope1.normalized, Vector3.down));
		Gizmos.DrawLine(p1.transform.position, p1.transform.position + rope1.normalized * Vector3.Dot(rope2.normalized, rope1.normalized));
		
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(p1.transform.position, p1.transform.position + Vector3.Cross(rope1.normalized, Vector3.forward) * Vector3.Dot(rope2.normalized, Vector3.Cross(rope1.normalized, Vector3.forward)));
		
	}
}

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

	void Start()
	{
		// Cambiar el Color del trazo
		ParticleSystem ps = p2.GetComponent<ParticleSystem>();
		ParticleSystem.MainModule psMain = ps.main;
		psMain.startColor = new ParticleSystem.MinMaxGradient(color);

		ParticleSystem.TrailModule psTrails = ps.trails;
		psTrails.colorOverLifetime = color;

		p1.ropeLength = ropeLength1;
		p2.ropeLength = ropeLength2;

		p1.angle = initAngle1;
		p2.angle = initAngle2;

		Vector3 pos = transform.position;
		p1.transform.localPosition = new Vector3(Mathf.Sin(Mathf.Deg2Rad * initAngle1) * p1.ropeLength, Mathf.Cos(Mathf.Deg2Rad * initAngle1) * p1.ropeLength, 0);
		p2.transform.localPosition = new Vector3(Mathf.Sin(Mathf.Deg2Rad * initAngle2) * p2.ropeLength, Mathf.Cos(Mathf.Deg2Rad * initAngle2) * p2.ropeLength, 0);
	}
	
	void FixedUpdate()
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
}

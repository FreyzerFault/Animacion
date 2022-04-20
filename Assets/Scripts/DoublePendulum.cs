using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DoublePendulum : MonoBehaviour
{
	public Pendulum p1;
	public Pendulum p2;

	public float speed = .05f;

	public Color color;

	void Start()
	{
		//ParticleSystem ps = p2.GetComponent<ParticleSystem>();
		//ParticleSystem.MainModule mainModule = ps.main;
		//mainModule.startColor = color;
	}
	
	void Update()
	{
		float g = Physics.gravity.magnitude;
		float m1 = p1.GetComponent<Rigidbody>().mass;
		float m2 = p2.GetComponent<Rigidbody>().mass;

		float l1 = p1.ropeDir.magnitude;
		float l2 = p2.ropeDir.magnitude;

		float a1 = p1.angle;
		float a2 = p2.angle;
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

		p1.angularForce *= Time.deltaTime * speed;
		p2.angularForce *= Time.deltaTime * speed;
	}
}

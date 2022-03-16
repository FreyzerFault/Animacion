using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class GiroPermanente : MonoBehaviour
{
	public float bounceForce;
	public float rotationSpeed;

	private GameObject player;
	private Rigidbody m_PlayerBody;

	// Start is called before the first frame update
	void Awake()
	{
		if (GameObject.FindGameObjectsWithTag("Player").Length <= 0)
		{
			Debug.LogWarning("No existe ningun objecto con la Tag 'Player'");
			return;
		}
		player = GameObject.FindGameObjectWithTag("Player");
		m_PlayerBody = player.GetComponent<Rigidbody>();
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		Rotate();
	}

	void Rotate()
	{
		transform.Rotate(Vector3.down, Time.deltaTime * rotationSpeed);
	}

	private Collision m_Collision;
	private bool bouncing = false;

	void OnCollisionEnter(Collision collision)
	{
		m_Collision = collision;
		// Bounce back
		if (collision.body == m_PlayerBody && !bouncing)
		{
			print("Collision with player");
			Vector3 up = Vector3.up;
			m_PlayerBody.velocity = Vector3.zero;
			m_PlayerBody.AddForce((up - m_Collision.contacts[0].normal) * bounceForce);
			Invoke("BounceBack", 0.05f);
			bouncing = true;
			Invoke("StopBounce", 0.5f);
		}
	}

	void BounceBack()
	{
		print("Bounce Back");
		m_PlayerBody.AddForce(-m_Collision.contacts[0].normal * bounceForce);
	}

	void StopBounce()
	{
		bouncing = false;
	}
}

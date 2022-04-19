using UnityEngine;

public class Pendulum : MonoBehaviour
{
	public Transform sujecion;

	private float startAngle;
	private float angle;
	private Vector3 ropeDir;

	void Start()
	{
		startAngle = Mathf.Acos(Vector3.Dot(ropeDir.normalized, new Vector3(0, ropeDir.y, 0).normalized));
	}

	void Update()
	{
		ropeDir = sujecion.position - transform.position;

		angle = Mathf.Acos(Vector3.Dot(ropeDir.normalized, new Vector3(0, ropeDir.y, 0).normalized));
		if (ropeDir.x > 0)
			angle = -angle;

		Rigidbody rb = GetComponent<Rigidbody>();

		rb.AddForce(
			-Physics.gravity.magnitude * rb.mass * Mathf.Sin(angle) * Vector3.Cross(ropeDir.normalized, Vector3.forward)
		);
		
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(sujecion.position, transform.position);
		

		Gizmos.color = Color.black;
		Gizmos.DrawLine(transform.position, transform.position + GetComponent<Rigidbody>().velocity);

		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, transform.position + -Physics.gravity.magnitude * GetComponent<Rigidbody>().mass * Mathf.Sin(angle) * Vector3.Cross(ropeDir.normalized, Vector3.forward));
	}
}

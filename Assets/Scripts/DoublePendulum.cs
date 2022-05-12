 using UnityEngine;

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

	private LineRenderer _lineRenderer;

	public bool active = true;
	private Rigidbody _p1Rb;
	private Rigidbody _p2Rb;

	public bool stopControlActivated;

	private void Start()
	{
		_p2Rb = p2.GetComponent<Rigidbody>();
		_p1Rb = p1.GetComponent<Rigidbody>();
		_lineRenderer = GetComponent<LineRenderer>(); 
		if (_lineRenderer)
		{
			_lineRenderer.positionCount = 3;
			var lossyScale = transform.lossyScale;
			_lineRenderer.startWidth = lossyScale.x * .1f;
			_lineRenderer.endWidth = lossyScale.x * .1f;
		}


		// Cambiar el Color del trazo
		var ps = p2.GetComponent<ParticleSystem>();
		var psTrails = ps.trails;
		psTrails.inheritParticleColor = false;
		psTrails.colorOverLifetime = color;

		Init();
		UpdatePendulumPosition();
	}

	private void Update()
	{
		if (_lineRenderer)
		{
			_lineRenderer.SetPosition(0, transform.position);
			_lineRenderer.SetPosition(1, p1.transform.position);
			_lineRenderer.SetPosition(2, p2.transform.position);
		}

		// Con espacio toggle entre activo e inactivo
		if (!stopControlActivated) return;
		if (!Input.GetKeyDown(KeyCode.Space)) return;
		
		active = !active;
		Reset();
	}

	private void FixedUpdate()
	{
		if (!active)
			return;

		Move();
		UpdatePendulumPosition();
	}

	private void Move()
	{
		var g = Physics.gravity.magnitude * Time.fixedDeltaTime * Time.fixedDeltaTime;
		var m1 = _p1Rb.mass;
		var m2 = _p2Rb.mass;

		var l1 = p1.ropeLength;
		var l2 = p2.ropeLength;

		var a1 = Mathf.Deg2Rad * p1.angle;
		var a2 = Mathf.Deg2Rad * p2.angle;
		var v1 = p1.angularVel;
		var v2 = p2.angularVel;

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

	public void UpdatePendulumPosition()
	{
		var thisPos = transform.position;
		var p1Pos = thisPos 
		            + Quaternion.Euler(0, 0, p1.angle) 
		            * Vector3.down 
		            * p1.ropeLength;
		
		p1.transform.position = p1Pos; 
		
		p2.transform.position = p1Pos 
		                        + Quaternion.Euler(0, 0, p2.angle) 
		                        * (p1Pos - thisPos).normalized
		                        * p2.ropeLength;
	}

	private void Reset()
	{
		var rope1 = p1.transform.localPosition;
		var rope2 = p2.transform.localPosition;
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

	private void Init()
	{
		p1.ropeLength = ropeLength1;
		p2.ropeLength = ropeLength2;

		p1.angle = initAngle1;
		p2.angle = initAngle2;
	}

	private void OnDrawGizmos()
	{
		var rope1 = p1.transform.localPosition;
		var rope2 = p2.transform.localPosition;

		Gizmos.color = Color.blue;
		var position = transform.position;
		var p1Pos = p1.transform.position;

		Gizmos.color = Color.red;
		Gizmos.DrawLine(position, position + Vector3.down * Vector3.Dot(rope1.normalized, Vector3.down));
		Gizmos.DrawLine(p1Pos, p1Pos + rope1.normalized * Vector3.Dot(rope2.normalized, rope1.normalized));
		
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(p1Pos, p1Pos + Vector3.Cross(rope1.normalized, Vector3.forward) * Vector3.Dot(rope2.normalized, Vector3.Cross(rope1.normalized, Vector3.forward)));
		
	}
}

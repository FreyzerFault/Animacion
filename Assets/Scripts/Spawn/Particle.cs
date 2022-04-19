using UnityEngine;

public class Particle : Spawneable
{
	public float speed = 1f;
	public float lifeTime = 2;
	public float size = 1;
	public Color color = Color.white;

	public Vector3 orientation = Vector3.up;

	private float timeAlive = 0;

	public bool snow = false;
	public bool gravity = false;

	protected override void OnEnable()
	{
	}

	protected override void OnDisable()
	{
	}

	void Update()
	{
		if (timeAlive < lifeTime)
		{
			timeAlive += Time.deltaTime;

			if (snow)
				snowEffect(80);
		}
		else
		{
			base.Destroy();
		}
	}


	public void Initialize()
	{
		Rigidbody rb = GetComponent<Rigidbody>();
		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

		rb.velocity = orientation * speed;
		
		rb.useGravity = gravity;

		transform.localScale = Vector3.one * size;

		meshRenderer.material.color = color;

		timeAlive = 0;
	}

	private void snowEffect(float maxAngle)
	{
		// Utilizando Perlin Noise con el valor de su tiempo de vida conseguimos valores similares
		// Por lo que si interpolamos entre la maxima y minima rotacion, en ambos ejes,
		// Conseguimos rotaciones similares a cada frame interpolando [-.5, .5]

		Quaternion xRotation = Quaternion.Slerp(
			Quaternion.Euler(maxAngle, 0, 0),
			Quaternion.Euler(-maxAngle, 0, 0),
			Mathf.PerlinNoise(timeAlive, 0)
			);
		Quaternion zRotation = Quaternion.Slerp(
			Quaternion.Euler(0, 0, maxAngle),
			Quaternion.Euler(0, 0, -maxAngle),
			Mathf.PerlinNoise(0, timeAlive)
			);

		Rigidbody rb = GetComponent<Rigidbody>();
		rb.velocity = xRotation * zRotation * orientation * speed;
	}

	void OnDrawGizmos()
	{
		//if (GetComponent<Rigidbody>())
		//	Gizmos.DrawLine(transform.position, transform.position + GetComponent<Rigidbody>().velocity);
	}
}

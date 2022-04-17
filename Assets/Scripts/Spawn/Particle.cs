using UnityEngine;

public class Particle : Spawneable
{
	public float speed = 1f;
	public float lifeTime = 2;
	public float size = 1;
	public float gravityModifier = 0;
	public Color color = Color.white;

	public Vector3 orientation = Vector3.up;

	private float timeAlive = 0;

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
		
		if (gravityModifier == 0)
			rb.useGravity = false;
		else
			rb.mass *= gravityModifier;

		transform.localScale = Vector3.one * size;

		meshRenderer.material.color = color;

		timeAlive = 0;
	}
}

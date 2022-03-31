using System.Collections;
using UnityEngine;

public class Bullet : Spawneable
{
	public float MaxTimeAlive = 2;
	public float damage = 5;
	private float timeAlive = 0;

	void Awake()
	{
		base.Awake();

		Physics.IgnoreCollision(GetComponent<Collider>(), spawner.GetComponentInParent<CapsuleCollider>());
	}

	void FixedUpdate()
	{
		timeAlive += Time.fixedDeltaTime;

		if (timeAlive > MaxTimeAlive)
			Destroy();
	}

	// Update is called once per frame
	void OnTriggerEnter(Collider other)
	{
		Destroy();

		// Hit Player
		if (other.gameObject.tag == "Player")
		{
			other.GetComponent<Health>().doDamage(damage);
		}
	}

	public override void OnEnable()
	{
		timeAlive = 0;
	}

	public override void OnDisable()
	{
		StopAllCoroutines();
		timeAlive = 0;
	}
}

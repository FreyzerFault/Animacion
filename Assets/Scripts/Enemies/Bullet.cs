using System.Collections;
using UnityEngine;

public class Bullet : Spawneable
{
	public float MaxTimeAlive = 2;
	public float damage = 1;
	private float timeAlive = 0;

	public ParticleSystem HitParticles;
	public ParticleSystem ShootParticles;

	protected override void Awake()
	{
		base.Awake();

		if (GetComponent<Collider>() && spawner.GetComponentInParent<CapsuleCollider>())
			Physics.IgnoreCollision(GetComponent<Collider>(), spawner.GetComponentInParent<CapsuleCollider>());
	}

	void FixedUpdate()
	{
		timeAlive += Time.fixedDeltaTime;

		if (timeAlive > MaxTimeAlive)
		{
			Destroy();
		}
	}

	// Update is called once per frame
	void OnTriggerEnter(Collider other)
	{
		StartCoroutine(Hit());

		// Hit Player
		if (other.GetComponent<Health>())
		{
			other.GetComponent<Health>().doDamage(damage);
		}
	}

	public override void OnEnable()
	{
		timeAlive = 0;
		ShootEffects();
	}

	public override void OnDisable()
	{
		StopAllCoroutines();
		timeAlive = 0;
	}

	public override void Destroy()
	{
		base.Destroy();

		if (GetComponent<Rigidbody>())
			GetComponent<Rigidbody>().velocity = Vector3.zero;
	}

	private void ShootEffects()
	{
		if (ShootParticles)
			ShootParticles.Play();
		
		if (GetComponents<AudioSource>().Length >= 1)
			GetComponents<AudioSource>()[0].Play();
	}

	private IEnumerator Hit()
	{
		//HitParticles = GetComponentInChildren<ParticleSystem>();
		if (HitParticles)
			HitParticles.Play();
		
		if (GetComponents<AudioSource>().Length >= 2)
			GetComponents<AudioSource>()[1].Play();

		yield return new WaitForSeconds(.3f);

		Destroy();
	}
}

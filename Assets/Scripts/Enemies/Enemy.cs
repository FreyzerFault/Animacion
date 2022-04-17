using System.Collections;
using UnityEditor;
using UnityEngine;

public class Enemy : Spawneable
{
	// SHOOTING
	public Gun gun;
	public float shootFrecuency = 1.5f;

	// MOVEMENT
	public float rotationSpeed = 0.5f;

	// TARGET
	private Transform target;
	private Vector3 targetDir;

	public ParticleSystem DeathParticles;

	protected override void Awake()
	{
		base.Awake();
		
		gun = GetComponent<Gun>();
	}

	void Update()
	{
		target = GameManager.Player.transform;
		targetDir = (target.position - gun.GetSpawnPoint()).normalized;
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDir), rotationSpeed * Time.deltaTime);
	}
	
	void OnDrawGizmos()
	{
		Gizmos.DrawLine(gun.GetSpawnPoint(), target.position);
	}

	protected override void OnEnable()
	{
		// Inicia el Spawner de balas segun la frecuencia
		int initNumBullets = (int)Mathf.Round(shootFrecuency * 10);

		gun.LoadPool(initNumBullets);

		StartCoroutine(ShootRoutine());
	}

	protected override void OnDisable()
	{
		StopAllCoroutines();

		// Destruye las balas que tenga
		gun.Clear();
	}


	// Dispara una bala con una frecuencia
	IEnumerator ShootRoutine()
	{
		while (true)
		{
			yield return new WaitForSeconds(1 / shootFrecuency + Random.value);

			GameObject bullet = gun.Shoot(targetDir * gun.ShootForce);
		}
	}

	public IEnumerator Death()
	{
		// Hide y Disable Collider
		foreach (SkinnedMeshRenderer child in GetComponentsInChildren<SkinnedMeshRenderer>())
		{
			child.gameObject.SetActive(false);
		}

		GetComponent<Collider>().enabled = false;

		// Play Death Animation
		DeathParticles.Play();

		yield return new WaitForSeconds(.3f);

		foreach (SkinnedMeshRenderer child in GetComponentsInChildren<SkinnedMeshRenderer>())
		{
			child.gameObject.SetActive(true);
		}
		GetComponent<Collider>().enabled = true;

		base.Destroy();
	}
}

using System.Collections;
using UnityEngine;

public class EnemyShoot : Spawneable
{
	// SHOOTING
	public SpawnerBox bulletSpawner => GetComponentInChildren<SpawnerBox>();
	public float shootFrecuency = 1.5f;
	public float shootForce = 10;

	// MOVEMENT
	public float rotationSpeed = 0.5f;

	// TARGET
	private Transform target => GameController.Player.transform;
	private Vector3 targetDir;

	void Update()
	{
		targetDir = (target.position - bulletSpawner.getCenter()).normalized;
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDir), rotationSpeed * Time.deltaTime);
	}

	// Dispara una bala con una frecuencia
	IEnumerator ShootRoutine()
	{
		while (true)
		{
			yield return new WaitForSeconds(1 / shootFrecuency + Random.value);

			GameObject bullet = bulletSpawner.Shoot(targetDir * shootForce);
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawLine(bulletSpawner.getCenter(), target.position);
	}

	public override void OnEnable()
	{
		// Inicia el Spawner de balas segun la frecuencia
		int initNumBullets = (int)Mathf.Round(shootFrecuency * 10);

		bulletSpawner.LoadPool(initNumBullets);

		StartCoroutine(ShootRoutine());
	}

	public override void OnDisable()
	{
		StopAllCoroutines();

		// Destruye las balas que tenga
		bulletSpawner.Clear();
	}
}

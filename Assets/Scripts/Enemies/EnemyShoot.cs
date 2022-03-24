using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
	public GameObject bulletPrefab;
	public Vector3 BulletInitialPoint;

	// SHOOTING
	public float shootFrecuency = 1.5f;
	public float shootForce = 10;
	public float rotationSpeed = 0.5f;

	// TARGET
	private Vector3 target;
	private Vector3 targetDir;

	// BULLETS POOLING
	[SerializeField] private List<GameObject> bulletsPool;
	private int numBullets;
	
	void Awake()
	{
		target = GameObject.FindGameObjectWithTag("Player").transform.position;
		target += BulletInitialPoint - transform.position;
	}

	// Start is called before the first frame update
	void Start()
	{
		
		numBullets = (int)Mathf.Round(shootFrecuency * 10);

		for (int i = 0; i < numBullets; i++)
		{
			GameObject bullet = Instantiate(bulletPrefab, transform);
			bulletsPool.Add(bullet);
			bullet.SetActive(false);
		}

		StartCoroutine(ShootRoutine());
	}

	void Update()
	{
		targetDir = (target - BulletInitialPoint).normalized;
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDir), rotationSpeed * Time.deltaTime);
	}

	IEnumerator ShootRoutine()
	{
		while (true)
		{
			yield return new WaitForSeconds(1 / shootFrecuency + Random.value);


			// Mover de la Pool a la lista de activos
			GameObject bullet = bulletsPool[numBullets - 1];
			bullet.SetActive(true);
			bulletsPool.RemoveAt(numBullets - 1);

			numBullets--;

			Rigidbody rb = bullet.GetComponent<Rigidbody>();
			if (!rb)
				print(ToString() + " No tiene RigidBody!!!!");

			rb.AddForce(targetDir * shootForce);
			bullet.transform.rotation = Quaternion.LookRotation(targetDir);
			bullet.transform.position = BulletInitialPoint;

			Physics.IgnoreCollision(bullet.GetComponent<Collider>(), GetComponent<Collider>());
		}
	}


	public void DisapearBullet(GameObject bullet)
	{
		// Mover de la Pool a la lista de activos
		bullet.transform.position = BulletInitialPoint;
		bullet.GetComponent<Rigidbody>().velocity = Vector3.zero;
		bullet.SetActive(false);
		bulletsPool.Add(bullet);

		numBullets++;
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawLine(BulletInitialPoint, GameObject.FindGameObjectWithTag("Player").transform.position);
	}
}

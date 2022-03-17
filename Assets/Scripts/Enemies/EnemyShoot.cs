using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
	public GameObject bulletPrefab;
	public float shootFrecuency = 1.5f;
	public float shootForce = 10;
	public float rotationSpeed = 0.5f;

	private GameObject target;
	[SerializeField] private List<GameObject> bulletsPool;
	private int numBullets;

	private Vector3 BulletInitPosition;

	private bool rotateTowardsPlayer = false;


	// Start is called before the first frame update
	void Awake()
	{
		target = GameObject.FindGameObjectWithTag("Player");

		BulletInitPosition = transform.position;
		BulletInitPosition += Vector3.up * 1.5f;

							  numBullets = (int)Mathf.Round(shootFrecuency * 10);
		for (int i = 0; i < numBullets; i++)
		{
			GameObject bullet = Instantiate(bulletPrefab, transform);
			// Para que salga a la altura de los ojos
			bullet.transform.position += Vector3.up * 2;
			bulletsPool.Add(bullet);
			bullet.SetActive(false);
		}
	}

	void Start()
	{
		StartCoroutine(ShootRoutine());
		StartCoroutine(CheckIfPlayerInFront());

		if (rotateTowardsPlayer)
		{

			Vector3 targetDir = (target.transform.position - transform.position).normalized;
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDir), rotationSpeed * Time.deltaTime);
		}
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

			Vector3 shootDirection = (target.gameObject.transform.position - transform.position).normalized;

			rb.AddForce(shootDirection * shootForce);
			bullet.transform.rotation = Quaternion.LookRotation(shootDirection);
			bullet.transform.position = BulletInitPosition;
		}
	}

	IEnumerator CheckIfPlayerInFront()
	{
		while (true)
		{
			yield return new WaitForSeconds(Mathf.Abs(Random.value - 0.5f));

			print("Rotate");

			// Mover de la Pool a la lista de activos
			Vector3 targetDir = (target.transform.position - transform.position).normalized;
			Vector3 enemyForward = transform.forward.normalized;

			// Si el DOT es menos de 1 es que no tienen similar direccion
			rotateTowardsPlayer = Vector3.Dot(targetDir, enemyForward) < 0.9;
		}
	}

	public void disapearBullet(GameObject bullet)
	{
		print("Bullet volvio a la Pool");
		
		// Mover de la Pool a la lista de activos
		bullet.transform.position = transform.position;
		bullet.GetComponent<Rigidbody>().velocity = Vector3.zero;
		bullet.SetActive(false);
		bulletsPool.Add(bullet);

		numBullets++;
	}
}

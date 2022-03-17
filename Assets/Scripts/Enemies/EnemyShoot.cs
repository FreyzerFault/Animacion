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

	// Start is called before the first frame update
	void Start()
	{
		target = GameObject.FindGameObjectWithTag("Player");

		BulletInitPosition = transform.position;
		BulletInitPosition += Vector3.up * 1.5f;

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
		Vector3 targetDir = (target.transform.position - transform.position).normalized;
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

			Vector3 shootDirection = (target.gameObject.transform.position - transform.position).normalized;

			rb.AddForce(shootDirection * shootForce);
			bullet.transform.rotation = Quaternion.LookRotation(shootDirection);
			bullet.transform.position = BulletInitPosition;

			Physics.IgnoreCollision(bullet.GetComponent<Collider>(), GetComponent<Collider>());
		}
	}


	public void DisapearBullet(GameObject bullet)
	{
		// Mover de la Pool a la lista de activos
		bullet.transform.position = transform.position;
		bullet.GetComponent<Rigidbody>().velocity = Vector3.zero;
		bullet.SetActive(false);
		bulletsPool.Add(bullet);

		numBullets++;
	}
}

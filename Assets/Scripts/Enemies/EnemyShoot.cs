using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
	public GameObject bulletPrefab;
	public float shootFrecuency = 1.5f;
	public float shootForce = 10;

	private GameObject target;
	[SerializeField] private List<GameObject> bulletsPool;
	private int numBullets;

	private Vector3 BulletInitPosition;

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
		StartCoroutine(shootRoutine());
	}
	

	IEnumerator shootRoutine()
	{
		while(true)
		{
			yield return new WaitForSeconds(1 / shootFrecuency + Random.value);

			print("Shoot");

			// Mover de la Pool a la lista de activos
			GameObject bullet = bulletsPool[numBullets - 1];
			bullet.SetActive(true);
			bulletsPool.RemoveAt(numBullets - 1);

			numBullets--;

			Rigidbody rb = bullet.GetComponent<Rigidbody>();
			if (!rb)
				print(ToString() + " No tiene RigidBody!!!!");

			Vector3 shootDirection = target.gameObject.transform.position - transform.position;

			rb.AddForce(shootDirection * shootForce);
			bullet.transform.rotation = Quaternion.LookRotation(shootDirection);
			bullet.transform.position = BulletInitPosition;
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

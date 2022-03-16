using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	public float MaxTimeAlive = 2;
	public float damage = 5;
	private float timeAlive = 0;

	// Start is called before the first frame update
	void Start()
	{
	}

	void Update()
	{
		timeAlive += Time.deltaTime;

		if (timeAlive > MaxTimeAlive)
			disappear();
	}

	// Update is called once per frame
	void OnTriggerEnter(Collider other)
	{
		disappear();

		if (other.GetComponent<Collider>() == GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>())
		{
			other.GetComponent<Health>().doDamage(damage);
		}
	}

	void disappear()
	{
		GetComponentInParent<EnemyShoot>().disapearBullet(this.gameObject);
		timeAlive = 0;
	}
}

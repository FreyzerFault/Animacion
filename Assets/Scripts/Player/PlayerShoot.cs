using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerShoot : MonoBehaviour
{
	public Gun gun;

	public Transform shootOrigin;
	public float maxShootDistance = 100;

	public float damage = 20;

	public ParticleSystem shootParticles;
	public ParticleSystem hitParticles;

	public AudioSource shootSound;
	public AudioSource hitSound;

	// Start is called before the first frame update
	void Start()
	{
		if (gun == null)
			gun = GetComponent<Gun>();
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse))
		{
			//gun.Shoot(GameManager.MainCamera.transform.forward * gun.ShootForce);

			Shoot();

			// Raycasting
			Ray shootRay = new Ray(shootOrigin.position, shootOrigin.forward);
			
			// No se comprueba directamente si es de enemigo porque podria atravesar algo entre medias
			RaycastHit[] hits = Physics.RaycastAll(shootRay, maxShootDistance, LayerMask.GetMask("Enemy", "Default"));

			if (hits.Length != 0)
			{
				StartCoroutine(ProcessHits(hits));
			}
		}
	}

	// Reproduce los Sonidos y Particulas en cada hit
	// Atraviesa enemigos pero otros objetos no
	IEnumerator ProcessHits(RaycastHit[] hits)
	{
		// Ordenamos los hits por distancia
		for (int i = 0; i < hits.Length; i++)
		{
			for (int j = i+1; j < hits.Length; j++)
			{
				if (hits[j].distance < hits[i].distance)
				{
					// Swap:
					(hits[i], hits[j]) = (hits[j], hits[i]);
				}
			}
		}

		// Por cada hit esperamos un tiempo proporcional a la distancia
		// Siendo 1 segundo el tiempo de vida de la bala
		float lastDistance = 0;
		foreach (var hit in hits)
		{
			yield return new WaitForSeconds((hit.distance - lastDistance) / maxShootDistance);
			lastDistance = hit.distance;

			Hit(hit);

			// Si el hit no es a un enemigo acaba ahi, pero si lo es le hace daño
			if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
			{
				Health enemyHealth = hit.collider.gameObject.GetComponent<Health>();
				if (enemyHealth != null)
					enemyHealth.doDamage(damage);
			}
			else
				break;
		}
	}

	void Hit(RaycastHit hit)
	{
		hitParticles.transform.SetPositionAndRotation(hit.point, Quaternion.LookRotation(hit.normal));
		hitParticles.Play(hit.transform);
		
		hitSound.Play();
	}

	void Shoot()
	{
		
		shootParticles.transform.SetPositionAndRotation(gun.spawnPoint.transform.position, gun.spawnPoint.transform.rotation);
		shootParticles.Play();

		shootSound.Play();
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawLine(shootOrigin.position, shootOrigin.position + shootOrigin.forward * maxShootDistance);
	}
}

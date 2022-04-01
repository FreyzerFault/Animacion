using UnityEngine;
using UnityEngine.UIElements;

public class Gun : Spawner
{
	public float ShootForce = 20f;

	public Transform spawnPoint;

	public ParticleSystem ShootParticles;

	protected override void Awake()
	{
		if (spawnPoint == null)
			spawnPoint = transform;

		// Ignorar las colisiones entre el proyectil y la pistola
		if (GetComponent<Collider>())
			Physics.IgnoreCollision(GetComponent<Collider>(), ObjectPrefab.GetComponent<Collider>());

		base.Awake();
	}

	// Spawnea el objeto disparandolo con una fuerza, orientado o no en la direccion de la fuerza
	public GameObject Shoot(Vector3 force, Quaternion? rotation = null)
	{
		GameObject item = Spawn(spawnPoint.position, rotation ?? Quaternion.LookRotation(force.normalized));

		if (item.GetComponent<Rigidbody>())
			item.GetComponent<Rigidbody>().AddForce(force);
		else
			print("Se ha intentado disparar el objeto que no tiene RigidBody: " + item);

		//ShootEffects(item);

		return item;
	}

	public Vector3 GetSpawnPoint()
	{
		return spawnPoint.position;
	}

	private void ShootEffects(GameObject item)
	{
		if (item.GetComponentInChildren<ParticleSystem>())
			item.GetComponentInChildren<ParticleSystem>().Play();
	}
}

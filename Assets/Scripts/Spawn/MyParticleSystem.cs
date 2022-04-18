using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityStandardAssets.Water;

public class MyParticleSystem : Spawner
{
	[Range(0, 10)] public float startSpd = 5f;
	[Range(0.1f, 100)] public float startLifetime = 2;
	[Range(0.1f, 10)] public float startSize = 1;
	public Gradient startColor;

	[Range(0, 10)] public float delay = 0;
	[Range(0, 10)] public float duration = 5;
	public bool looping = true;

	// Particulas / Segundo
	[Range(0, 1000)] public float rateEmission = 5;

	private float timePlaying = 0;

	// Area de Spawn
	[Range(0, 90)]public float coneAngle;
	[Range(.1f, 10)]public float coneRadius = 1;

	// Aleatoriedad
	public bool randomSpd;
	public bool randomOrientation;
	public bool randomLifeTime;
	public bool randomSize;
	public bool randomColor;

	public bool snowEffect = false;
	public bool useGravity = false;

	void Start()
	{
		StartCoroutine(Play());
	}

	public IEnumerator Play()
	{
		yield return new WaitForSeconds(delay);

		while (looping || timePlaying < duration)
		{
			yield return new WaitForSeconds(1 / rateEmission);

			SpawnParticle();

			timePlaying += Time.deltaTime;
		}
		Debug.Log("Particle System Finished");
	}

	Particle SpawnParticle()
	{
		Vector3 position = transform.position + new Vector3(
			Mathf.Lerp(-coneRadius, coneRadius, Random.value),
			0,
			Mathf.Lerp(-coneRadius, coneRadius, Random.value)
			);

		Particle particle = Spawn(position).GetComponent<Particle>();

		// Set Variables
		particle.speed = randomSpd ? Mathf.Lerp(startSpd - 1, startSpd + 1, Random.value) : startSpd;
		particle.lifeTime = randomLifeTime ? Mathf.Lerp(startLifetime - 1, startLifetime + 1, Random.value) : startLifetime;
		particle.size = randomSize ? Mathf.Lerp(startSize - startSize/10, startSize + startSize / 10, Random.value) : startSize;
		particle.color = randomColor ? startColor.Evaluate(Random.value) : startColor.Evaluate(0);
		particle.orientation = randomOrientation
			? Quaternion.Lerp(
				Quaternion.Euler(0, 0, coneAngle),
				Quaternion.Euler(0, 0, -coneAngle),
				Random.value
			) * Quaternion.Lerp(
				Quaternion.Euler(coneAngle, 0, 0),
				Quaternion.Euler(-coneAngle, 0, 0),
				Random.value
			) * transform.up
			: transform.up;

		particle.gravity = useGravity;

		particle.snow = snowEffect;

		particle.Initialize();

		return particle;
	}
}


[CustomEditor(typeof(MyParticleSystem))]
public class MyParticleSystemEditor : Editor
{
	public override void OnInspectorGUI()
	{
		MyParticleSystem ps = target as MyParticleSystem;
		if (ps == null)
		{
			Debug.Log("No existe ningun objeto MyParticleSystem al que modificar su editor en el inspector");
			return;
		}

		DrawDefaultInspector();
	}

	public void OnSceneGUI()
	{
		MyParticleSystem ps = target as MyParticleSystem;

		float coneHeight = 5;

		Vector3 pos = ps.transform.position;
		Vector3 up = ps.transform.up * coneHeight;
		float radius = ps.coneRadius;
		float upRadius = ps.coneRadius + Mathf.Sin(Mathf.Deg2Rad * ps.coneAngle) * coneHeight / Mathf.Cos(Mathf.Deg2Rad * ps.coneAngle);

		Handles.color = Color.magenta;

		Handles.DrawWireDisc(pos, up, ps.coneRadius, 1);

		Handles.DrawWireDisc(
			pos + up,
			up,
			upRadius,
			1);

		Handles.DrawLine(
			pos + ps.transform.forward * radius,
			pos + up + ps.transform.forward * upRadius
			);
		Handles.DrawLine(
			pos - ps.transform.forward * radius,
			pos + up - ps.transform.forward * upRadius
			);
		Handles.DrawLine(
			pos + ps.transform.right * radius,
			pos + up + ps.transform.right * upRadius
			);
		Handles.DrawLine(
			pos - ps.transform.right * radius,
			pos + up - ps.transform.right * upRadius
			);
	}

}
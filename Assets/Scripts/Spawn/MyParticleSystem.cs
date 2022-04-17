using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityStandardAssets.Water;

public class MyParticleSystem : Spawner
{
	[Range(0, 10)] public float startSpd = 5f;
	[Range(0.1f, 10)] public float startLifetime = 2;
	[Range(0, 10)] public float startDelay = 0;
	[Range(0.1f, 10)] public float startSize = 1;
	[Range(0, 10)] public float gravityModifier = 0;
	public Color startColor = Color.white;
	public Vector3 startOrientation = Vector3.up;

	[Range(0, 10)] public float duration = 5;
	public bool looping = true;

	// Particulas / Segundo
	[Range(0, 100)] public float rateEmission = 5;

	private float timePlaying = 0;

	// Area de Spawn
	public float coneAngle;
	public float coneRadius = 1;
	public float coneThickness;

	void Start()
	{
		StartCoroutine(Play());
	}

	public IEnumerator Play()
	{
		yield return new WaitForSeconds(startDelay);

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
		Particle particle = Spawn().GetComponent<Particle>();

		// Set Variables
		particle.speed = startSpd;
		particle.lifeTime = startLifetime;
		particle.size = startSize;
		particle.gravityModifier = gravityModifier;
		particle.color = startColor;
		particle.orientation = startOrientation;

		particle.Initialize();

		return particle;
	}
}


[CustomEditor(typeof(MyParticleSystem))]
public class MyParticleSystemEditor : Editor
{
	public void OnSceneGUI()
	{
		MyParticleSystem ps = target as MyParticleSystem;

		float coneHeight = 5;

		Vector3 pos = ps.transform.position;
		Vector3 up = ps.transform.up * coneHeight;
		float radius = ps.coneRadius;
		float upRadius = ps.coneRadius + Mathf.Sin(ps.coneAngle);

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
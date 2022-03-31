using System.Collections;
using UnityEngine;

public class Shake : MonoBehaviour
{
	public IEnumerator ShakeOnce()
	{
		// Rotacion random bajo un maximo
		float rotation = ((Random.value - 0.5f) * 2) * 20;
		float scaling = Random.value / 2 + 1;

		transform.Rotate(Vector3.forward, rotation);
		transform.localScale *= scaling;

		yield return new WaitForSeconds(.1f);

		transform.localScale *= 1 / scaling;
		transform.Rotate(Vector3.forward, -rotation);
	}
}

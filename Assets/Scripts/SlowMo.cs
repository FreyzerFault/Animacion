using System.Collections;
using UnityEngine;

public static class SlowMo
{
	public static IEnumerator slowMo(float seconds)
	{
		Time.timeScale = .5f;

		yield return new WaitForSeconds(seconds);

		Time.timeScale = 1;
	}

	public static IEnumerator slowMoPogressive(float seconds)
	{
		Time.timeScale = .5f;
		const float increment = .1f;

		for (float i = seconds; i > 0; i -= increment)
		{
			yield return new WaitForSecondsRealtime(increment);

			Time.timeScale = Mathf.InverseLerp(0, seconds, i / 2);
		}

		Time.timeScale = 1;
	}

	public static IEnumerator slowMoExponential(float seconds)
	{
		Time.timeScale = .5f;
		const float increment = .1f;

		const float exp = 0.5f;

		for (float i = seconds; i > 0; i -= increment)
		{
			yield return new WaitForSecondsRealtime(increment);

			float t = Mathf.InverseLerp(0, seconds, Mathf.Pow(i / 2, exp));

			Time.timeScale = t;
		}

		Time.timeScale = 1;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Alerta : MonoBehaviour
{
	private Text text;

	// Start is called before the first frame update
	void Start()
	{
		text = GetComponent<Text>();
		text.enabled = false;
	}

	public IEnumerator ShowMessage(string msg, float seconds, bool resetGame = false)
	{
		if (resetGame)
			Time.timeScale = .5f;

		text.text = msg;
		text.enabled = true;

		if (resetGame)
		{
			const float increment = .1f;
			for (float i = seconds; i > 0; i -= increment)
			{
				yield return new WaitForSecondsRealtime(increment);

				Time.timeScale = Mathf.InverseLerp(0, seconds, i / 2);
			}
		}
		else
		{
			yield return new WaitForSeconds(seconds);
		}

		text.enabled = false;

		if (resetGame)
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			Time.timeScale = 1;
		}
	}
}

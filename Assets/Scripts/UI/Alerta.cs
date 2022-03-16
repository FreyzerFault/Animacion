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
		text.text = msg;
		text.enabled = true;

		yield return new WaitForSeconds(seconds);

		text.enabled = false;

		if (resetGame)
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}

using System.Collections;
using UnityEngine;
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

		if (resetGame)
			yield return SlowMo.slowMoPogressive(seconds);
		else
			yield return new WaitForSeconds(seconds);

		text.enabled = false;

		if (resetGame)
		{
			SceneController.ReloadScene();
		}
	}

	
}

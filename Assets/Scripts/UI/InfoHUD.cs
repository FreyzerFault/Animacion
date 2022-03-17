using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class InfoHUD : MonoBehaviour
{
	private Text timeText;

	// Start is called before the first frame update
	void Start()
	{
		timeText = GameObject.FindGameObjectWithTag("TimeText").GetComponent<Text>();
	}

	// Update is called once per frame
	void Update()
	{
		string seconds = ((int) Time.timeSinceLevelLoad).ToString();
		string minutes = ((int) Time.timeSinceLevelLoad / 60).ToString();


		StringBuilder sb = new StringBuilder();

		if (minutes.Length < 2) sb.Append("0");
		sb.Append(minutes);

		sb.Append(":");

		if (seconds.Length < 2) sb.Append("0");
		sb.Append(seconds);
		timeText.text = sb.ToString();
	}
}

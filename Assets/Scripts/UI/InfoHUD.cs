using System.Globalization;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class InfoHUD : MonoBehaviour
{
	public Text timeText;

	// Start is called before the first frame update
	void Start()
	{
		timeText = GameObject.FindGameObjectWithTag("TimeText").GetComponent<Text>();
	}

	// Update is called once per frame
	void Update()
	{
		UpdateTimeText((int)Time.timeSinceLevelLoad);
	}


	void UpdateTimeText(int time)
	{
		string minutes = Mathf.Floor(time / 60f).ToString(CultureInfo.CurrentCulture);
		string seconds = (time % 60).ToString(CultureInfo.CurrentCulture);

		StringBuilder sb = new StringBuilder();

		if (minutes.Length < 2) sb.Append("0");
		sb.Append(minutes);

		sb.Append(":");

		if (seconds.Length < 2) sb.Append("0");
		sb.Append(seconds);
		timeText.text = sb.ToString();
	}
}

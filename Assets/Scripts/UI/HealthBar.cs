using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
	public Slider slider;
	public Gradient gradient = new Gradient();

	void Awake()
	{
		slider = GetComponent<Slider>();
	}
	
	public void SetHealth(float health)
	{
		if (health < 0)
			return;

		slider.value = health;

		// Cambia la barra de color
		Image bar = GetComponentInChildren<Image>();
		if (bar != null)
		{
			float t = slider.normalizedValue;
			
			Color barColor = bar.color;
			barColor = gradient.Evaluate(t);
			bar.color = barColor;
		}

		Text text = GetComponentInChildren<Text>();
		if (text != null)
		{
			text.text = health.ToString(CultureInfo.CurrentCulture);
		}
	}

	public void SetMaxHealth(float health)
	{
		slider.maxValue = health;
	}
}

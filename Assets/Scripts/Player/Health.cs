using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
	public float MaxHealth = 100;

	public GameObject HealthSprite;

	private Image greenBar;
	private Image redBar;
	private Image icon;

	[SerializeField] private float health;

	// Start is called before the first frame update
	void Start()
	{
		HealthSprite = GameObject.FindGameObjectWithTag("Health");

		Image[] images = HealthSprite.GetComponentsInChildren<Image>();

		redBar = images[1];
		greenBar = images[2];
		icon = images[4];

		health = MaxHealth;

		UpdateHealthBar();
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	public void doDamage(float dmg)
	{
		health -= dmg;

		if (health < 0)
		{
			GameObject alertaObj = GameObject.FindGameObjectWithTag("Alerta");
			Alerta alerta = alertaObj.GetComponent<Alerta>();
			if (!alerta)
				print(ToString() + " NO ENCONTRO EL GameObject con TAG = 'Alerta'");

			StartCoroutine(alerta.ShowMessage("YOU LOSE", 3, true));
		}

		UpdateHealthBar();
	}

	public void UpdateHealthBar()
	{
		// Change Color
		float t = Mathf.InverseLerp(0, MaxHealth, health);

		Color green = greenBar.color; 
		
		green.a = t;

		greenBar.color = green;

		// Shrink Bar
		//greenBar.rectTransform.
	}
}

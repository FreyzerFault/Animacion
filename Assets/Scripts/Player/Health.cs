using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
	public float MaxHealth = 100;

	public HealthBar healthBar;
	public Shake healthIcon;

	public float health;

	// Start is called before the first frame update
	void Start()
	{
		health = MaxHealth;

		if (healthBar != null)
		{
			healthBar.SetMaxHealth(MaxHealth);
			healthBar.SetHealth(health);
		}
	}

	public void doDamage(float dmg)
	{
		health -= dmg;

		// Shrink Bar
		if (healthBar != null)
			healthBar.SetHealth(health);

		// Shake Icon
		if (healthIcon != null)
			StartCoroutine(healthIcon.ShakeOnce());

		if (health <= 0)
		{
			if (gameObject.tag == "Player")
			{
				GameObject alertaObj = GameObject.FindGameObjectWithTag("Alerta");
				Alerta alerta = alertaObj.GetComponent<Alerta>();
				if (!alerta)
					print(ToString() + " NO ENCONTRO EL GameObject con TAG = 'Alerta'");

				StartCoroutine(alerta.ShowMessage("YOU LOSE", 5, true));
			}
			else if (GetComponent<EnemyShoot>())
			{
				StartCoroutine(GetComponent<EnemyShoot>().Death());
			}
		}
	}
}

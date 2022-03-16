using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
	public float MaxHealth = 100;

	[SerializeField] private float health;

	// Start is called before the first frame update
	void Start()
	{
		health = MaxHealth;
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
	}
}

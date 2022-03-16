using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinalPosition : MonoBehaviour
{
	public GameObject thePlayer;

	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	void OnTriggerEnter(Collider other)
	{
		if (other == GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>())
		{
			YouWin();
		}

	}

	void YouWin()
	{
		Alerta alerta = GameObject.FindGameObjectWithTag("Alerta").GetComponent<Alerta>();
		if (!alerta)
			print(ToString() + " NO ENCONTRO EL GameObject con TAG = 'Alerta'");

		StartCoroutine(alerta.ShowMessage("YOU WIN", 3, true));
	}

}

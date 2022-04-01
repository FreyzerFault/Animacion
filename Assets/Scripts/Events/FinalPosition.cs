using UnityEngine;

public class FinalPosition : MonoBehaviour
{
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == GameManager.Player)
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

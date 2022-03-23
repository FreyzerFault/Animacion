using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{

	public void switchScene(int i)
	{
		if (i != SceneManager.GetActiveScene().buildIndex)
		{
			SceneManager.LoadScene(i);
		}
	}
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
	public void OnClick(int i)
	{
		if (i != SceneManager.GetActiveScene().buildIndex)
			SceneManager.LoadScene(i);
	}
}

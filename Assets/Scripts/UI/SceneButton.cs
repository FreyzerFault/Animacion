using UnityEngine;

public class SceneButton : MonoBehaviour
{
	public void OnClick(int i)
	{
		SceneController.SwitchScene(i);
	}
}

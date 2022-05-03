using UnityEngine;
using UnityEngine.UI;

public class StereoscopyControl : MonoBehaviour
{
	private Camera leftEyeCam;
	private Camera rigthEyeCam;

	public Slider introcularDistSlider;
	public Slider focalDistSlider;
	public Slider FOVSlider;

	public float introcularDist = 0.5f;
	public float focalDist = 1;
	public float FOV = 60;

	void Start()
	{
		Camera[] cameras = GetComponentsInChildren<Camera>();

		if (cameras.Length == 3){
			leftEyeCam = cameras[1];
			rigthEyeCam = cameras[2];
		}
	}

	void Update()
	{
		introcularDist = introcularDistSlider.value;
		focalDist = focalDistSlider.value;
		FOV = FOVSlider.value;

		Text idlabels = introcularDistSlider.GetComponentInChildren<Text>();
		Text fdlabels = focalDistSlider.GetComponentInChildren<Text>();
		Text fovlabels = FOVSlider.GetComponentInChildren<Text>();

		idlabels.text = "Distancia Introcular: " + introcularDist.ToString();
		fdlabels.text = "Distancia Focal: " + focalDist.ToString();
		fovlabels.text = "FOV: " + FOV.ToString();
		
		UpdateCameras();
	}

	void UpdateCameras()
	{
		leftEyeCam.transform.localPosition = new Vector3(-introcularDist / 2, 0, 0);
		rigthEyeCam.transform.localPosition = new Vector3(introcularDist / 2, 0, 0);

		leftEyeCam.fieldOfView = FOV;
		rigthEyeCam.fieldOfView = FOV;
	}
}

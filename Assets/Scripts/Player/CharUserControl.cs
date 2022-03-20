using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class CharUserControl : MonoBehaviour
{
	private Char character;

	private Animator animator;

	private int isWalkingHash;
	private int isRunningHash;
	private int velocityHash;

	private float speed;


	// Start is called before the first frame update
	void Start()
	{
		character = GetComponent<Char>();
		animator = GetComponent<Animator>();

		isWalkingHash = Animator.StringToHash("isWalking");
		isRunningHash = Animator.StringToHash("isRunning");
		velocityHash = Animator.StringToHash("velocity");
	}

	// Update is called once per frame
	void Update()
	{
		AnimateStateMove(Time.deltaTime);
	}

	void AnimateProgressiveMove(float time)
	{
		bool forwardPressed = Input.GetKey("w");
		bool runPressed = Input.GetKey("left shift");

		speed += (forwardPressed ? time : -time) * (runPressed ? 4 : 1);
		speed = Mathf.Clamp(speed, 0, runPressed ? 2 : 1);

		animator.SetFloat(velocityHash, speed);
	}

	void AnimateStateMove(float time)
	{
		bool forwardPressed = Input.GetKey("w");
		bool runPressed = Input.GetKey("left shift");
		
		animator.SetBool(isWalkingHash, forwardPressed);

		animator.SetBool(isRunningHash, forwardPressed && runPressed);
	}
}

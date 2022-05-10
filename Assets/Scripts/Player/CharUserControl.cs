using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharUserControl : MonoBehaviour
{
	private Char character;

	private Animator animator;

	private int isWalkingHash;
	private int isRunningHash;
	private int velocityXHash;
	private int velocityYHash;

	private Vector2 speed2D;


	// Start is called before the first frame update
	void Start()
	{
		character = GetComponent<Char>();
		animator = GetComponent<Animator>();

		isWalkingHash = Animator.StringToHash("isWalking");
		isRunningHash = Animator.StringToHash("isRunning");
		velocityXHash = Animator.StringToHash("vx");
		velocityYHash = Animator.StringToHash("vy");
	}

	// Update is called once per frame
	void Update()
	{
		AnimateStateMove(Time.deltaTime);
		AnimateProgressiveMove(Time.deltaTime);
	}

	void AnimateProgressiveMove(float time)
	{
		bool runPressed = Input.GetKey("left shift");
		bool forwardPressed = Input.GetKey("w");
		bool backPressed = Input.GetKey("s");
		bool leftPressed = Input.GetKey("a");
		bool rightPressed = Input.GetKey("d");

		const float walkLimit = 1;
		const float runLimit = 2;
		const float walkMultiplier = 1;
		const float runMultiplier = 4;

		// Aumenta o disminuye progresivamente segun si avanza o no, hasta retenerse
		float speedIncrement = time * walkMultiplier;
		
		if (leftPressed || rightPressed)
		{
			if (leftPressed)
				speed2D.x -= speedIncrement;
			if (rightPressed)
				speed2D.x += speedIncrement;
		}
		else
		{
			speed2D.x += speed2D.x > 0 ? -speedIncrement : speedIncrement;
		}

		if (forwardPressed || backPressed)
		{
			if (forwardPressed)
				speed2D.y += speedIncrement;
			if (backPressed)
				speed2D.y -= speedIncrement;
		}
		else
		{
			speed2D.y += speed2D.y > 0 ? -speedIncrement : speedIncrement;
		}

		// Se limita al maximo
		speed2D.x = Mathf.Clamp(speed2D.x, -walkLimit, walkLimit);
		speed2D.y = Mathf.Clamp(speed2D.y, -walkLimit, walkLimit);



		animator.SetFloat(velocityXHash, speed2D.x);
		animator.SetFloat(velocityYHash, speed2D.y);
	}

	void AnimateStateMove(float time)
	{
		bool forwardPressed = Input.GetKey("w");
		bool runPressed = Input.GetKey("left shift");
		
		animator.SetBool(isWalkingHash, forwardPressed);

		// Solo corre si esta andando a la vez:
		animator.SetBool(isRunningHash, forwardPressed && runPressed);
	}
}

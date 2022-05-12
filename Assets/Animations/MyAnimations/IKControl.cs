using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Serialization;

[RequireComponent(typeof(Animator))]

public class IKControl : MonoBehaviour
{
	private Animator _animator;

	public bool ikIsActive;
	public bool handsInRemote;

	public Transform planeHandle;
	public Transform leftHandRemote;
	public Transform rightHandRemote;
	public Transform lookObj;

	private float animTime = 0;

	void Start()
	{
		_animator = GetComponent<Animator>();
	}

	private void Update()
	{
		var bezierMovable = lookObj.GetComponent<BezierMovable>();
		if (bezierMovable && bezierMovable.Bezier.Move.T < .05f)
		{
			handsInRemote = false;
			animTime = 0;
		}
		else
		{
			handsInRemote = true;
			animTime += Time.deltaTime;
		}
	}

	//a callback for calculating IK
	private void OnAnimatorIK(int layerIndex)
	{
		if (!_animator) return;
		//if the IK is not active, set the position and rotation to the default. 
		if (!ikIsActive)
		{
			_animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
			_animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
			_animator.SetLookAtWeight(0);
		}
		else
		{
			// Set the look target position, if one has been assigned
			if (lookObj)
			{
				_animator.SetLookAtWeight(1, 0.3f, 1, 0);
				_animator.SetLookAtPosition(lookObj.position);
			}

			// Mano derecha en el mando
			if (rightHandRemote)
			{
				_animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
				_animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
				_animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandRemote.position);
				_animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandRemote.rotation);
			}

			// Dos Estados, cuando no tiene el mando cogido lanza el avion, cuando no, coge el mando
			if (handsInRemote)
			{
				// Mano izzquierda en el mando
				if (leftHandRemote)
				{
					_animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, animTime);
					_animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, animTime);
					_animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandRemote.position);
					_animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandRemote.rotation);
				}
			}
			else if (planeHandle)
			{
				// Mano agarrando el avion
				if (leftHandRemote)
				{
					_animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
					_animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
					_animator.SetIKPosition(AvatarIKGoal.LeftHand, planeHandle.position);
					_animator.SetIKRotation(AvatarIKGoal.LeftHand, planeHandle.rotation);
				}
			}
		}
	}
}
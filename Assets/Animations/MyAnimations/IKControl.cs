using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Animator))]

public class IKControl : MonoBehaviour
{

	protected Animator animator;

	public bool IKisActive = false;
	public bool handsInRemote = false;

	public Transform planeHandle = null;
	public Transform LeftHandRemote = null;
	public Transform RightHandRemote = null;
	public Transform lookObj = null;

	void Start()
	{
		animator = GetComponent<Animator>();
	}

	//a callback for calculating IK
	void OnAnimatorIK()
	{
		if (animator)
		{

			//if the IK is active, set the position and rotation directly to the goal. 
			if (IKisActive)
			{
				// Set the look target position, if one has been assigned
				if (lookObj != null)
				{
					animator.SetLookAtWeight(1);
					animator.SetLookAtPosition(lookObj.position);
				}

				// Mano derecha en el mando
				if (RightHandRemote != null)
				{

					animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
					animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
					animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandRemote.position);
					animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandRemote.rotation);
				}

				// Dos Estados, cuando no tiene el mando cogido lanza el avion, cuando no, coge el mando
				if (handsInRemote)
				{

					// Mano izzquierda en el mando
					if (LeftHandRemote != null)
					{
						animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
						animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
						animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandRemote.position);
						animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandRemote.rotation);
					}

				}
				else
				{
					// Mano agarrando el avion
					if (LeftHandRemote != null)
					{
						animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
						animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
						animator.SetIKPosition(AvatarIKGoal.LeftHand, planeHandle.position);
						animator.SetIKRotation(AvatarIKGoal.LeftHand, planeHandle.rotation);
					}
				}

				



			}

			//if the IK is not active, set the position and rotation of the hand and head back to the original position
			else
			{
				animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
				animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
				animator.SetLookAtWeight(0);
			}
		}
	}
}
using Unity.Collections;
using UnityEngine;

public class MyIKControl : MonoBehaviour
{

	protected Animator animator;

	public bool IKisActive = false;
	public bool handsInRemote = false;

	public Transform planeHandle = null;
	public Transform LeftHandRemote = null;
	public Transform RightHandRemote = null;
	public Transform lookObj = null;

	public SkinnedMeshRenderer skinMesh;

	private Transform hand;

	[ExecuteAlways]
	void Update()
	{
		// Search skin of the Bone:
		//hand = skinMesh.bones[8];
		//BoneWeight1[] boneWeight = skinMesh.sharedMesh.GetAllBoneWeights().ToArray();

		// Cada articulacion padre de la mano:
		Transform shoulder = skinMesh.bones[6];
		Transform elbow = skinMesh.bones[7];
		Transform wrist = skinMesh.bones[8];

		shoulder.rotation = planeHandle.rotation;
	}

	void OnDrawGizmos()
	{
		Transform handBone = skinMesh.bones[8];
		Gizmos.color = Color.red;
		Gizmos.DrawLine(planeHandle.position, handBone.position);
	}


}

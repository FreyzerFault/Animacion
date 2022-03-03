using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aircraft : BezierMovable
{
	// Start is called before the first frame update
	void Start()
	{
		transform.position = Bezier.GetBezierPointT(0);
	}

	new void Update()
	{
		base.Update();
	}

	
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fluxy;

[RequireComponent(typeof(FluxyTarget))]
public class OpacityFromVelocity : MonoBehaviour
{

	public AnimationCurve velocityToOpacity = new AnimationCurve();
	FluxyTarget target;

	void Awake()
	{
		target = GetComponent<FluxyTarget>();
	}

    void Update()
    {
		var c = target.color;
		c.a = velocityToOpacity.Evaluate(target.velocity.magnitude);
		target.color = c;
	}
}

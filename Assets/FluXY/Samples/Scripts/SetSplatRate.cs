using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fluxy;

namespace FluxySamples
{
	public class SetSplatRate : MonoBehaviour
	{
		public void SetRate(float value)
		{
			GetComponent<FluxyTarget>().rateOverTime = (int)value;
		}
	}
}

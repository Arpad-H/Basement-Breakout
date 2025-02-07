using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Pinwheel.Poseidon;

/*
	Simple Buoyancy Script for Poseidon water system
	to adapt to any other water system, just edit the GetWaterHeight() method to whatever your water system uses to get the wave/water height of a given point
	the container should sit at the center of the ship for best effect
*/

[ExecuteInEditMode]
public class AdvancedBuoyController : MonoBehaviour {

	private WaterBehaviour water;
	public bool applyRipple;
	
	
	[SerializeField] float front;
	[SerializeField] float back;
	[SerializeField] float sides;
	// [SerializeField] Transform container;
	[SerializeField] Rigidbody rb;
	[SerializeField] float heightOffset = 0f;
	[SerializeField, Range(0f,20f)] float floatDynamic = 10f;
	[SerializeField, Range(0f,20f)] float pitchDynamic = 5f;
	[SerializeField, Range(0f,20f)] float rollDynamic = 2.5f;
	

	void Start() {
		rb = this.GetComponent<Rigidbody>();
		water = FindObjectOfType<WaterBehaviour>();
		if (water == null) {
			Debug.LogError("No WaterBehaviour found in scene");
		}
	}


	public void Update() {
		// if (water.transform.position.y+0.09f < this.transform.position.y)
		// {
		// 	return;
		// }
		float frontHeight = GetWaterHeight(transform.position + transform.forward * front);
		float backHeight = GetWaterHeight(transform.position - transform.forward * back);
		float rightHeight = GetWaterHeight(transform.position + transform.right * sides);
		float leftHeight = GetWaterHeight(transform.position - transform.right * sides);
		
		
		// float pitch = backHeight - frontHeight;
		// float roll = rightHeight - leftHeight;
		
		float minHeight = Mathf.Min(frontHeight, backHeight, rightHeight, leftHeight);
		float lowAvgHeight = (frontHeight + backHeight + rightHeight + leftHeight + minHeight) / 5f;
		lowAvgHeight += heightOffset;
		
		float waterLevel = lowAvgHeight;
		float objectHeight = transform.position.y;
		
		if (objectHeight < waterLevel) // Object is submerged
		{
			float depth = waterLevel - objectHeight;
			Vector3 buoyancyForce = new Vector3(0f, depth * floatDynamic, 0f);
			rb.AddForce(buoyancyForce, ForceMode.Acceleration);
		}

		// Compute pitch and roll
		float pitch = backHeight - frontHeight;
		float roll = rightHeight - leftHeight;

		Vector3 torque = new Vector3(
			pitch * pitchDynamic, 
			0f, 
			roll * rollDynamic
		);

		rb.AddTorque(torque, ForceMode.Acceleration);
	}

	

	float GetWaterHeight(Vector3 pos) {
		Vector3 myPos = water.transform.InverseTransformPoint(pos);
		return water.GetWaveDisplacement(myPos, Time.time).y;
		// return water.transform.TransformPoint(myPos).y;
		// return worldPos.y;
		// return 0;
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(transform.position + transform.forward * front, 0.05f);
		Gizmos.DrawSphere(transform.position - transform.forward * back, 0.05f);
		Gizmos.DrawSphere(transform.position + transform.right * sides, 0.05f);
		Gizmos.DrawSphere(transform.position - transform.right * sides, 0.05f);
	}
}
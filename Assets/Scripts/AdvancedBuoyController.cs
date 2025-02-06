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
	[SerializeField] Transform container;
	[SerializeField] float heightOffset = 0f;
	[SerializeField, Range(0f,20f)] float floatDynamic = 10f;
	[SerializeField, Range(0f,20f)] float pitchDynamic = 5f;
	[SerializeField, Range(0f,20f)] float rollDynamic = 2.5f;

	void Start() {
		water = FindObjectOfType<WaterBehaviour>();
		if (water == null) {
			Debug.LogError("No WaterBehaviour found in scene");
		}
	}


	public void Update() {
		if (water == null) return;

		float frontHeight = GetWaterHeight(transform.position + transform.forward * front);
		float backHeight = GetWaterHeight(transform.position - transform.forward * back);
		float rightHeight = GetWaterHeight(transform.position + transform.right * sides);
		float leftHeight = GetWaterHeight(transform.position - transform.right * sides);
		
		float pitch = backHeight - frontHeight;
		float roll = rightHeight - leftHeight;
		
		float minHeight = Mathf.Min(frontHeight, backHeight, rightHeight, leftHeight);
		float lowAvgHeight = (frontHeight + backHeight + rightHeight + leftHeight + minHeight) / 5f;
		lowAvgHeight += heightOffset;
		
		container.position = Vector3.Lerp(container.position, new Vector3(container.position.x, lowAvgHeight, container.position.z), floatDynamic * Time.deltaTime);

		container.localEulerAngles = new Vector3(
			Mathf.LerpAngle(container.localEulerAngles.x, pitch * pitchDynamic, Time.deltaTime), 
			0f, 
			Mathf.LerpAngle(container.localEulerAngles.z, roll * rollDynamic, Time.deltaTime)
		);
	}

	float GetWaterHeight(Vector3 pos) {
		// Vector3 myPos = water.transform.InverseTransformPoint(pos);
		// myPos = water.GetLocalVertexPosition(myPos, applyRipple);
		Vector3 worldPos = transform.position;
		worldPos.y = water.GetWaveDisplacement(worldPos, Time.time).y;
		// return water.transform.TransformPoint(myPos).y;
		return worldPos.y;
		return 0;
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(transform.position + transform.forward * front, 0.5f);
		Gizmos.DrawSphere(transform.position - transform.forward * back, 0.5f);
		Gizmos.DrawSphere(transform.position + transform.right * sides, 0.5f);
		Gizmos.DrawSphere(transform.position - transform.right * sides, 0.5f);
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class AdvancedBuoyController : MonoBehaviour {

	[SerializeField] bool cannotSpin = false;
	private WaterBehaviour water;
	//public bool applyRipple;
	
	
	/*[SerializeField] float front;
	[SerializeField] float back;
	[SerializeField] float sides;*/
	// [SerializeField] Transform container;
	[SerializeField] Rigidbody rb;
	[SerializeField] float heightOffset = 0f;
	[SerializeField, Range(0f,200f)] float floatDynamic = 10f;
	[SerializeField, Range(0f,20f)] float pitchDynamic = 5f;
	[SerializeField, Range(0f,20f)] float rollDynamic = 2.5f;
	
	[SerializeField] private Transform waterTransform;
	private bool underwater;
	[SerializeField] float underwaterDrag = 3f;
	[SerializeField] float underwaterAngularDrag = 1f;
	[SerializeField] float airDrag = 0f;
	[SerializeField] float airAngularDrag = 0.05f;

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
		/*float frontHeight = GetWaterHeight(transform.position + transform.forward * front);
		float backHeight = GetWaterHeight(transform.position - transform.forward * back);
		float rightHeight = GetWaterHeight(transform.position + transform.right * sides);
		float leftHeight = GetWaterHeight(transform.position - transform.right * sides);*/
		
		
		// float pitch = backHeight - frontHeight;
		// float roll = rightHeight - leftHeight;
		
		/*float minHeight = Mathf.Min(frontHeight, backHeight, rightHeight, leftHeight);
		float lowAvgHeight = (frontHeight + backHeight + rightHeight + leftHeight + minHeight) / 5f;
		lowAvgHeight += heightOffset;
		
		float waterLevel = lowAvgHeight;*/
		
		float objectHeight = transform.position.y;
		//float waterHeight = waterTransform.position.y + heightOffset;
		
		float depth = objectHeight - GetWaterHeight(transform.position) - heightOffset;
		if (depth < 0) // Object is submerged
		{
			if (!cannotSpin)
			{
				rb.AddTorque(new Vector3(Random.Range(-4f, 4f), 0, Random.Range(-4f, 4f)), ForceMode.Acceleration);
			}
			rb.AddForceAtPosition(Vector3.up * floatDynamic * Mathf.Abs(depth), transform.position, ForceMode.Force);
			if (!underwater)
			{
				underwater = true;
				switchState(true);
			}
		}
		else if (underwater)
		{
			underwater = false;
			switchState(false);
		}

		// Compute pitch and roll
		/*float pitch = backHeight - frontHeight;
		float roll = rightHeight - leftHeight;

		Vector3 torque = new Vector3(
			pitch * pitchDynamic, 
			0f, 
			roll * rollDynamic
		);*/
		//rb.AddTorque(torque, ForceMode.Acceleration);
	}

	void switchState(bool isUnderwater)
	{
		if (isUnderwater)
		{
			rb.drag = underwaterDrag;
			rb.angularDrag = underwaterAngularDrag;
		}
		else
		{
			rb.drag = airDrag;
			rb.angularDrag = airAngularDrag;
		}
	}
	

	float GetWaterHeight(Vector3 pos) {
		Vector3 myPos = water.transform.InverseTransformPoint(pos);
		return water.GetWaveDisplacement(myPos, Time.time).y;
		// return water.transform.TransformPoint(myPos).y;
		// return worldPos.y;
		// return 0;
	}

	/*void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(transform.position + transform.forward * front, 0.05f);
		Gizmos.DrawSphere(transform.position - transform.forward * back, 0.05f);
		Gizmos.DrawSphere(transform.position + transform.right * sides, 0.05f);
		Gizmos.DrawSphere(transform.position - transform.right * sides, 0.05f);
	}*/
}
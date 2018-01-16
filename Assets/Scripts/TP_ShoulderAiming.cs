using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TP_ShoulderAiming : MonoBehaviour {
	public Transform aimMarker;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Hack")) 
		{
			GetTargetPoint ();
		}
	}

	Vector3 GetTargetPoint()
	{
		Vector3 aimingPoint = new Vector3(0,0,0);
		RaycastHit hit;

		if (Physics.Raycast (aimMarker.position, Camera.main.transform.position-aimMarker.position, out hit)) 
		{
			aimingPoint = hit.point;
		}
		return aimingPoint;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TP_ShoulderAiming : MonoBehaviour {
	public Transform aimMarker;
	public CinemachineVirtualCamera aimingView;
	public CinemachineFreeLook normalView;

	Vector3[] cameraCorners;
	public float aimMarkerSpeed;



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		/*if (Input.GetAxis ("OrbitHorizontal") > 0.2f) 
		{
			aimMarker.localPosition = new Vector3 (aimMarker.localPosition.x+aimMarkerSpeed, aimMarker.localPosition.y, aimMarker.localPosition.z);
		}

		if (Input.GetAxis ("OrbitHorizontal") < -0.2f) 
		{
			aimMarker.localPosition = new Vector3 (aimMarker.localPosition.x-aimMarkerSpeed, aimMarker.localPosition.y, aimMarker.localPosition.z);
		}

		if (Input.GetAxis ("OrbitVertical") > 0.2f) 
		{
			aimMarker.localPosition = new Vector3 (aimMarker.localPosition.x, aimMarker.localPosition.y-aimMarkerSpeed, aimMarker.localPosition.z);
		}

		if (Input.GetAxis ("OrbitVertical") < -0.2f) 
		{
			aimMarker.localPosition = new Vector3 (aimMarker.localPosition.x, aimMarker.localPosition.y+aimMarkerSpeed, aimMarker.localPosition.z);
		}*/

		if (Input.GetAxis ("OrbitHorizontal") > 0.2f || Input.GetAxis ("OrbitHorizontal") < -0.2f || Input.GetAxis ("OrbitVertical") > 0.2f || Input.GetAxis ("OrbitVertical") < -0.2f) 
		{
			float vChange = -Input.GetAxis("OrbitVertical")*aimMarkerSpeed;
			float hChange = Input.GetAxis("OrbitHorizontal")*aimMarkerSpeed;

			aimMarker.localPosition = new Vector3 (aimMarker.localPosition.x+hChange, aimMarker.localPosition.y+vChange, aimMarker.localPosition.z);
		}


		UpdateCameraClipPoints (transform.position, Camera.main.transform.rotation, cameraCorners);
	}

	public void AimingMode (bool value)
	{
		normalView.enabled = !value;
		aimingView.gameObject.SetActive(value);
	}

	public Vector3 GetTargetPoint()
	{
		Vector3 aimingPoint = new Vector3(0,0,0);
		RaycastHit hit;

		if (Physics.Raycast (aimMarker.position, Camera.main.transform.position-aimMarker.position, out hit)) 
		{
			aimingPoint = hit.point;
		}
		return aimingPoint;
	}

	public void UpdateCameraClipPoints(Vector3 cameraPosition, Quaternion atRotation, Vector3[] intoArray)
	{
		//clear the content of intoArray
		intoArray = new Vector3[4];

		float z = Camera.main.nearClipPlane;
		float x = Mathf.Tan (Camera.main.fieldOfView / 3.41f) * z;
		float y = x / Camera.main.aspect;

		//top left
		intoArray[0] = (atRotation * new Vector3(-x,y,z))+ cameraPosition; //added and rotated the point relative to camera
		//top right
		intoArray[1] = (atRotation * new Vector3(x,y,z))+ cameraPosition;
		//bottom left
		intoArray[2] = (atRotation * new Vector3(-x,-y,z))+ cameraPosition;
		//bottom right
		intoArray[3] = (atRotation * new Vector3(x,-y,z))+ cameraPosition;
	}


}

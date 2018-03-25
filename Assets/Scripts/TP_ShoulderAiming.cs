using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TP_ShoulderAiming : MonoBehaviour {
	public Transform aimMarker;
	public CinemachineVirtualCamera aimingView;
	public GameObject pivot;
	public CinemachineFreeLook normalView;

	public float horizontalSpeed;
	public float verticalSpeed;
	public float vMax;
	public float vMin;

	float originVerticalPivotRotation;
	float vChange;
	float hChange;



	// Use this for initialization
	void Start () {
		originVerticalPivotRotation = aimingView.transform.localRotation.eulerAngles.x;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		AimingHelp ();
		//Aim horizontaly by rotating the player
		if ((Input.GetAxis ("OrbitHorizontal") > 0.2f || Input.GetAxis ("OrbitHorizontal") < -0.2f) && Chara_PlayerController.isAiming)
		{
			hChange += Input.GetAxis("OrbitHorizontal")*horizontalSpeed;

			/*transform.rotation = Quaternion.Euler(new Vector3 (transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y+hChange, transform.rotation.eulerAngles.z));
			hChange = 0f;*/
		}

		//Aim verticaly by rotating the pivot point of the aimMarker
		if (Input.GetAxis ("OrbitVertical") > 0.2f || Input.GetAxis ("OrbitVertical") < -0.2f) 
		{
			if (vChange >= vMin && vChange <= vMax) 
			{
				vChange += Input.GetAxis ("OrbitVertical") * verticalSpeed;

				if (vChange < vMin)
					vChange = vMin;
				
				if (vChange > vMax)
					vChange = vMax;
			}


		}

		transform.rotation = Quaternion.Euler(new Vector3 (transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y+hChange, transform.rotation.eulerAngles.z));
		hChange = 0f;
		pivot.transform.localRotation = Quaternion.Euler (new Vector3 (originVerticalPivotRotation+vChange, pivot.transform.localRotation.eulerAngles.y, pivot.transform.localRotation.eulerAngles.z));
	}

	/// <summary>
	/// Switch between aiming and normal mode
	/// </summary>
	/// <param name="value">true = aiming / false = normal</param>
	public void AimingMode (bool value)
	{
		if (value) {
			StartCoroutine (OrientPlayer ());
		} else {
			//switch view
			normalView.enabled = !value;
			aimingView.gameObject.SetActive(value);

			//reset aiming
			vChange = 0f;
			pivot.transform.localRotation = Quaternion.Euler (new Vector3 (originVerticalPivotRotation+vChange, pivot.transform.localRotation.eulerAngles.y, pivot.transform.localRotation.eulerAngles.z));
		}


	}

	public void AimingHelp()
	{
		RaycastHit hit;
		LayerMask layerMask = (1 << 2);

		Vector3 targetPoint;
		Vector3 hitPoint; 

		if (Physics.Raycast (Camera.main.transform.position, aimMarker.position-Camera.main.transform.position, out hit, Mathf.Infinity, layerMask)) 
		{
			if (hit.collider.tag == "Hackable") 
			{
				targetPoint = hit.transform.position;
				hitPoint = hit.point;

				hChange += AngleDir (hitPoint - Camera.main.transform.position, Camera.main.transform.forward, Vector3.up)*0.5f; 
				Vector3 right = Vector3.Cross (-Vector3.forward, Vector3.up);
				vChange += AngleDir (hitPoint - Camera.main.transform.position, Camera.main.transform.forward, right) * 0.5f;
			}
		}
	}

	/// <summary>
	/// Orients the player towards camera orientation when switching to aiming mode.
	/// </summary>
	/// <returns>Coroutine</returns>
	IEnumerator OrientPlayer()
	{
		Quaternion originRotation = transform.rotation;
		Vector3 camRotation = new Vector3 (originRotation.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y, originRotation.eulerAngles.z);
		float i = 0;
		while (i <= 1) 
		{
			transform.rotation = Quaternion.Slerp (originRotation, Quaternion.Euler(camRotation), i);
			i += 0.1f;
			yield return null;
		}
		normalView.enabled = false;
		aimingView.gameObject.SetActive(true);
		vChange = 0f;
		pivot.transform.localRotation = Quaternion.Euler (new Vector3 (originVerticalPivotRotation+vChange, pivot.transform.localRotation.eulerAngles.y, pivot.transform.localRotation.eulerAngles.z));
	}

	/*public void CheckCameraBoundraries()
	{
		RaycastHit hit;
		Ray topBound = new Ray (cameraCorners[0], cameraCorners[1] - cameraCorners[0]);
		Ray leftBound = new Ray (cameraCorners[0], cameraCorners[2] - cameraCorners[0]);
		Ray bottomBound = new Ray (cameraCorners [2], cameraCorners [3] - cameraCorners [2]);
		Ray rightBound = new Ray (cameraCorners [1], cameraCorners [3] - cameraCorners [1]);

		Debug.DrawRay (cameraCorners[0], cameraCorners[1] - cameraCorners[0], Color.magenta); //top
		Debug.DrawRay (cameraCorners[0], cameraCorners[2] - cameraCorners[0], Color.magenta); //left
		Debug.DrawRay (cameraCorners[2], cameraCorners[3] - cameraCorners[2], Color.magenta); //bottom
		Debug.DrawRay (cameraCorners[1], cameraCorners[3] - cameraCorners[1], Color.magenta); //right

		///////////////////////////////////////////////
		/// //////////////////////////////////////////// beware of parenting
		/// ////////////////////////////////////////////
		/// ///////////////////////////////////////////// 
		if (Physics.Raycast (topBound, out hit)) 
		{
			if (hit.collider.transform == aimMarker) 
			{
				aimingView.transform.localRotation = Quaternion.Euler(new Vector3 (aimingView.transform.localRotation.eulerAngles.x-0.2f, aimingView.transform.localRotation.eulerAngles.y, aimingView.transform.localRotation.eulerAngles.z));
			}
		}

		if (Physics.Raycast (leftBound, out hit)) 
		{
			if (hit.collider.transform == aimMarker) 
			{

			}
		}

		if (Physics.Raycast (bottomBound, out hit)) 
		{
			if (hit.collider.transform == aimMarker) 
			{

			}
		}

		if (Physics.Raycast (rightBound, out hit)) 
		{
			if (hit.collider.transform == aimMarker) 
			{

			}
		}

	}*/

	/// <summary>
	/// Gets the target point (currently aimed).
	/// </summary>
	/// <returns>The target point.</returns>
	public Vector3 GetTargetPoint()
	{
		Vector3 aimingPoint = new Vector3(0,0,0);
		RaycastHit hit;

		LayerMask layerMask = (1 << 2);
		layerMask = ~layerMask;

		if (Physics.Raycast (Camera.main.transform.position, aimMarker.position-Camera.main.transform.position, out hit, Mathf.Infinity, layerMask)) 
		{
			aimingPoint = hit.point;
		}
		Debug.DrawRay (Camera.main.transform.position, aimMarker.position-Camera.main.transform.position, Color.green, 2f);
		return aimingPoint;
	}

	//Returns -1 if target is left and 1 if right (0 if straight forward or backward)
	float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up) 
	{
		Vector3 perp = Vector3.Cross(fwd, targetDir);
		float dir = Vector3.Dot(perp, up);

		if (dir > 0f) {
			return 1f;
		} else if (dir < 0f) {
			return -1f;
		} else {
			return 0f;
		}
	}

	/*public void UpdateCameraCorners(Vector3 cameraPosition, Quaternion atRotation)
	{
		//clear the content of intoArray
		cameraCorners = new Vector3[4];

		float z = aimMarker.localPosition.z;
		float x = Mathf.Tan (Camera.main.fieldOfView / 4f) * z;
		float y = x / Camera.main.aspect;

		//top left
		cameraCorners[0] = (atRotation * new Vector3(-x,y,z))+ cameraPosition; //added and rotated the point relative to camera
		//top right
		cameraCorners[1] = (atRotation * new Vector3(x,y,z))+ cameraPosition;
		//bottom left
		cameraCorners[2] = (atRotation * new Vector3(-x,-y,z))+ cameraPosition;
		//bottom right
		cameraCorners[3] = (atRotation * new Vector3(x,-y,z))+ cameraPosition;
	}*/


}

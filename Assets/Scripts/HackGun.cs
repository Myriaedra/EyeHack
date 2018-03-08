using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackGun : MonoBehaviour 
{
	TP_ShoulderAiming aimingSys;
	public GameObject sphere;
	Camera playerView;

	public GameObject currentTarget;
	GameObject currentQuarrel;
	bool shot;


	// Use this for initialization
	void Start () 
	{
		aimingSys = transform.GetComponent<TP_ShoulderAiming> ();
		playerView = Camera.main;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetAxisRaw ("Aim") > 0.8 && !PlayerController.isAiming) 
		{
			PlayerController.isAiming = true;
			aimingSys.AimingMode (true);
		} 
		else if (Input.GetAxisRaw ("Aim") < 0.2 && PlayerController.isAiming) 
		{
			PlayerController.isAiming = false;
			aimingSys.AimingMode (false);
		}


		if (Input.GetAxisRaw ("Hack") > 0.4 && !shot) 
		{
			RaycastHit hit;
			Vector3 targetPoint = aimingSys.GetTargetPoint ();

			if (Physics.Raycast (transform.position, targetPoint - transform.position, out hit)) 
			{
				if (currentQuarrel != null)
					Destroy (currentQuarrel);
				currentQuarrel = Instantiate (sphere, hit.point, Quaternion.identity);
				currentQuarrel.transform.parent = hit.collider.gameObject.transform;
				currentQuarrel.layer = 1;

				if (hit.collider.gameObject.tag == "Hackable") 
				{
					currentTarget = hit.collider.gameObject;
				}
				else 
				{
					currentTarget = null;
				}
			}
			shot = true;
		} 
		else if (Input.GetAxisRaw ("Hack") < 0.8) 
		{
			shot = false;
		}


		if (Input.GetButtonDown ("Interaction") && currentTarget != null) 
		{
			if (currentTarget.GetComponent<Viewable>() != null) 
			{
				ViewInterpolation(currentTarget.GetComponent<Viewable>().GetView ());

				if (PlayerController.isAiming) 
				{
					aimingSys.AimingMode (false);
				}
			}
		}
	}

	void ViewInterpolation(Camera enemyView)
	{
		PlayerController.controlsAble = !PlayerController.controlsAble;
		playerView.enabled = !playerView.enabled;
		enemyView.enabled = !enemyView.enabled;
	}	
		
}

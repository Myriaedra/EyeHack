using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackGun : MonoBehaviour 
{
	TP_ShoulderAiming aimingSys;
	public GameObject sphere;
	Camera playerView;

	public GameObject currentTarget;
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


		if (Input.GetAxisRaw ("Hack") > 0.8 && !shot) 
		{
			RaycastHit hit;
			Vector3 targetPoint = aimingSys.GetTargetPoint ();

			if (Physics.Raycast (transform.position, targetPoint - transform.position, out hit)) 
			{
				GameObject quarrel = Instantiate (sphere, hit.point, Quaternion.identity);
				quarrel.transform.parent = hit.collider.gameObject.transform;

				if (hit.collider.gameObject.tag == "Hackable") 
				{
					currentTarget = hit.collider.gameObject;
				}
			}
			shot = true;
		} 
		else if (Input.GetAxisRaw ("Hack") < 0.8) 
		{
			shot = false;
		}


		if (Input.GetButtonDown ("Interaction")) 
		{
			if (currentTarget.GetComponent<Viewable>() != null) 
			{
				ViewInterpolation (currentTarget.GetComponent<Viewable>().GetView ());
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

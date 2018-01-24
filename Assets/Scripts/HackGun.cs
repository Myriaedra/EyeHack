using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackGun : MonoBehaviour {
	TP_ShoulderAiming aimingSys;


	// Use this for initialization
	void Start () {
		aimingSys = transform.GetComponent<TP_ShoulderAiming> ();
	}
	
	// Update is called once per frame
	void Update () {
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

		if (Input.GetButtonDown("Hack"))
		{
			RaycastHit hit;

			if (Physics.Raycast(transform.position, Vector3.forward, out hit))
				print("Found an object - distance: " + hit.collider.gameObject.name);	
		}
	}
}

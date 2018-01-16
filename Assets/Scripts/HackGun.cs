using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackGun : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Hack"))
		{
			RaycastHit hit;

			if (Physics.Raycast(transform.position, Vector3.forward, out hit))
				print("Found an object - distance: " + hit.collider.gameObject.name);	
		}
	}
}

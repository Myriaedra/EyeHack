using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_Trap : Hackable 
{
	public float openTimer;
	public Transform openTarget;


	// Use this for initialization
	void Start () {
		
	}
	
	public override void Switch()
	{
		if (!isActivated) 
		{
			isActivated = true;
			OnActivationEffect ();
		}
	}

	public override void OnActivationEffect()
	{
		StartCoroutine (ActivatedTrap());
	}

	IEnumerator ActivatedTrap()
	{
		Vector3 targetPosition = openTarget.position;
		Vector3 startPosition = transform.position;

		for (float i = 0; i <= 1f; i += Time.deltaTime*2) 
		{
			transform.position = Vector3.Lerp (startPosition, targetPosition, i);
			yield return null;
		}

		yield return new WaitForSeconds(openTimer);

		for (float i = 0; i <= 1f; i += Time.deltaTime*2) 
		{
			transform.position = Vector3.Lerp (targetPosition, startPosition, i);
			yield return null;
		}

		isActivated = false;
	}
}
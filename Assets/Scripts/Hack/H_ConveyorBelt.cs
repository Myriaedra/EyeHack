using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H_ConveyorBelt : Hackable {
	public bool goingForward;
	public bool alternateSens;
	public Transform forwardTarget;
	public float speed;

	Vector3 forwardDirection;
	public List<Character> currentTargets;
	// Use this for initialization
	void Start () 
	{
		forwardDirection = (forwardTarget.position - transform.position).normalized; 
	}
	
	protected override void Update()
	{
		base.Update ();
	}

	public override void OnActivationEffect()
	{
		if (alternateSens) 
		{
			goingForward = !goingForward;
		}
	}

	public override void ConstantActivatedEffect ()
	{
		if (isActivated && currentTargets.Count > 0) 
		{
			for (int i = 0; i < currentTargets.Count; i++) 
			{
				if (goingForward)
					currentTargets [i].GetComponent<Rigidbody> ().AddForce(forwardDirection * speed, ForceMode.VelocityChange);
				else
					currentTargets [i].GetComponent<Rigidbody> ().AddForce(-forwardDirection * speed, ForceMode.VelocityChange);
			}
		}
	}

	/*void OnCollisionEnter (Collision other)
	{
		if (other.gameObject.GetComponent<Character> () != null) 
		{
			currentTargets.Add (other.other.gameObject.GetComponent<Character> ());
		}
	}

	void OnCollisionExit (Collision other)
	{
		if (other.gameObject.GetComponent<Character> () != null) 
		{
			currentTargets.Remove (other.other.gameObject.GetComponent<Character> ());
		}
	}*/
	void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.GetComponent<Character> () != null) 
		{
			currentTargets.Add (other.gameObject.GetComponent<Character> ());
		}
	}

	void OnTriggerExit (Collider other)
	{
		if (other.gameObject.GetComponent<Character> () != null) 
		{
			currentTargets.Remove (other.gameObject.GetComponent<Character> ());
		}
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hackable : MonoBehaviour {
	public bool isActivated;
	// Use this for initialization
	void Update()
	{
		if (isActivated) {ConstantActivatedEffect ();} 
		else {ConstantDeactivatedEffect ();}
	}

	public virtual void Switch()
	{
		isActivated = !isActivated;

		if (isActivated) {OnActivationEffect ();} 
		else {OnDeactivationEffect ();}
	}

	public virtual void OnActivationEffect(){}

	public virtual void OnDeactivationEffect(){}

	public virtual void ConstantActivatedEffect(){}

	public virtual void ConstantDeactivatedEffect(){}
}

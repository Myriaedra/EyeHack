using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackSwitch : MonoBehaviour {

	public Hackable[] targets;

	public void SwitchTargets()
	{
		for (int i = 0; i < targets.Length; i++) 
		{
			targets [i].Switch ();
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HackSwitch : MonoBehaviour {

	public Hackable[] targets;
    public Image icon;
    public Sprite[] icons;
    bool init;

    void Update()
    {
        if (!init)
        {
            if (targets.Length > 0)
                icon.sprite = icons[targets[0].icon];
            init = true;
        }
    }

	public void SwitchTargets()
	{
		for (int i = 0; i < targets.Length; i++) 
		{
			targets [i].Switch ();
		}
	}
}

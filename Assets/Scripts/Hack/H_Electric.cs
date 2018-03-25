using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class H_Electric : Hackable {
	public float timeON;
	public float timeOFF;
	public bool ac;
	public bool isDeadly;

	public List<Character> currentTargets;

	// Use this for initialization
	void Start () {
		if (ac) 
		{
			StartCoroutine (ElectricCycle ());
		}
	}

	public override void OnActivationEffect()
	{
		if (!ac) 
		{
			isDeadly = true;
		}
	}

	public override void OnDeactivationEffect()
	{
		if (!ac) 
		{
			isDeadly = false;
		}
	}

	public override void ConstantActivatedEffect()
	{
		if (isDeadly && currentTargets.Count > 0) 
		{
			for (int i = 0; i < currentTargets.Count; i++) 
			{
				currentTargets [i].DeathCall ();
			}
		}
	}

	void OnCollisionEnter (Collision other)
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
	}

	IEnumerator ElectricCycle()
	{
		isDeadly = true;
		yield return new WaitForSeconds (timeON);

		isDeadly = false;
		yield return new WaitForSeconds (timeOFF);

		StartCoroutine (ElectricCycle ());
	}
}

[CustomEditor(typeof(H_Electric))]
public class H_ElectricEditor : Editor
{
	override public void OnInspectorGUI()
	{
		H_Electric electric = (H_Electric)target;

		electric.isActivated = EditorGUILayout.Toggle("Activated ?", electric.isActivated);

		electric.ac = EditorGUILayout.Toggle("Alternate currant", electric.ac);
		EditorGUILayout.Space ();

		using (var group = new EditorGUILayout.FadeGroupScope (Convert.ToSingle (electric.ac))) 
		{
			if (group.visible != false) 
			{
				EditorGUILayout.LabelField ("Looping Movement Paramaters", EditorStyles.boldLabel);
				electric.timeON = EditorGUILayout.FloatField ("Time ON (seconds) :", electric.timeON);
				electric.timeOFF = EditorGUILayout.FloatField ("Time OFF (seconds) :", electric.timeOFF);
				EditorGUILayout.Space ();
			}
		}
	}
}
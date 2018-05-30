using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class H_Bumper : Hackable {
	public float force;
	public float maxForce;
	public Transform directionTarget;
	Vector3 direction = new Vector3 (0, 0, 0);
	BoxCollider bumperCollider;

	void Start()
	{
        icon = 3;
        direction = directionTarget.position - transform.position;
		direction = direction.normalized;
		bumperCollider = GetComponent<BoxCollider> ();

		if (isActivated)
			bumperCollider.isTrigger = true;
		else
			bumperCollider.isTrigger = false;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player" && isActivated) 
		{
			Rigidbody rB = other.gameObject.GetComponent<Rigidbody> ();
			float fallEnergy = -rB.velocity.y;
			rB.velocity = new Vector3 (rB.velocity.x, 0, rB.velocity.z);
			rB.AddForce (Vector3.ClampMagnitude(direction * fallEnergy * force, maxForce));
            Chara_PlayerController.bumped = true; 
		}
	}

	public override void OnActivationEffect ()
	{
		bumperCollider.isTrigger = true;
	}

	public override void OnDeactivationEffect ()
	{
		bumperCollider.isTrigger = false;
	}
}

[CustomEditor(typeof(H_Bumper))]
public class H_BumperEditor : Editor
{
	override public void OnInspectorGUI()
	{
		H_Bumper bumper = (H_Bumper)target;
		bumper.isActivated = EditorGUILayout.Toggle("Activated ?", bumper.isActivated);
		EditorGUILayout.LabelField ("Parameter", EditorStyles.boldLabel);
		bumper.force = EditorGUILayout.Slider ("Rebound multiplier :", bumper.force, 60f, 200f);
		bumper.maxForce = EditorGUILayout.Slider ("Maximum rebound force :", bumper.maxForce, 1000f, 10000f);
		bumper.directionTarget = EditorGUILayout.ObjectField ("Direction Target :", bumper.directionTarget, typeof(Transform), true) as Transform;
		if (GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
	}
}
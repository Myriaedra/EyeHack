using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using System;

public class H_MovingElement : Hackable {

	[SerializeField]
	public bool loopMovement;
	public bool isPTF;

	public Transform pointA;
	public Transform pointB;

	public float speedAB;
	public float speedBA;
	public float waitOnA;
	public float waitOnB;

	Coroutine currentCoroutine;
	bool currentPointIsA;

	// Use this for initialization
	void Start () {
        if (!isPTF)
            icon = 4;
        else
            icon = 5;

		//If the movement does not loop, init position according to it's activation state
		if (!loopMovement) 
		{
			currentPointIsA = !isActivated;

			if (isActivated)
				transform.position = pointB.position;
			else
				transform.position = pointA.position;
		} 
		else
		{
			if (isActivated) 
			{
				currentCoroutine = StartCoroutine (MoveToPointLoop ());
			}
		}
	}

	public override void OnActivationEffect()
	{
		if (!loopMovement) 
		{
			if (currentCoroutine != null)
				StopCoroutine (currentCoroutine);

			currentCoroutine = StartCoroutine (MoveToPointSimple ());
		} 
		else
		{
			currentCoroutine = StartCoroutine (MoveToPointLoop ());
		}

	}

	public override void OnDeactivationEffect()
	{
		if (!loopMovement) {
			if (currentCoroutine != null)
				StopCoroutine (currentCoroutine);

			currentCoroutine = StartCoroutine (MoveToPointSimple ());
		} else {
			StopCoroutine (currentCoroutine);
		}
	}

	public override void ConstantActivatedEffect(){}

	public override void ConstantDeactivatedEffect(){}

	IEnumerator MoveToPointSimple ()
	{
		Vector3 targetPosition = new Vector3 ();
		Vector3 startPosition = transform.position;
		float speed = 0f;
		if (currentPointIsA) 
		{
			targetPosition = pointB.position;
			speed = speedAB;
		}
		else 
		{
			targetPosition = pointA.position;
			speed = speedBA;
		}
		float dist = Vector3.Distance (transform.position, targetPosition);

		currentPointIsA = !currentPointIsA;

		for (float i = 0; i <= 1f; i += speed / dist * Time.deltaTime) 
		{
			transform.position = Vector3.Lerp (startPosition, targetPosition, i);
			yield return null;
		}

		transform.position = targetPosition;

	}

	IEnumerator MoveToPointLoop ()
	{
		Vector3 targetPosition = new Vector3 ();
		Vector3 startPosition = transform.position;
		float speed = 0f;
		if (currentPointIsA) 
		{
			targetPosition = pointB.position;
			speed = speedAB;
		}
		else 
		{
			targetPosition = pointA.position;
			speed = speedBA;
		}
		float dist = Vector3.Distance (transform.position, targetPosition);

		for (float i = 0; i <= 1f; i += speed / dist * Time.deltaTime) 
		{
			transform.position = Vector3.Lerp (startPosition, targetPosition, i);
			yield return null;
		}

		transform.position = targetPosition;

		if (currentPointIsA) 
		{
			yield return new WaitForSeconds (waitOnB);
			currentPointIsA = !currentPointIsA;
			currentCoroutine = StartCoroutine (MoveToPointLoop ());
		}
		else 
		{
			yield return new WaitForSeconds (waitOnA);
			currentPointIsA = !currentPointIsA;
			currentCoroutine = StartCoroutine (MoveToPointLoop ());
		}
	}

	void OnCollisionEnter (Collision other)
	{
		if (other.gameObject.tag == "Player" && other.transform.position.y > transform.position.y && isPTF) 
		{
			other.transform.parent = transform;
		}
	}

	void OnCollisionExit (Collision other)
	{
		if (other.gameObject.tag == "Player" && isPTF) 
		{
			other.transform.parent = null;
		}
	}
}

//[CustomEditor(typeof(H_MovingElement))]
//public class H_MovingElementEditor : Editor
//{
//	override public void OnInspectorGUI()
//	{
//		H_MovingElement movingElement = (H_MovingElement)target;
//		movingElement.isActivated = EditorGUILayout.Toggle("Activated ?", movingElement.isActivated);
//		EditorGUILayout.LabelField ("Behaviour type", EditorStyles.boldLabel);
//		movingElement.loopMovement = EditorGUILayout.Toggle("Loop Movement", movingElement.loopMovement);
//		movingElement.isPTF = EditorGUILayout.Toggle("Is a Platform", movingElement.isPTF);
//		EditorGUILayout.Space ();

//		EditorGUILayout.LabelField ("Movement Paramaters", EditorStyles.boldLabel);
//		/*movingElement.speedAB = EditorGUILayout.FloatField ("Speed from A to B :", movingElement.speedAB);
//		movingElement.speedBA = EditorGUILayout.FloatField ("Speed from B to A :", movingElement.speedBA);*/
//		movingElement.speedAB = EditorGUILayout.Slider ("Speed from A to B :", movingElement.speedAB, 0.1f, 10f);
//		movingElement.speedBA = EditorGUILayout.Slider ("Speed from B to A :", movingElement.speedBA, 0.1f, 10f);
//		EditorGUILayout.Space ();

//		using (var group = new EditorGUILayout.FadeGroupScope (Convert.ToSingle (movingElement.loopMovement))) 
//		{
//			if (group.visible != false) 
//			{
//				EditorGUILayout.LabelField ("Looping Movement Paramaters", EditorStyles.boldLabel);
//				movingElement.waitOnA = EditorGUILayout.FloatField ("Wait on A (seconds) :", movingElement.waitOnA);
//				movingElement.waitOnB = EditorGUILayout.FloatField ("Wait on B (seconds) :", movingElement.waitOnB);
//				EditorGUILayout.Space ();
//			}
//		}

//		EditorGUILayout.LabelField ("Target Points", EditorStyles.boldLabel);
//		movingElement.pointA = EditorGUILayout.ObjectField ("Target Point A :", movingElement.pointA, typeof(Transform), true) as Transform;
//		movingElement.pointB = EditorGUILayout.ObjectField ("Target Point B :", movingElement.pointB, typeof(Transform), true) as Transform;


//		if (GUI.changed)
//		{
//			EditorUtility.SetDirty(target);
//		}
//	}
//}

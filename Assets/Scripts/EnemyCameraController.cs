using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyCameraState {
	watching,
	alarm
}

public class EnemyCameraController : Character {

	public Collider player;
	public Hackable target;

	public EnemyCameraState state = EnemyCameraState.watching;

	private Vector3 angle;
	private float angleDifference;

	// Use this for initialization
	void Start () {
		player = GameObject.Find ("Player").GetComponent<Collider>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		angle = (transform.position - player.transform.position).normalized;
		angleDifference = Vector3.Angle (-transform.forward, angle);

		switch (state)
		{
		case EnemyCameraState.watching:
			CheckForPlayer ();
			if (target.isActivated) {
				target.Switch ();
			}
			break;

		case EnemyCameraState.alarm:
			AlarmBehaviour ();
			if (!target.isActivated) {
				target.Switch ();
			}
			break;
		}
	}

	void CheckForPlayer()
	{
		RaycastHit hit;
		if (angleDifference < 30 && Physics.Linecast (transform.position, player.transform.position, out hit))
		{
			if (hit.collider == player)
			{
				StartAlarm ();
			}
		}
	}

	void AlarmBehaviour()
	{
		if (angleDifference >= 30)
		{
			StartLooking ();
		}
	}

	void StartLooking()
	{
		target.Switch ();
		state = EnemyCameraState.watching;
	}

	void StartAlarm()
	{
		target.Switch ();
		state = EnemyCameraState.alarm;
	}
}

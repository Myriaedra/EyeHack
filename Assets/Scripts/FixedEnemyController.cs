using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FixedEnemyState {
	looking,
	alarm
}

public enum turnDirection {
	right,
	left,
	nothing
}

public class FixedEnemyController : Character {

	bool canSwitch = true;

	public Collider player;
	public FixedEnemyState state = FixedEnemyState.looking;
	public turnDirection direction = turnDirection.right;

	public EnemyController securityRobot;
	public GameObject callingPoint;

	private Vector3 angle;
	private float angleDifference;
	public float turnSpeed = 1;
	private RaycastHit hit;
	[Range(0, 360)]
	public float maxTurnAngle;
	float currentAngle;
	float initialAngle;
	public float waitTime = 1;

	// Use this for initialization
	void Start () {
		initialAngle = transform.rotation.eulerAngles.y;
	}
	
	// Update is called once per frame
	void Update () {
		angle = (transform.position - player.transform.position).normalized;
		angleDifference = Vector3.Angle (-transform.forward, angle);

		switch (state) {
		case FixedEnemyState.looking:
			CheckForPlayer ();
			break;

		case FixedEnemyState.alarm:
			AlarmBehaviour ();
			break;
		}
	}

	void FixedUpdate() {
		switch (state) {
		case FixedEnemyState.looking:
			TurnHead ();
			break;

		case FixedEnemyState.alarm:
			StareAtPlayer ();
			break;
		}
	}

	#region Rotation functions
	void TurnHead() {
		currentAngle = transform.rotation.eulerAngles.y - initialAngle;

//		transform.rotation = Quaternion.AngleAxis (turnSpeed * direction, transform.up);

		switch (direction) {
		case turnDirection.left:
			transform.rotation = Quaternion.AngleAxis (currentAngle - turnSpeed, transform.up);
			break;

		case turnDirection.right:
			transform.rotation = Quaternion.AngleAxis (currentAngle + turnSpeed, transform.up);
			break;
		}
		if (maxTurnAngle != 360) {
			CheckAngle ();
		}
	}

	void CheckAngle() {
		if (direction != turnDirection.nothing && currentAngle >= maxTurnAngle/2 && currentAngle <= 360-maxTurnAngle/2 && canSwitch) {
//		if ((direction == turnDirection.left && currentAngle <= 360-maxTurnAngle / 2 && currentAngle > maxTurnAngle/2) || (direction == turnDirection.right && currentAngle >= maxTurnAngle / 2 && currentAngle <=360-maxTurnAngle/2)) {
			StartCoroutine(SwitchDirection ());
		}
	}

	IEnumerator SwitchDirection() {
		canSwitch = false;
		print ("Je suis ici");
		direction = turnDirection.nothing;
		yield return new WaitForSeconds (waitTime);
		if (currentAngle > 180) {
			direction = turnDirection.right;
		} else {
			direction = turnDirection.left;
		}
		yield return new WaitForSeconds (0.1f);
		canSwitch = true;
	}
	#endregion

	void StareAtPlayer() {
		transform.rotation = Quaternion.FromToRotation (transform.forward, angle);
	}

	void CheckForPlayer()
	{
		if (angleDifference < 30 && Physics.Linecast (transform.position, player.transform.position, out hit))
		{
			if (hit.collider == player)
			{
				StartAlarm ();
			}
		}
	}

	void StartAlarm() {
		state = FixedEnemyState.alarm;
		RaycastHit alarmHit;
		if (Physics.Linecast (transform.position, player.transform.position, out alarmHit)) {
			Instantiate (callingPoint, alarmHit.transform.position, Quaternion.identity);
			securityRobot.Called (alarmHit.transform.position);
		}
	}

	void StartLooking() {
		state = FixedEnemyState.looking;
	}

	void AlarmBehaviour() {
		if (angleDifference >= 30)
		{
			StartLooking ();
		}
	}

}

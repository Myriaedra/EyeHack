using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

enum EnemyState {
	patrol,
	chase,
	search
}

public class EnemyController : MonoBehaviour {

	EnemyState state = EnemyState.patrol;
	public Transform[] patrolPoints;
	private NavMeshAgent agent;
	private int nextDestination;
	private Plane[] cameraPlanes;
	public Camera view;
	public Collider player;
	Vector3 angle;
	float angleDifference;
	RaycastHit hit;

	float searchTimer;

	// Use this for initialization
	void Start ()
	{
		player = GameObject.Find ("Player").GetComponent<Collider>();
		view = GetComponentInChildren<Camera> ();
		cameraPlanes = GeometryUtility.CalculateFrustumPlanes (view);
		agent = GetComponent<NavMeshAgent> ();
//		agent.Stop ();
		StartPatrol();
	}
	
	// Update is called once per frame
	void Update ()
	{
		print (view);
		Debug.DrawRay (transform.position, transform.forward, Color.red, 1f);
		angle = (transform.position - player.transform.position).normalized;
		angleDifference = Vector3.Angle (-transform.forward, angle);

		switch (state) {
		case EnemyState.chase:
			if (angleDifference >= 30) {
				state = EnemyState.search;
				searchTimer = 1f;
			} else if (Vector3.Distance (transform.position, player.transform.position) < 1) {
				Attack ();
			}
			break;

		case EnemyState.patrol:
			CheckForPlayer ();
			break;

		case EnemyState.search:
			CheckForPlayer ();
			WaitBeforePatrol ();
			break;
		}

	}

	void OnTriggerEnter (Collider other)
	{
		if (state == EnemyState.patrol) {
			if (other.tag == "PatrolPoint") {
				SwitchDestination ();
			}
		}
	}

	void Attack () {
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}

	void WaitBeforePatrol () {
		searchTimer -= Time.deltaTime;
		print (searchTimer);
		if (searchTimer <= 0) {
			state = EnemyState.patrol;
			StartPatrol ();
		}
	}

	void CheckForPlayer() {
		if (angleDifference < 30 && Physics.Linecast (transform.position, player.transform.position, out hit)) {
			if (hit.collider == player) {
				state = EnemyState.chase;
				StartCoroutine ("GetPlayerPosition");
			}
		}
	}

	IEnumerator GetPlayerPosition () {
		if (state == EnemyState.chase) {
			agent.destination = player.transform.position;
			yield return new WaitForSeconds (.2f);
			StartCoroutine ("GetPlayerPosition");
		}
	}

	void StartPatrol() {
		if (patrolPoints.Length > 0)
		{
			agent.destination = patrolPoints [0].position;
			nextDestination = 1;
		}
	}

	void SwitchDestination()
	{
		print ("Reached destination");
		agent.destination = patrolPoints [nextDestination].position;
		if (nextDestination < patrolPoints.Length-1)
		{
			nextDestination++;
		}
		else
		{
			nextDestination = 0;
		}
	}
}

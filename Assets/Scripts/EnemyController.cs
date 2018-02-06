using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

	// Use this for initialization
	void Start ()
	{
		player = GameObject.Find ("Player").GetComponent<Collider>();
		view = GetComponentInChildren<Camera> ();
		cameraPlanes = GeometryUtility.CalculateFrustumPlanes (view);
		agent = GetComponent<NavMeshAgent> ();
		if (patrolPoints.Length > 0)
		{
			agent.destination = patrolPoints [0].position;
			nextDestination = 1;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		print (view);
		if (GeometryUtility.TestPlanesAABB (cameraPlanes, player.bounds)) 
		{
			print ("Seen you");
		}
		else
		{
			print ("Unseen");
		}
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "PatrolPoint")
		{
			SwitchDestination ();
		}
	}

	void CheckForPlayer() {

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

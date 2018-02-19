using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public enum EnemyState {
	patrol,
	chase,
	search
}

public class EnemyController : MonoBehaviour {

	public EnemyState state = EnemyState.patrol;
	public Transform[] patrolPoints;
	public Transform[] searchPatrolPoints;
	public LevelManager level;

	private NavMeshAgent agent;
	private int destination;
	private Plane[] cameraPlanes;
	public Camera view;
	public Collider player;
	Vector3 angle;
	float angleDifference;
	RaycastHit hit;


	// Use this for initialization
	void Start ()
	{
		print (searchPatrolPoints.Length);
		level = GameObject.Find ("LevelManager").GetComponent<LevelManager> ();
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
		Debug.DrawRay (transform.position, transform.forward, Color.red, 1f);
		angle = (transform.position - player.transform.position).normalized;
		angleDifference = Vector3.Angle (-transform.forward, angle);

		switch (state)
		{
		case EnemyState.chase:
			ChaseBehavior ();
			break;

		case EnemyState.patrol:
			CheckForPlayer ();
			break;

		case EnemyState.search:
			SearchPatrol ();
			CheckForPlayer ();
			break;
		}

	}
		
	void ChaseBehavior()
	{
		if (angleDifference >= 30)
		{
			StartSearch ();
		}
		else if (Vector3.Distance (transform.position, player.transform.position) < 1)
		{
			Attack ();
		}
	}

	void SearchPatrol()
	{
		
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "PatrolPoint" && other.transform == patrolPoints[destination])
		{
			if (state == EnemyState.patrol)
			{
				SwitchDestination (patrolPoints);
			}
			else if (state == EnemyState.search)
			{
				SwitchDestination (searchPatrolPoints);
				CheckEndSearch (other);
			}
		}
	}
		
	void Attack ()
	{
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}

	void CheckEndSearch (Collider _other)
	{
		if (_other.transform == searchPatrolPoints[3])
		{
			StartPatrol ();
		}
	}
		
	void CheckForPlayer()
	{
		if (angleDifference < 30 && Physics.Linecast (transform.position, player.transform.position, out hit)) {
			if (hit.collider == player)
			{
				StartChase ();
			}
		}
	}

	IEnumerator GetPlayerPosition ()
	{
		if (state == EnemyState.chase)
		{
			agent.destination = player.transform.position;
			yield return new WaitForSeconds (.2f);
			StartCoroutine ("GetPlayerPosition");
		}
	}

	void StartSearch()
	{
		state = EnemyState.search;
		agent.speed = 2f;
		GetSearchPoints ();
		destination = 0;
		agent.destination = searchPatrolPoints [0].position;
	}

	void StartChase()
	{
		state = EnemyState.chase;
		agent.speed = 7f;
		StartCoroutine ("GetPlayerPosition");
	}
		
	void StartPatrol()
	{
		state = EnemyState.patrol;
		agent.speed = 3.5f;
		if (patrolPoints.Length > 0)
		{
			agent.destination = patrolPoints [0].position;
			destination = 0;
		}
	}
		
	void GetSearchPoints()
	{
		searchPatrolPoints = new Transform[] {level.searchPoints [0], level.searchPoints [0], level.searchPoints [0], level.searchPoints [0]};
		print (searchPatrolPoints.Length);
		for (int i = 0; i < 3; i++)
		{
			searchPatrolPoints[i] = level.searchPoints [0];
			for (int x = 0; x < level.searchPoints.Length; x++)
			{
				if (Vector3.Distance (transform.position, level.searchPoints[x].position) < Vector3.Distance (transform.position, searchPatrolPoints[i].position))
				{
					searchPatrolPoints [i] = level.searchPoints [x];
				}
			}
		}

	}
		
	void SwitchDestination(Transform[] _patrolPoints)
	{
		if (destination < _patrolPoints.Length-1)
		{
			destination++;
		}
		else
		{
			destination = 0;
		}

		agent.destination = _patrolPoints [destination].position;
	}
}

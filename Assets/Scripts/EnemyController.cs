using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public enum EnemyState {
	patrol,
	chase,
	search,
	called
}

public class EnemyController : Character {


	public LevelManager level;
	public Camera view;
	public Collider player;

	public EnemyState state = EnemyState.patrol;

	private NavMeshAgent agent;
	public int destination;
	public Transform[] patrolPoints;
	public Transform[] searchPatrolPoints;

	private Plane[] cameraPlanes;
	private Vector3 angle;
	private float angleDifference;
	private RaycastHit hit;
	private float heardTimer;

	private bool isLookingAround;
	private float lookingAroundTimer;
	private int pauseIndex;

	public bool activated = true;

    public LayerMask groundLayer;

	// Use this for initialization
	void Start ()
	{
		level = GameObject.Find ("LevelManager").GetComponent<LevelManager> ();
		player = GameObject.Find ("Player").GetComponent<Collider>();
		view = GetComponentInChildren<Camera> ();
		cameraPlanes = GeometryUtility.CalculateFrustumPlanes (view);
		agent = GetComponent<NavMeshAgent> ();
		if (activated) {
			StartPatrol ();
		}
	}

    void Update()
    {
        if (activated) {
            //			Debug.DrawRay (transform.position, transform.forward, Color.red, 1f);
            angle = (transform.position - player.transform.position).normalized;
            angleDifference = Vector3.Angle(-transform.forward, angle);

            switch (state) {

                case EnemyState.chase:
                    ChaseBehavior();
                    break;

                case EnemyState.patrol:
                    CheckForPlayer();
                    break;

                case EnemyState.search:
                    CheckForPlayer();
                    if (isLookingAround) {
                        LookAround();
                    }
                    break;

            }

            if (heardTimer > 0) {
                heardTimer -= Time.deltaTime;
            }
        }

        if (!Physics.SphereCast(transform.position, 0.5f, -transform.up, out hit, 1.2f, groundLayer))
        {
            agent.enabled = false;
        }
    }
		
	void ChaseBehavior()
	{
		if (angleDifference >= 30  && heardTimer <= 0)
		{
			StartSearch ();
		}
		else if (Vector3.Distance (transform.position, player.transform.position) < 1)
		{
			Attack ();
		}
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "CallingPoint" && state == EnemyState.called) {
			StartChase ();
			Destroy(other.gameObject);
		}
		else if (other.tag == "PatrolPoint")
		{
			if (state == EnemyState.patrol && other.transform == patrolPoints [destination])
			{
				SwitchDestination (patrolPoints);
			}
			else if (state == EnemyState.search && other.transform == searchPatrolPoints [destination] && !isLookingAround)
			{
				SwitchDestination (searchPatrolPoints);
				CheckEndSearch (other);
			}
		}
		else if (other.tag == "Player")
		{
			StartChase ();
			HeardPlayer ();
		}
	}
		
	void Attack ()
	{
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}

	void CheckEndSearch (Collider _other)
	{
		if (_other.transform == searchPatrolPoints [1])
		{
			StartPatrol ();
		}
		else
		{
			StartLookingARound ();
		}
	}
		
	void CheckForPlayer()
	{
		if (angleDifference < 30 && Physics.Linecast (transform.position, player.transform.position, out hit))
		{
			if (hit.collider == player)
			{
				StartChase ();
			}
		}
	}

	void HeardPlayer()
	{
		heardTimer = 1f;
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
		agent.isStopped = false;
		state = EnemyState.search;
		agent.speed = 2f;
		GetSearchPoints ();
		destination = 0;
		agent.destination = searchPatrolPoints [0].position;
	}

	void StartChase()
	{
		agent.isStopped = false;
		state = EnemyState.chase;
		agent.speed = 7f;
		StartCoroutine ("GetPlayerPosition");
	}
		
	void StartPatrol()
	{
		agent.isStopped = false;
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
		searchPatrolPoints = new Transform[] {level.searchPoints [0], level.searchPoints [1]};                  //À reprendre plus tard
		List<Transform> searchPointsInOrder = new List<Transform>();
		for (int n = 0; n < level.searchPoints.Length; n++)
		{
			searchPointsInOrder.Add (level.searchPoints[n]);
		}
		for (int i = 0; i < searchPatrolPoints.Length; i++)
		{
			int index = 0;
			for (int x = 0; x < searchPointsInOrder.Count - 1; x++)
			{
				if (Vector3.Distance (transform.position, searchPointsInOrder [x].position) < Vector3.Distance (transform.position, searchPatrolPoints [i].position) || x == 0)
				{
					searchPatrolPoints [i] = searchPointsInOrder [x];
					index = x;
				}
			}
			searchPointsInOrder.RemoveAt (index);
		}
	}

	void StartLookingARound()
	{
		isLookingAround = true;
		lookingAroundTimer = 360;
		agent.isStopped = true;
		pauseIndex = 0;
	}

	void LookAround()
	{
		transform.Rotate (transform.up * Time.deltaTime * (360/3));
		lookingAroundTimer -= Time.deltaTime * (360/3);
		if (lookingAroundTimer <= 0 && pauseIndex == 3)
		{
			isLookingAround = false;
			agent.isStopped = false;
		}
		else if ((lookingAroundTimer <= 90 && pauseIndex == 2) ||
				(lookingAroundTimer <= 180 && pauseIndex == 1) ||
				(lookingAroundTimer <= 270 && pauseIndex == 0))
		{
			StartCoroutine ("Pause");
		}
	}

	IEnumerator Pause()
	{
		pauseIndex++;
		isLookingAround = false;
		yield return new WaitForSeconds (1);
		isLookingAround = true;
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

	public void Called(Vector3 callDestination) {
		activated = true;
		state = EnemyState.called;
		agent.destination = callDestination;
	}

}

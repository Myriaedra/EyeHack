using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viewable : MonoBehaviour {
	Camera enemyView;
	// Use this for initialization
	void Start () {
		enemyView = transform.GetComponentInChildren<Camera> ();
		//Debug.Log (enemyView);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public Camera GetView()
	{
		return enemyView;
	}
}

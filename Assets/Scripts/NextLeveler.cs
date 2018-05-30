using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLeveler : MonoBehaviour {
    public enum Levels { Scn_Level_2, Scn_Level_3, Snc_Lvl_4, Scn_End_Game};
    public Levels nextLevel;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator ToNextLevel()
    {
        print("end"); 
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(nextLevel.ToString());
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            StartCoroutine(ToNextLevel());
    }
}

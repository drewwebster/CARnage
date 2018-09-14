using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrePlayerSelection : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        PlayerPrefs.SetString("Player0_controlledBy", "");
        PlayerPrefs.SetString("Player1_controlledBy", "");
        PlayerPrefs.SetString("Player2_controlledBy", "");
        PlayerPrefs.SetString("Player3_controlledBy", "");
        SceneManager.LoadScene("PLAYER_SELECTION");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

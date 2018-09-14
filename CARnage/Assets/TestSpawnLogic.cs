using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawnLogic : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        CarFactory.spawnCarsForAllPlayers(true);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

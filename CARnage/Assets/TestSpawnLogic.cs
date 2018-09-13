using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawnLogic : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        CarFactory.spawnCarForPlayer("Player0", Vector3.zero);

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

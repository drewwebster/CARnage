using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class freezeRotation : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.rotation = Quaternion.Euler(40, transform.rotation.y, 0);
        //transform.Rotate(Vector3.up, horizontal, Space.Self);
    }
}

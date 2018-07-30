using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletCase : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //transform.parent = null;
        GetComponent<Rigidbody>().AddForce(0, 200, -20);
        GetComponent<Rigidbody>().AddForce(transform.up * -200);
        GetComponent<Rigidbody>().AddForce(transform.forward * 20);
    }
	
}

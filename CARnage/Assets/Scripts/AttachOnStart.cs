using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachOnStart : MonoBehaviour {

    public GameObject attachTo;

	// Use this for initialization
	void Start () {
        transform.parent = attachTo.transform;
	}
}

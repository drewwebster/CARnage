using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTrajectory : MonoBehaviour {

    public float projectileSpeed = 10;	

	// Update is called once per frame
	void Update () {
        transform.Translate(0, Time.deltaTime * projectileSpeed, 0);
	}
}

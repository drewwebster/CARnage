using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CARnageWeapon : MonoBehaviour {

    public GameObject Projectile;

	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0))
        {
            // shoot
            GameObject go = Instantiate(Projectile,transform); // parent transform for intialisation
            go.transform.parent = null;
        }
	}
}

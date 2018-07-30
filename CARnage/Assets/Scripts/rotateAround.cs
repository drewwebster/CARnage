using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateAround : MonoBehaviour {

    public GameObject targetObject;
    public float speedScale = 5;
	
	// Update is called once per frame
	void Update () {
		if(targetObject != null)
        {
            transform.LookAt(targetObject.transform);
            transform.Translate(Vector3.right * Time.deltaTime * speedScale);
        }
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enableColliderDelayed : MonoBehaviour {

    public float delayTime = 0.25f;

    // Use this for initialization
    void Start () {
        Invoke("enableCollider", delayTime);
	}
	
    void enableCollider()
    {
        GetComponent<Collider>().enabled = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolcanoPort : MonoBehaviour {

    public GameObject portExit;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponentInParent<CARnageCar>())
        {
            int upForce = 55000;
            int forwardForce = 0;
            int sidewardForce = 0;

            switch (Random.Range(0,8))
            {
                case 0:
                    forwardForce = -20000;
                    break;
                case 1:
                    forwardForce = 5000;
                    break;
                case 2:
                    sidewardForce = -12000;
                    break;
                case 3:
                    sidewardForce = 10000;
                    break;
                case 4:
                    forwardForce = -11000;
                    sidewardForce = -11000;
                    break;
                case 5:
                    forwardForce = -12000;
                    sidewardForce = 12000;
                    break;
                case 6:
                    forwardForce = 6000;
                    sidewardForce = 11000;
                    break;
                case 7:
                    forwardForce = 5000;
                    sidewardForce = -10000;
                    break;
            }

            other.GetComponentInParent<CARnageCar>().transform.position = portExit.transform.position;
            other.GetComponentInParent<CARnageCar>().transform.rotation = Quaternion.identity;
            other.GetComponentInParent<CARnageCar>().GetComponent<Rigidbody>().velocity = Vector3.zero;
            other.GetComponentInParent<CARnageCar>().GetComponent<Rigidbody>().AddForce(Vector3.up * upForce + Vector3.forward * forwardForce + Vector3.right * sidewardForce, ForceMode.Impulse);


            //other.GetComponentInParent<CARnageCar>().GetComponent<Rigidbody>().AddTorque(transform.up * 7500, ForceMode.Impulse);
            other.GetComponentInParent<CARnageCar>().GetComponent<Rigidbody>().AddTorque(transform.forward * 7500, ForceMode.Impulse);
            other.GetComponentInParent<CARnageCar>().GetComponent<Rigidbody>().AddTorque(transform.right * 7500, ForceMode.Impulse);
        }
    }
}

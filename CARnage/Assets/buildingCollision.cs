using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buildingCollision : MonoBehaviour {

    float criticalForce = 10; // 10-15?
    //float impact = 10;
    public bool destroyed = false;

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.name);
        //Debug.Log(collision.relativeVelocity.magnitude);
        if (collision.gameObject.tag.Equals("Player") && GetComponent<Rigidbody>().isKinematic) // TODO: scale with "Impact"
        {
            float impact = collision.gameObject.GetComponent<CARnageCar>().impact;
            Debug.Log("car collision: " + collision.relativeVelocity.magnitude * impact);
            damageMe(collision.relativeVelocity.magnitude * impact);
        }

        //if (collision.gameObject.tag.Equals("BuildingPart") && collision.gameObject.GetComponent<buildingCollision>().getResidualForce() * impact >= criticalForce)
        //{
        //    Debug.Log("residual force destroy: " + collision.gameObject.GetComponent<buildingCollision>().getResidualForce() * impact);
        //    destroyMe(collision.gameObject.GetComponent<buildingCollision>().getResidualForce() * impact);
        //}
    }

    void damageMe(float force)
    {
        if (force >= criticalForce)
        {
            destroyMe(force);
        }
    }

    void destroyMe(float force)
    {
        destroyed = true;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().useGravity = true;
        //GetComponent<Collider>().isTrigger = true;
        //GetComponent<Rigidbody>().AddForce(25, 1000, 30);
        if (force > 0)
            transform.parent.GetComponent<Building>().removePart(gameObject);
        GetComponent<Rigidbody>().AddForce(Random.Range(1f, 100f), Random.Range(1f, 100f), Random.Range(1f, 100f), ForceMode.Acceleration);
        calcAdditionalDamage(force - criticalForce);
        Invoke("sink", Random.Range(4f,8f));
    }

    void sink()
    {
        GetComponent<Collider>().enabled = false;
        Destroy(gameObject, 5);
    }

    void calcAdditionalDamage(float residualForce)
    {
        if (residualForce < criticalForce)
            return;

        //find random building part
        //Transform[] parts = transform.parent.GetComponentsInChildren<Transform>();
        //Transform part = parts[Random.Range(0, parts.Length)];
        if(transform.parent.GetComponent<Building>().getBuildingParts().Count <= 700)
        {
            // Destroy entirely
            foreach(GameObject part in transform.parent.GetComponent<Building>().getBuildingParts())
            {
                part.GetComponent<buildingCollision>().destroyMe(-1);
            }
            return;
        }

        GameObject go = transform.parent.GetComponent<Building>().getBuildingParts()[Random.Range(0, transform.parent.GetComponent<Building>().getBuildingParts().Count)];
        
        Debug.Log("residual damage: " + residualForce);
        //transform.parent.GetComponent<Building>().buildingParts.Remove(go);
        go.GetComponent<buildingCollision>().damageMe(residualForce);

        //float mindist = float.PositiveInfinity;
        //Transform minTrans = null;
        //foreach (Transform part in parts)
        //{
        //    if (part.gameObject.GetComponent<buildingCollision>() != null && !part.gameObject.GetComponent<buildingCollision>().destroyed && Mathf.Pow(Vector3.Distance(part.position, transform.position), 2) < mindist)
        //    {
        //        mindist = Mathf.Pow(Vector3.Distance(part.position, transform.position), 2);
        //        minTrans = part;
        //    }
        //}
        //if (minTrans != null)
        //    minTrans.GetComponent<buildingCollision>().damageMe(residualForce);

        //calcAdditionalDamage();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buildingCollision : MonoBehaviour {

    float criticalForce = 10; // 10-15?
    //float impact = 10;
    public bool destroyed = false;
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Player") && !destroyed) // TODO: scale with "Impact"
        {
            CARnageCar dmgCar = collision.gameObject.GetComponent<CARnageCar>();
            if(dmgCar)
                damageMe(collision.relativeVelocity.magnitude * dmgCar.impact, true, DamageType.RAM, dmgCar);
        }

        //if (collision.gameObject.tag.Equals("BuildingPart") && collision.gameObject.GetComponent<buildingCollision>().getResidualForce() * impact >= criticalForce)
        //{
        //    Debug.Log("residual force destroy: " + collision.gameObject.GetComponent<buildingCollision>().getResidualForce() * impact);
        //    destroyMe(collision.gameObject.GetComponent<buildingCollision>().getResidualForce() * impact);
        //}
    }

    public void damageMe(float damage, bool initialDMG, DamageType damageType, CARnageCar damager)
    {
        if (destroyed)
            return;
        if (initialDMG && damager)
            damage = damage * damager.getModController().getBuildingDMG_Multiplier();

        GetComponentInParent<Building>().lastDamager = damager;

        if (damage > 0 && initialDMG)
        {
            Transform parentTrans = GetComponentInParent<Building>().transform;
            if (GetComponentInParent<Building>().GetComponentInChildren<spawnableOffset>())
                parentTrans = GetComponentInParent<Building>().GetComponentInChildren<spawnableOffset>().transform;
            GameObject dmgDisplay = Instantiate(Resources.Load<GameObject>("DMGDisplay"), parentTrans);
            dmgDisplay.GetComponent<DMGdisplay>().display((int)damage, damageType);
        }

        if (damage >= criticalForce)
        {
            if (initialDMG)
            {
                // Particle FX: Get Building Center and align FX
                GameObject fx = Instantiate(Resources.Load<GameObject>("FX_BuildingDamage"), transform);
                fx.transform.LookAt(transform.parent);
                fx.transform.Rotate(0, 90, 0);
            }

            destroyMe(damage, damageType, damager);
        }
        else
            criticalForce -= damage; // Damage the buildingPart
    }

    public void destroyMe(float force, DamageType damageType, CARnageCar destroyer)
    {
        destroyed = true;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().useGravity = true;
        //GetComponent<Collider>().isTrigger = true;
        //GetComponent<Rigidbody>().AddForce(25, 1000, 30);
        if (force > 0)
            GetComponentInParent<Building>().removePart(gameObject);
        GetComponent<Rigidbody>().AddForce(Random.Range(1f, 100f), Random.Range(1f, 100f), Random.Range(1f, 100f), ForceMode.Acceleration);
        GetComponentInParent<Building>().checkIfDestroyed();
        if (force - criticalForce >= criticalForce)
            GetComponentInParent<Building>().calcAdditionalDamage(force - criticalForce, damageType);
        Invoke("sink", Random.Range(4f,8f));

        if (destroyer)
            destroyer.getModController().onBuildingDestroyed();
    }

    void sink()
    {
        GetComponent<Collider>().enabled = false;
        Destroy(gameObject, 5);
    }

    public Building getBuilding()
    {
        return GetComponentInParent<Building>();
    }
}

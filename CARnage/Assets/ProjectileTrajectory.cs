using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTrajectory : MonoBehaviour {

    public GameObject rel_car;
    public CARnageWeapon rel_weapon;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.name);
        if(rel_car.GetComponent<CARnageCar>() != other.GetComponentInParent<CARnageCar>()) // no friendly fire in projectiles
        {
            if(other.gameObject.GetComponent<buildingCollision>() != null)
            {
                // damage to building
                other.gameObject.GetComponent<buildingCollision>().damageMe(rel_weapon.Damage,true);
                //Debug.Log("bullet > building");
            }
            CARnageCar car = other.GetComponentInParent<CARnageCar>();
            if (car != null)
            {
                // damage to car
                Debug.Log("bullet => " + car.gameObject.name);
                car.damageMe(rel_weapon.Damage);
            }
            Destroy(gameObject);
        }
    }
}

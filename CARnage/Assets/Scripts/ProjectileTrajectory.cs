using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTrajectory : MonoBehaviour {

    public CARnageCar rel_car;
    public CARnageWeapon rel_weapon;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.name);
        if(rel_car != other.GetComponentInParent<CARnageCar>()) // no friendly fire in projectiles
        {
            if(other.gameObject.GetComponent<buildingCollision>() != null)
            {
                // damage to building
                float damage = rel_weapon.calcDamage(other.gameObject.GetComponent<buildingCollision>());
                other.gameObject.GetComponent<buildingCollision>().damageMe(damage, true);
            }
            CARnageCar damagedCar = other.GetComponentInParent<CARnageCar>();
            if (damagedCar != null)
            {
                // damage to car
                float damage = rel_weapon.calcDamage(damagedCar);
                damagedCar.damageMe(damage,rel_car,DamageType.PROJECTILE);
            }
            Destroy(gameObject);
        }
    }
}

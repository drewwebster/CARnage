using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTrajectory : MonoBehaviour {

    public CARnageCar rel_car;
    public CARnageWeapon rel_weapon;

    private void Start()
    {
        if (GetComponentInChildren<explosionHitbox>())
            GetComponentInChildren<explosionHitbox>().rel_weapon = rel_weapon;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<CARnageCar>() == rel_car || other.GetComponent<CARnageWeapon>() != null || other.gameObject.name.Contains("Bulletcase")) // no friendly fire in projectiles
            return;
            Debug.Log(other.gameObject.name);

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
            rel_weapon.OnDMG_WeaponModelMod(rel_car, damagedCar);
        }

        if (GetComponent<explodableHitbox>() != null)
            GetComponent<explodableHitbox>().explode();
        else
            Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meleeHitbox : MonoBehaviour {

    public bool reflectsProjectiles;

    private void OnTriggerEnter(Collider other)
    {
        CARnageCar rel_car = GetComponentInParent<CARnageCar>();
        CARnageWeapon rel_weapon = GetComponentInParent<CARnageWeapon>();
        CARnageCar damagedCar = other.GetComponentInParent<CARnageCar>();
        ProjectileTrajectory projectile = other.GetComponentInParent<ProjectileTrajectory>();
        if (rel_weapon.meleeDMGdelay)
            return;
        if (damagedCar == rel_car) // no friendly fire
            return;
        if (rel_weapon.weaponState == CARnageWeapon.WeaponState.COLLECTABLE)
            return;
        //Debug.Log("hit: " + other.gameObject.name);
        if(reflectsProjectiles && projectile != null)
        {
            projectile.reflectMe();
            return;
        }

        buildingCollision building = other.gameObject.GetComponent<buildingCollision>();
        if (building != null)
        {
            // damage to building
            float damage = rel_weapon.calcDamage(building);
            other.gameObject.GetComponent<buildingCollision>().damageMe(damage, true, DamageType.MELEE, rel_car);
            rel_weapon.onHit();
            rel_weapon.OnDMG_WeaponModelMod(rel_car, building);
        }
        if (damagedCar != null)
        {
            // damage to car
            float damage = rel_weapon.calcDamage(damagedCar);
            damagedCar.damageMe(damage, rel_car, DamageType.MELEE);
            rel_weapon.onHit();
            rel_weapon.OnDMG_WeaponModelMod(rel_car,damagedCar);
        }
    }
}

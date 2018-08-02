using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meleeHitbox : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        CARnageCar rel_car = GetComponentInParent<CARnageCar>();
        CARnageWeapon rel_weapon = GetComponentInParent<CARnageWeapon>();
        CARnageCar damagedCar = other.GetComponentInParent<CARnageCar>();
        if (rel_weapon.meleeDMGdelay)
            return;
        if (damagedCar == rel_car) // no friendly fire
            return;
        if (rel_weapon.weaponState == CARnageWeapon.WeaponState.COLLECTABLE)
            return;
        Debug.Log("hit: " + other.gameObject.name);

        if (other.gameObject.GetComponent<buildingCollision>() != null)
        {
            // damage to building
            float damage = rel_weapon.calcDamage(other.gameObject.GetComponent<buildingCollision>());
            other.gameObject.GetComponent<buildingCollision>().damageMe(damage, true);
            rel_weapon.onHit();
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

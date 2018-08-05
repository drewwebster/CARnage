using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosionHitbox : MonoBehaviour {

    [HideInInspector]
    public CARnageWeapon rel_weapon;

    private void Start()
    {
        Destroy(gameObject, 0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name);
        if(other.GetComponent<damageCar>())
        {
            CARnageCar damagedCar = other.GetComponentInParent<CARnageCar>();
            float damage = rel_weapon.calcDamage(damagedCar);
            damagedCar.damageMe(damage, rel_weapon.getCar(),DamageType.EXPLOSION);
            rel_weapon.OnDMG_WeaponModelMod(rel_weapon.getCar(), damagedCar);

            // knockback
            var heading = other.transform.position - transform.position;
            //var distance = heading.magnitude;
            //var direction = heading / distance;

            heading.y = 1;
            
            var knockbackDirection = heading * 10000;
            damagedCar.GetComponent<Rigidbody>().AddForce(knockbackDirection, ForceMode.Impulse);
        }        
    }
}

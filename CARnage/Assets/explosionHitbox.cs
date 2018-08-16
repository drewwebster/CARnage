using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosionHitbox : MonoBehaviour {
    
    public CARnageWeapon rel_weapon;
    public float fixedDamage;
    public CARnageCar rel_car;
    List<CARnageCar> alreadyDamaged;

    private void Start()
    {
        alreadyDamaged = new List<CARnageCar>();
        foreach (Collider c in GetComponentsInChildren<Collider>())
            c.enabled = true;
        Destroy(gameObject, 0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<CARnageCar>())
            Debug.Log("explo collision: " + other.GetComponentInParent<CARnageCar>().gameObject.name);
        Debug.Log(other.gameObject.name);
        if (other.GetComponent<damageCar>())
        {
            CARnageCar damagedCar = other.GetComponentInParent<CARnageCar>();
            if (alreadyDamaged.Contains(damagedCar))    // single explosion hit for all vehicles
                return;
            alreadyDamaged.Add(damagedCar);

            if (rel_weapon != null)
                rel_car = rel_weapon.getCar();
            
            float damage = 0;
            if (fixedDamage > 0)
                damage = fixedDamage;
            else
                damage = rel_weapon.calcDamage(damagedCar);
            
            damagedCar.damageMe(damage, rel_car,DamageType.EXPLOSION);

            if(rel_weapon != null)
                rel_weapon.OnDMG_WeaponModelMod(rel_weapon.getCar(), damagedCar);

            // knockback
            var heading = other.transform.position - transform.position;
            //var distance = heading.magnitude;
            //var direction = heading / distance;

            heading.y = 1;

            var knockbackDirection = heading * 10000;
            if(rel_weapon != null)
                knockbackDirection *= rel_weapon.knockbackMult;
            damagedCar.GetComponent<Rigidbody>().AddForce(knockbackDirection, ForceMode.Impulse);
        }        
    }
}

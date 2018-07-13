using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTrajectory : MonoBehaviour {

    public GameObject rel_car;
    public CARnageWeapon rel_weapon;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.name);
        if(!CARnageAuxiliary.getCarFromHitbox(other.gameObject)) // no friendly fire in projectiles
        {
            if(other.gameObject.GetComponent<buildingCollision>() != null)
            {
                // damage to building
                other.gameObject.GetComponent<buildingCollision>().damageMe(rel_weapon.Damage,true);
                Debug.Log("bullet > building");
            }
            Destroy(gameObject);
        }
    }
}

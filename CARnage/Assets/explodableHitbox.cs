using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explodableHitbox : MonoBehaviour {

    bool exploded;
    public float explodeTime = 3f;
    public GameObject explosionFX;
    [HideInInspector]
    public CARnageWeapon rel_weapon;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.GetComponentInParent<CARnageCar>() != null)
        explode();
    }

    private void Start()
    {
        Invoke("explode", explodeTime);
    }
    
    public void explode()
    {
        if (exploded)   // can explode after time or @collision
            return;
        exploded = true;
        explosionFX.SetActive(true);
        explosionFX.GetComponentInChildren<explosionHitbox>().rel_weapon = rel_weapon;
        explosionFX.transform.parent = null;
        explosionFX.transform.rotation = Quaternion.identity;

        //GetComponentInParent<CARnageWeapon>().onExplosionDMG;
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.transform.GetComponent<damageCar>() != null)
            Debug.Log("SPHERE COLLISION: " + other.name);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explodableHitbox : MonoBehaviour {

    bool exploded;
    public float explodeTime = 3f;
    public GameObject explosionFX;

    public CARnageWeapon rel_weapon;
    bool isCollidable = false;

    private void OnCollisionEnter(Collision collision)
    {
        if(isCollidable && (collision.transform.GetComponentInParent<CARnageCar>() != null || collision.transform.GetComponentInParent<Building>()))
            explode();
    }

    private void Start()
    {
        Invoke("explode", explodeTime);
        Invoke("setCollidable", 0.25f);
    }

    public void setCollidable()
    {
        isCollidable = true;
    }
    
    public void explode()
    {
        if (exploded)   // can explode after time or @collision
            return;

        exploded = true;
        explosionFX.GetComponentInChildren<explosionHitbox>().rel_weapon = rel_weapon;
        explosionFX.SetActive(true);
        explosionFX.transform.parent = null;
        explosionFX.transform.rotation = Quaternion.identity;

        //GetComponentInParent<CARnageWeapon>().onExplosionDMG;
        Destroy(gameObject);
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantDestructor : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {

        CARnageCar damagedCar = other.GetComponentInParent<CARnageCar>();
        if (damagedCar != null)
        {
            damagedCar.damageMe(999, null, DamageType.DIRECT_DAMAGE);
        }
    }
}

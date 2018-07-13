using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CARnageAuxiliary : MonoBehaviour {

    public static GameObject getCarFromHitbox(GameObject hitbox)
    {
        if (hitbox.transform.parent == null)
            return null;
        if (hitbox.GetComponent<RCC_CarControllerV3>() != null)
            return hitbox;

        return getCarFromHitbox(hitbox.transform.parent.gameObject);
    }
}

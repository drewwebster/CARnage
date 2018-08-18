using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFactory : MonoBehaviour {
    
    static CARnageWeapon.WeaponModel getRndModel()
    {
        //  actual:
        Array values = Enum.GetValues(typeof(CARnageWeapon.WeaponModel));
        return (CARnageWeapon.WeaponModel)values.GetValue(UnityEngine.Random.Range(0, values.Length));
    }

    public static GameObject spawnRndWeapon(Vector3 position)
    {
        return spawnWeapon(getRndModel(), position);
    }

    public static GameObject spawnRndWeapon(CARnageCar car)
    {
        return spawnWeapon(getRndModel(), car);
    }

    public static GameObject spawnWeapon(CARnageWeapon.WeaponModel model, CARnageCar car)
    {
        return spawnWeapon(model, car, Vector3.zero);
    }

    public static GameObject spawnWeapon(CARnageWeapon.WeaponModel model, Vector3 position)
    {
        return spawnWeapon(model, null, position);
    }

    static GameObject spawnWeapon(CARnageWeapon.WeaponModel model, CARnageCar car, Vector3 position)
    {
        //Debug.Log("Spawning weapon: " + model.ToString());
        if(car != null)
        {
            GameObject go = Instantiate(Resources.Load<GameObject>(model.ToString()), car.getModController().transform);
            car.getWeaponController().obtainWeapon(go);
            return go;
        }
        else
        {
            GameObject go = Instantiate(Resources.Load<GameObject>(model.ToString()));
            go.transform.position = new Vector3(position.x, position.y + 10, position.z);
            return go;
        }
    }
}

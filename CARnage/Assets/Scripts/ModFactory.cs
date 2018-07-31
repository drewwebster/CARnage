using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModFactory : MonoBehaviour {
    
    public static GameObject spawnRandomMod(CARnageCar car)
    {
        Array values = Enum.GetValues(typeof(CARnageModifier.ModID));
        CARnageModifier.ModID mod = (CARnageModifier.ModID)values.GetValue(UnityEngine.Random.Range(0, values.Length));

        return spawnMod(mod, car);
    }

	public static GameObject spawnMod(CARnageModifier.ModID modID, CARnageCar car)
    {
        GameObject go = Instantiate(Resources.Load<GameObject>(modID.ToString()), car.getModController().transform);
        go.GetComponent<CARnageModifier>().onSpawn();
        return go;
    }
}

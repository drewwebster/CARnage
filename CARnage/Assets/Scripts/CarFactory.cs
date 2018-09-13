using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarFactory : MonoBehaviour {

    public static GameObject spawnCar(CarModel carModel, Vector3 position)
    {
        return spawnCar(carModel, position, "");
    }
    public static GameObject spawnCarForPlayer(string playerName, Vector3 position)
    {
        return spawnCar((CarModel)Enum.Parse(typeof(CarModel), PlayerPrefs.GetString(playerName + "_Car")), position, playerName);
    }
    public static GameObject spawnCar(CarModel carModel, Vector3 position, string playerName)
    {
        GameObject go = Instantiate(Resources.Load<GameObject>(carModel.ToString()));
        go.transform.position = new Vector3(position.x, position.y + 10, position.z);
        go.GetComponent<CARnageCar>().controlledBy = CARnageAuxiliary.ControllerType.AI;

        if (!playerName.Equals(""))
        {
            modifyCarToPlayerPrefs(go, playerName);
            spawnCameraForCar(go);
        }
            

        return go;
    }

    public static void modifyCarToPlayerPrefs(GameObject car, string playerName)
    {
        CarModel carModel = car.GetComponent<CARnageCar>().carModel;
        if (!PlayerPrefs.GetString(playerName + "_controlledBy").Equals(""))
            car.GetComponent<CARnageCar>().controlledBy = (CARnageAuxiliary.ControllerType)Enum.Parse(typeof(CARnageAuxiliary.ControllerType), PlayerPrefs.GetString(playerName + "_controlledBy"));
        if (!PlayerPrefs.GetString(playerName + "_CarColor_" + carModel).Equals(""))
            car.GetComponent<CARnageCar>().setColor((CARnageCar.CarColor)Enum.Parse(typeof(CARnageCar.CarColor), PlayerPrefs.GetString(playerName + "_CarColor_" + carModel)));
        
        if (car.GetComponent<CARnageCar>().getWeaponController().getLeftWeapon() && !PlayerPrefs.GetString(playerName + "_WeaponUpgrade_" + car.GetComponent<CARnageCar>().getWeaponController().getLeftWeapon().weaponName).Equals(""))
            car.GetComponent<CARnageCar>().getWeaponController().getLeftWeapon().addUpgrade((CARnageWeapon.UpgradeTypes)Enum.Parse(typeof(CARnageWeapon.UpgradeTypes), PlayerPrefs.GetString(playerName + "_WeaponUpgrade_" + car.GetComponent<CARnageCar>().getWeaponController().getLeftWeapon().weaponName)));
        if (car.GetComponent<CARnageCar>().getWeaponController().getRightWeapon() && !PlayerPrefs.GetString(playerName + "_WeaponUpgrade_" + car.GetComponent<CARnageCar>().getWeaponController().getRightWeapon().weaponName).Equals(""))
            car.GetComponent<CARnageCar>().getWeaponController().getRightWeapon().addUpgrade((CARnageWeapon.UpgradeTypes)Enum.Parse(typeof(CARnageWeapon.UpgradeTypes), PlayerPrefs.GetString(playerName + "_WeaponUpgrade_" + car.GetComponent<CARnageCar>().getWeaponController().getRightWeapon().weaponName)));

        if(!PlayerPrefs.GetString("Player0_CarMod_" + carModel + "_0").Equals(""))
        {
            foreach (CARnageModifier mod in car.GetComponent<CARnageCar>().getModController().getMods())
                DestroyImmediate(mod.gameObject);
            Instantiate(Resources.Load<GameObject>("MODSResources/" + PlayerPrefs.GetString("Player0_CarMod_" + carModel + "_0")), car.GetComponent<CARnageCar>().getModController().transform);
            Instantiate(Resources.Load<GameObject>("MODSResources/" + PlayerPrefs.GetString("Player0_CarMod_" + carModel + "_1")), car.GetComponent<CARnageCar>().getModController().transform);
        }

    }

    public static void spawnCameraForCar(GameObject car)
    {
        GameObject go = Instantiate(Resources.Load<GameObject>("CARnageCamera"));
        go.GetComponent<RCC_Camera>().playerCar = car.transform;
    }
}

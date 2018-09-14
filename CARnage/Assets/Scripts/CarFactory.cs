using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarFactory : MonoBehaviour {

    public static GameObject spawnCar(CarModel carModel, Vector3 position)
    {
        return spawnCar(carModel, position, "", false);
    }
    public static GameObject spawnCarForPlayer(string playerName, Vector3 position, bool alsoSpawnCamera)
    {
        Debug.Log("Spawn for: " + playerName + "_Car | " + PlayerPrefs.GetString(playerName + "_Car"));
        return spawnCar((CarModel)Enum.Parse(typeof(CarModel), PlayerPrefs.GetString(playerName + "_Car")), position, playerName, alsoSpawnCamera);
    }
    public static GameObject spawnCar(CarModel carModel, Vector3 position, string playerName, bool alsoSpawnCamera)
    {
        GameObject go = Instantiate(Resources.Load<GameObject>(carModel.ToString()));
        go.transform.position = new Vector3(position.x, position.y + 10, position.z);
        go.GetComponent<CARnageCar>().controlledBy = CARnageAuxiliary.ControllerType.AI;

        if (!playerName.Equals(""))
        {
            modifyCarToPlayerPrefs(go, playerName);
            if(alsoSpawnCamera)
                spawnCameraForCar(go, playerName);
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

    public static void spawnCameraForCar(GameObject car, string playerName)
    {
        int playerID = int.Parse(playerName.Substring(playerName.Length - 1));
        GameObject go = Instantiate(Resources.Load<GameObject>("CARnageCamera"));
        go.GetComponent<RCC_Camera>().playerCar = car.transform;
        switch(CARnageAuxiliary.getPlayersPlayingCount())
        {
            case 2:
                if (playerID == 0)
                    go.GetComponentInChildren<Camera>().rect = new Rect(0, 0, 0.5f, 1);
                if (playerID == 1)
                    go.GetComponentInChildren<Camera>().rect = new Rect(0.5f, 0, 0.5f, 1);
                break;
            case 3:
                if (playerID == 0)
                    go.GetComponentInChildren<Camera>().rect = new Rect(0, 0.5f, 0.5f, 0.5f);
                if (playerID == 1)
                    go.GetComponentInChildren<Camera>().rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                if (playerID == 2)
                    go.GetComponentInChildren<Camera>().rect = new Rect(0, 0, 1, 0.5f);
                break;
            case 4:
                if (playerID == 0)
                    go.GetComponentInChildren<Camera>().rect = new Rect(0, 0.5f, 0.5f, 0.5f);
                if (playerID == 1)
                    go.GetComponentInChildren<Camera>().rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                if (playerID == 2)
                    go.GetComponentInChildren<Camera>().rect = new Rect(0, 0, 0.5f, 0.5f);
                if (playerID == 3)
                    go.GetComponentInChildren<Camera>().rect = new Rect(0.5f, 0, 0.5f, 0.5f);
                break;
        }
    }

    public static void spawnCarsForAllPlayers(bool alsoSpawnCameras)
    {
        if (!PlayerPrefs.GetString("Player0_controlledBy").Equals(""))
            spawnCarForPlayer("Player0", Vector3.zero, alsoSpawnCameras);
        if (!PlayerPrefs.GetString("Player1_controlledBy").Equals(""))
            spawnCarForPlayer("Player1", new Vector3(10, 0, 10), alsoSpawnCameras);
        if (!PlayerPrefs.GetString("Player2_controlledBy").Equals(""))
            spawnCarForPlayer("Player2", new Vector3(20, 0, 20), alsoSpawnCameras);
        if (!PlayerPrefs.GetString("Player3_controlledBy").Equals(""))
            spawnCarForPlayer("Player3", new Vector3(30, 0, 30), alsoSpawnCameras);

        if(alsoSpawnCameras)
        {
            GameObject multiplayerCanvas = Instantiate(Resources.Load<GameObject>("CARnageMultiplayerCanvas"));
            Debug.Log(CARnageAuxiliary.getPlayersPlayingCount());
            switch(CARnageAuxiliary.getPlayersPlayingCount())
            {
                case 1:
                    Destroy(multiplayerCanvas);
                    break;
                case 2:
                    multiplayerCanvas.transform.Find("3players").gameObject.SetActive(false);
                    multiplayerCanvas.transform.Find("4players").gameObject.SetActive(false);
                    break;
                case 3:
                    multiplayerCanvas.transform.Find("2players").gameObject.SetActive(false);
                    multiplayerCanvas.transform.Find("4players").gameObject.SetActive(false);
                    break;
                case 4:
                    multiplayerCanvas.transform.Find("3players").gameObject.SetActive(false);
                    multiplayerCanvas.transform.Find("2players").gameObject.SetActive(false);
                    break;
            }
        }
    }
}

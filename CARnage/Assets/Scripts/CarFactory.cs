using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarFactory : MonoBehaviour {

    public static GameObject spawnCar(CarModel carModel, Vector3 position)
    {
        GameObject go = Instantiate(Resources.Load<GameObject>(carModel.ToString()));
        go.transform.position = new Vector3(position.x, position.y + 10, position.z);
        go.GetComponent<CARnageCar>().controlledBy = CARnageAuxiliary.ControllerType.AI;

        return go;
    }
}

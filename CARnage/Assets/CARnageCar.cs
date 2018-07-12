using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CARnageCar : MonoBehaviour {

    public int impact = 1;
    public GameObject speedUI;

    private void Update()
    {
        if (speedUI != null)
            speedUI.GetComponent<Text>().text = (int)(GetComponent<RCC_CarControllerV3>().speed) + " km/h";
    }
}

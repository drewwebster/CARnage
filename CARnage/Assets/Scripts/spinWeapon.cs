using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spinWeapon : MonoBehaviour
{
    public GameObject collectableFX;

    // Update is called once per frame
    void Update()
    {
        //GetComponent<CARnageWeapon>();
        transform.Rotate(0, 100 * Time.deltaTime, 0);
    }
}

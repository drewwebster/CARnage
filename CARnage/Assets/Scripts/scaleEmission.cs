using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scaleEmission : MonoBehaviour {

    float minSpeed = 0;
    float maxSpeed = 100;
    float minEmissionSize = 0.2f;
    float maxEmissionSize = 1;
    public GameObject nitroFX;
    public GameObject emissionFX;

    // Update is called once per frame
    void Update() {
        if (GetComponentInParent<CARnageCar>().destroyed)
        {
            gameObject.SetActive(false);
            return;
        }
        float speed = GetComponentInParent<RCC_CarControllerV3>().speed; // get speed of vehicle

        if (GetComponentInParent<RCC_CarControllerV3>().isUsingNitro)
        {
            nitroFX.SetActive(true);
            //emissionFX.SetActive(false);
            return;
        }   

        nitroFX.SetActive(false);
        emissionFX.SetActive(true);

        ParticleSystem ps = emissionFX.GetComponent<ParticleSystem>();
        var main = ps.main;
        if (speed < 0)
            speed *= -1;
        if (speed < minSpeed)
            speed = minSpeed;
        if (speed > maxSpeed)
            speed = maxSpeed;

        float percentage = (speed - minSpeed)/ (maxSpeed - minSpeed);
        float emission = ((maxEmissionSize - minEmissionSize) * percentage) + minEmissionSize;
        main.startSize = emission;
	}
}

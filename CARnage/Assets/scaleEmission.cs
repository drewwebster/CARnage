using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scaleEmission : MonoBehaviour {

    float minSpeed = 0;
    float maxSpeed = 100;
    float minEmissionSize = 0.2f;
    float maxEmissionSize = 1;
    	
	// Update is called once per frame
	void Update () {
        float speed = transform.parent.GetComponent<RCC_CarControllerV3>().speed; // get speed of vehicle

        if (speed < 0)
            speed *= -1;
        if (speed < minSpeed)
            speed = minSpeed;
        if (speed > maxSpeed)
            speed = maxSpeed;

        float percentage = (speed - minSpeed)/ (maxSpeed - minSpeed);
        float emission = ((maxEmissionSize - minEmissionSize) * percentage) + minEmissionSize;
        ParticleSystem ps = GetComponent<ParticleSystem>();
        var main = ps.main;
        main.startSize = emission;
	}
}

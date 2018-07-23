using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damageCar : MonoBehaviour {

    bool acid;
    bool fire;
    bool drain;

    public GameObject FX_CarAcid;
    public GameObject FX_CarFire;
    public GameObject FX_CarDrain;
    public GameObject FX_HPSmoke;
    public GameObject FX_Destroy;

    // Update is called once per frame
    void Update () {
        // debug: H
		if(Input.GetKeyDown(KeyCode.H))
        {
            if (acid)
            {
                acid = false;
                if (FX_CarAcid != null)
                    FX_CarAcid.SetActive(false);
                fire = true;
                if (FX_CarFire != null)
                    FX_CarFire.SetActive(true);
                GetComponent<Animator>().SetTrigger("Fire");
                return;
            }
            if (fire)
            {
                fire = false;
                if (FX_CarFire != null)
                    FX_CarFire.SetActive(false);
                drain = true;
                if (FX_CarDrain != null)
                    FX_CarDrain.SetActive(true);
                GetComponent<Animator>().SetTrigger("Drain");
                return;
            }
            if (drain)
            {
                if (FX_CarDrain != null)
                    FX_CarDrain.SetActive(false);
                drain = false;
                GetComponent<Animator>().SetTrigger("Neutral");
                return;
            }

            //else
            acid = true;
            if (FX_CarAcid != null)
                FX_CarAcid.SetActive(true);
            GetComponent<Animator>().SetTrigger("Acid");
        }
	}

    public void updateHP(CARnageCar car)
    {

        if (car.currentHP <= 0 && car.currentShield <= 0)
        {
            FX_CarAcid.SetActive(false);
            FX_CarDrain.SetActive(false);
            FX_CarFire.SetActive(false);
            FX_HPSmoke.SetActive(false);
            FX_Destroy.SetActive(true);
            return;
        }

        float percentage = 1;
        if(car.currentShield <= 0)
        {
            percentage = car.currentHP * 2 / car.maxHP;
        }
        if (percentage > 1)
            percentage = 1;

        percentage = 1 - percentage;

        if (percentage > 0)
        {
            FX_HPSmoke.SetActive(true);
            var ps = FX_HPSmoke.GetComponent<ParticleSystem>().main;
            ps.startSize = new ParticleSystem.MinMaxCurve(percentage * 2f, percentage * 6f);
        }
        else
            FX_HPSmoke.SetActive(false);

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damageCar : MonoBehaviour {

    bool acid;
    bool fire;
    bool drain;
    bool freeze;
    bool locked;

    public GameObject FX_CarAcid;
    public GameObject FX_CarFire;
    public GameObject FX_CarDrain;
    public GameObject FX_HPSmoke;
    public GameObject FX_Destroy;
    public GameObject FX_Freeze;
    public GameObject FX_Locked;

    public void showAcid()
    {
        acid = true;
        if (FX_CarAcid != null)
            FX_CarAcid.SetActive(true);
        GetComponent<Animator>().SetTrigger("Acid");
    }

    public void hideAcid()
    {
        acid = false;
        if (FX_CarAcid != null)
            FX_CarAcid.SetActive(false);
        onHide();
    }

    public void showFire()
    {
        fire = true;
        if (FX_CarFire != null)
            FX_CarFire.SetActive(true);
        GetComponent<Animator>().SetTrigger("Fire");
    }
    public void hideFire()
    {
        fire = false;
        if (FX_CarFire != null)
            FX_CarFire.SetActive(false);
        onHide();
    }

    public void showDrain()
    {
        drain = true;
        if (FX_CarDrain != null)
            FX_CarDrain.SetActive(true);
        GetComponent<Animator>().SetTrigger("Drain");
    }
    public void hideDrain()
    {
        if (FX_CarDrain != null)
            FX_CarDrain.SetActive(false);
        drain = false;
        onHide();
    }

    public void showFreeze()
    {
        freeze = true;
        if (FX_Freeze != null)
            FX_Freeze.SetActive(true);
        GetComponent<Animator>().SetTrigger("Freeze");
    }
    public void hideFreeze()
    {
        if (FX_Freeze != null)
            FX_Freeze.SetActive(false);
        freeze = false;
        onHide();
    }

    public void showLocked()
    {
        locked = true;
        if (FX_Locked != null)
            FX_Locked.SetActive(true);
        GetComponent<Animator>().SetTrigger("Locked");
    }
    public void hideLocked()
    {
        if (FX_Locked != null)
            FX_Locked.SetActive(false);
        locked = false;
        onHide();
    }

    void onHide()
    {
        // handle multiple debuffs
        if (fire)
            showFire();
        else if (acid)
            showAcid();
        else if (drain)
            showDrain();
        else
            GetComponent<Animator>().SetTrigger("Neutral");
    }


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

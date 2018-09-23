using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

    public GameObject speedTextGO;
    public GameObject gearTextGO;
    public GameObject killsGO;
    public GameObject ModsGO;
    public GameObject ModIconIngame_Prefab;
    CARnageCar rel_car;

    public Sprite transparentSprite;
    public GameObject destructionScreenGO;
    public GameObject destroyerCarGO;
    public GameObject destroyedCarGO;
    public GameObject destructionType_fire;
    public GameObject destructionType_acid;
    public GameObject destructionType_leak;
    public GameObject destructionType_ram;
    public GameObject destructionType_directDMG;
    public GameObject destructionType_projectile;
    public GameObject destructionType_melee;
    public GameObject destructionType_explosion;

    public void init(CARnageCar car)
    {
        rel_car = car;

        foreach (CARnageModifier mod in rel_car.getModController().getMods())
        {
            GameObject go = Instantiate(ModIconIngame_Prefab, ModsGO.transform);
            go.GetComponentInChildren<Image>().sprite = mod.image;
            go.GetComponentInChildren<Text>().text = CARnageAuxiliary.colorMe(mod.description);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (!rel_car)
            return;

        speedTextGO.GetComponent<Text>().text = (int)(rel_car.GetComponent<RCC_CarControllerV3>().speed) + "<size=40> km/</size><size=25>h</size>";
        gearTextGO.GetComponent<Text>().text = rel_car.currentGears + "<size=25>/" + rel_car.maxGears + "</size>";
        if(rel_car.destroyedCars > 0)
        {
            killsGO.GetComponentInChildren<Text>().text = rel_car.destroyedCars.ToString();
            killsGO.GetComponentInChildren<Image>().enabled = true;
        }
        else
        {
            killsGO.GetComponentInChildren<Text>().text = "";
            killsGO.GetComponentInChildren<Image>().enabled = false;
        }

        if(rel_car.destroyed)
        {
            destructionScreenGO.SetActive(true);

            destroyedCarGO.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("CarIcons/" + rel_car.carModel);
            if (rel_car.lastDamager)
                destroyerCarGO.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("CarIcons/" + rel_car.lastDamager.carModel);
            else
                destroyerCarGO.GetComponentInChildren<Image>().sprite = transparentSprite;
            switch(rel_car.lastDamageType)
            {
                case DamageType.DEBUFF_FIRE:
                    destructionType_fire.SetActive(true);
                    break;
                case DamageType.DEBUFF_ACID:
                    destructionType_acid.SetActive(true);
                    break;
                case DamageType.DEBUFF_DRAIN:
                    destructionType_leak.SetActive(true);
                    break;
                case DamageType.RAM:
                    destructionType_ram.SetActive(true);
                    break;
                case DamageType.DIRECT_DAMAGE:
                    destructionType_directDMG.SetActive(true);
                    break;
                case DamageType.PROJECTILE:
                    destructionType_projectile.SetActive(true);
                    break;
                case DamageType.MELEE:
                    destructionType_melee.SetActive(true);
                    break;
                case DamageType.EXPLOSION:
                    destructionType_explosion.SetActive(true);
                    break;
            }

            rel_car = null;
        }
    }

    public void onPauseScreen()
    {
        foreach (Text t in ModsGO.GetComponentsInChildren<Text>())
            t.enabled = true;
    }

    public void onPauseScreenEnd()
    {
        foreach (Text t in ModsGO.GetComponentsInChildren<Text>())
            t.enabled = false;
    }

    public void onModsChanged()
    {
        foreach (Image i in ModsGO.GetComponentsInChildren<Image>())
            DestroyImmediate(i.transform.parent.gameObject);

        foreach (CARnageModifier mod in rel_car.getModController().getMods())
        {
            GameObject go = Instantiate(ModIconIngame_Prefab, ModsGO.transform);
            go.GetComponentInChildren<Image>().sprite = mod.image;
            go.GetComponentInChildren<Text>().text = CARnageAuxiliary.colorMe(mod.description);
        }
    }
}

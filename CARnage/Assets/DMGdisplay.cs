using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DMGdisplay : MonoBehaviour {

    public GameObject number;
    public GameObject addIcon;
    public GameObject[] oner;
    public GameObject[] tener;
    public GameObject[] addIcons;
    public Material mat_Debuff_Fire;
    public Material mat_Debuff_Acid;
    public Material mat_Debuff_Drain;
    public Material mat_Explosion;
    public Material mat_Melee;
    public Material mat_Projectile;
    public Material mat_Ram;
    public Material mat_Repair;
    public Material mat_RepairShield;

    public void displayRepair(int pointsRepaired, bool isShieldRepair)
    {
        display(pointsRepaired, DamageType.DIRECT_DAMAGE, !isShieldRepair, isShieldRepair);
    }

    public void display(int damage, DamageType damageType)
    {
        display(damage, damageType, false, false);
    }

    void display (int damage, DamageType damageType, bool isRepair, bool isShieldRepair) {
        if (damage > 99)
            damage = 99;
        if (damage <= 0)
        {
            Destroy(gameObject);
            return;
        }
        transform.parent = null;

        int i_oner = damage % 10;
        int i_tener = damage / 10;

        oner[i_oner].SetActive(true);
        if(i_tener > 0)
            tener[i_tener].SetActive(true);

        switch(damageType)
        {
            case DamageType.DEBUFF_FIRE:
                oner[i_oner].GetComponent<MeshRenderer>().material = mat_Debuff_Fire;
                tener[i_tener].GetComponent<MeshRenderer>().material = mat_Debuff_Fire;
                addIcons[0].SetActive(true);
                break;
            case DamageType.DEBUFF_ACID:
                oner[i_oner].GetComponent<MeshRenderer>().material = mat_Debuff_Acid;
                tener[i_tener].GetComponent<MeshRenderer>().material = mat_Debuff_Acid;
                addIcons[1].SetActive(true);
                break;
            case DamageType.DEBUFF_DRAIN:
                oner[i_oner].GetComponent<MeshRenderer>().material = mat_Debuff_Drain;
                tener[i_tener].GetComponent<MeshRenderer>().material = mat_Debuff_Drain;
                addIcons[2].SetActive(true);
                break;
            case DamageType.EXPLOSION:
                oner[i_oner].GetComponent<MeshRenderer>().material = mat_Explosion;
                tener[i_tener].GetComponent<MeshRenderer>().material = mat_Explosion;
                break;
            case DamageType.MELEE:
                oner[i_oner].GetComponent<MeshRenderer>().material = mat_Melee;
                tener[i_tener].GetComponent<MeshRenderer>().material = mat_Melee;
                break;
            case DamageType.PROJECTILE:
                oner[i_oner].GetComponent<MeshRenderer>().material = mat_Projectile;
                tener[i_tener].GetComponent<MeshRenderer>().material = mat_Projectile;
                break;
            case DamageType.RAM:
                oner[i_oner].GetComponent<MeshRenderer>().material = mat_Ram;
                tener[i_tener].GetComponent<MeshRenderer>().material = mat_Ram;
                break;
        }
        if (isRepair)
        {
            oner[i_oner].GetComponent<MeshRenderer>().material = mat_Repair;
            tener[i_tener].GetComponent<MeshRenderer>().material = mat_Repair;
            addIcons[3].SetActive(true);
        }
        else if (isShieldRepair)
        {
            oner[i_oner].GetComponent<MeshRenderer>().material = mat_RepairShield;
            tener[i_tener].GetComponent<MeshRenderer>().material = mat_RepairShield;
            addIcons[4].SetActive(true);
        }
        
        number.GetComponent<Rigidbody>().AddForce(10 * transform.up + transform.right * Random.Range(-0.5f, 0.5f), ForceMode.Impulse);
        addIcon.GetComponent<Rigidbody>().AddForce(10 * transform.up + transform.right * Random.Range(-0.5f, 0.5f), ForceMode.Impulse);

        Destroy(gameObject, 5);
    }
}

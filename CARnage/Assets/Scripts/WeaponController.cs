using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour {

    public GameObject leftWeaponGO;
    public GameObject rightWeaponGO;
    public GameObject stashGO;

	// Use this for initialization
	void Start () {
        foreach (CARnageWeapon weapon in getAllWeapons())
            onWeaponObtained(weapon);
	}

    private void Update()
    {
        bool changeLeftWeapon = false;
        bool changeRightWeapon = false;
        if (GetComponentInParent<CARnageCar>().controlledBy == CARnageAuxiliary.ControllerType.MouseKeyboard && Input.GetKeyDown(KeyCode.Q))
            changeLeftWeapon = true;
        if (GetComponentInParent<CARnageCar>().controlledBy == CARnageAuxiliary.ControllerType.MouseKeyboard && Input.GetKeyDown(KeyCode.E))
            changeRightWeapon = true;

        if (changeLeftWeapon)
            changeWeapon(CARnageWeapon.WeaponState.EQUIPPED_LEFT);
        if (changeRightWeapon)
            changeWeapon(CARnageWeapon.WeaponState.EQUIPPED_RIGHT);
    }

    public void changeWeapon(CARnageWeapon.WeaponState side)
    {
        GameObject currentWeapon = null;
        if (side == CARnageWeapon.WeaponState.EQUIPPED_LEFT && getLeftWeapon() != null)
            currentWeapon = getLeftWeapon().gameObject;
        if (side == CARnageWeapon.WeaponState.EQUIPPED_RIGHT && getRightWeapon() != null)
            currentWeapon = getRightWeapon().gameObject;

        // 1st weapon from stash
        GameObject weaponToEquip = null;
        if(stashGO.GetComponentInChildren<CARnageWeapon>() != null)
            weaponToEquip = stashGO.GetComponentInChildren<CARnageWeapon>().gameObject;

        // or: swap L<->R
        if(weaponToEquip == null)
        {
            if (side == CARnageWeapon.WeaponState.EQUIPPED_LEFT && rightWeaponGO.GetComponentInChildren<CARnageWeapon>() != null)
                weaponToEquip = rightWeaponGO.GetComponentInChildren<CARnageWeapon>().gameObject;
            else if (side == CARnageWeapon.WeaponState.EQUIPPED_RIGHT && leftWeaponGO.GetComponentInChildren<CARnageWeapon>() != null)
                weaponToEquip = leftWeaponGO.GetComponentInChildren<CARnageWeapon>().gameObject;

            if(currentWeapon != null && weaponToEquip != null)
            {
                // swap
                equipWeapon(weaponToEquip, side);
                equipWeapon(currentWeapon, otherWeaponSide(side));
                return;
            }
        }
        
        if (weaponToEquip != null)
        {
            if (currentWeapon != null)
                equipWeapon(currentWeapon, CARnageWeapon.WeaponState.STASHED);
            equipWeapon(weaponToEquip, side);
        }
        else
        {
            // pushing around L<->R
            if (currentWeapon != null)
                equipWeapon(currentWeapon, otherWeaponSide(side));
        }
    }

    public CARnageWeapon.WeaponState otherWeaponSide(CARnageWeapon.WeaponState side)
    {
        if (side == CARnageWeapon.WeaponState.EQUIPPED_LEFT)
            return CARnageWeapon.WeaponState.EQUIPPED_RIGHT;
        if (side == CARnageWeapon.WeaponState.EQUIPPED_RIGHT)
            return CARnageWeapon.WeaponState.EQUIPPED_LEFT;

        return CARnageWeapon.WeaponState.STASHED;
    }

    public CARnageWeapon[] getAllWeapons()
    {
        return GetComponentsInChildren<CARnageWeapon>();
    }

    public CARnageWeapon getLeftWeapon()
    {
        return leftWeaponGO.GetComponentInChildren<CARnageWeapon>();
    }
    public CARnageWeapon getRightWeapon()
    {
        return rightWeaponGO.GetComponentInChildren<CARnageWeapon>();
    }

    public CARnageCar getCar()
    {
        return GetComponentInParent<CARnageCar>();
    }

    public void onWeaponObtained(CARnageWeapon weapon)
    {
        weapon.onObtained();
        getCar().onWeaponObtained(weapon);
    }

    public void obtainWeapon(GameObject weaponGO)
    {
        if (getLeftWeapon() == null)
            // equip in Left slot
            equipWeapon(weaponGO, CARnageWeapon.WeaponState.EQUIPPED_LEFT);
        else if (getRightWeapon() == null)
            // equip in right slot
            equipWeapon(weaponGO, CARnageWeapon.WeaponState.EQUIPPED_RIGHT);
        else
            // stash it
            equipWeapon(weaponGO, CARnageWeapon.WeaponState.STASHED);

        onWeaponObtained(weaponGO.GetComponent<CARnageWeapon>());
    }

    public void equipWeapon(GameObject weaponGO, CARnageWeapon.WeaponState side)
    {
        weaponGO.GetComponent<AudioSource>().Play();
        switch (side)
        {
            case CARnageWeapon.WeaponState.EQUIPPED_LEFT:
                weaponGO.transform.parent = leftWeaponGO.transform;
                weaponGO.GetComponent<CARnageWeapon>().WeaponGameObject.SetActive(true);
                break;
            case CARnageWeapon.WeaponState.EQUIPPED_RIGHT:
                weaponGO.transform.parent = rightWeaponGO.transform;
                weaponGO.GetComponent<CARnageWeapon>().WeaponGameObject.SetActive(true);
                break;
            case CARnageWeapon.WeaponState.STASHED:
                weaponGO.transform.parent = stashGO.transform;
                weaponGO.GetComponent<CARnageWeapon>().WeaponGameObject.SetActive(false);
                break;
        }
        weaponGO.transform.localPosition = Vector3.zero;
        weaponGO.GetComponent<CARnageWeapon>().weaponState = side;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModController : MonoBehaviour {

    public CARnageModifier[] getMods()
    {
        return GetComponentsInChildren<CARnageModifier>();
    }

    public void resetMods()
    {
        foreach (CARnageModifier mod in getMods())
            mod.resetMod();
    }

    public void onSpawn()
    {
        foreach (CARnageModifier mod in getMods())
            mod.onSpawn();
        resetMods();
        GlobalModifiers.registerModController(this);
    }

    public float getModifierPrice_Multiplier()
    {
        float mult = 1f;
        foreach (CARnageModifier mod in getMods())
            mult *= mod.getModifierPrice_Multiplier();
        return mult;
    }

    public float getSelfDMG_Multiplier(CARnageCar damager, DamageType damageType)
    {
        float mult = 1f;
        foreach (CARnageModifier mod in getMods())
            mult *= mod.getSelfDMG_Multiplier(damager, damageType);
        return mult;
    }

    public float getDMG_Multiplier(DamageType damageType, CARnageCar damagedCar)
    {
        float mult = 1f;
        foreach (CARnageModifier mod in getMods())
            mult *= mod.getDMG_Multiplier(damageType,damagedCar);
        return mult;
    }

    public bool getDebuffImmunity(CARnageCar.Debuff debuff)
    {
        bool check = false;
        foreach (CARnageModifier mod in getMods())
            if (mod.getDebuffImmunity(debuff))
                check = true;
        return check;
    }

    public float getBuildingDMG_Multiplier()
    {
        float mult = 1f;
        foreach (CARnageModifier mod in getMods())
            mult *= mod.getBuildingDMG_Multiplier();
        return mult;
    }

    public void onDestroyingCar(CARnageCar destroyedCar)
    {
        foreach (CARnageModifier mod in getMods())
            mod.onDestroyingCar(destroyedCar);
        GetComponentInParent<CARnageCar>().destroyedCars++;
    }

    public void onSelfDestroyed(CARnageCar killerCar)
    {
        foreach (CARnageModifier mod in getMods())
            mod.onSelfDestroyed(killerCar);
    }

    public void onShieldDestroyed()
    {
        foreach (CARnageModifier mod in getMods())
            mod.onShieldDestroyed();
    }

    public void onDMGReceived(float damage)
    {
        foreach (CARnageModifier mod in getMods())
            mod.onDMGReceived(damage);
    }

    public float getWeaponReloadTime_Multiplier()
    {
        float mult = 1f;
        foreach (CARnageModifier mod in getMods())
            mult *= mod.getWeaponReloadTime_Multiplier();
        return mult;
    }

    public void onDMGDealt(CARnageCar damagedCar, DamageType damageType, float amount)
    {
        foreach (CARnageModifier mod in getMods())
            mod.onDMGDealt(damagedCar, damageType, amount);
    }

    public float getShotDelay_Multiplier()
    {
        float mult = 1f;
        foreach (CARnageModifier mod in getMods())
            mult *= mod.getShotDelay_Multiplier();
        return mult;
    }

    public float getWeaponPrice_Multiplier()
    {
        float mult = 1f;
        foreach (CARnageModifier mod in getMods())
            mult *= mod.getWeaponPrice_Multiplier();
        return mult;
    }

    public float getRepairPrice_Multiplier()
    {
        float mult = 1f;
        foreach (CARnageModifier mod in getMods())
            mult *= mod.getRepairPrice_Multiplier();
        return mult;
    }

    public float getWaterContactDMG_Multiplier()
    {
        float mult = 1f;
        foreach (CARnageModifier mod in getMods())
            mult *= mod.getWaterContactDMG();
        return mult;
    }

    public void onReturnToLand()
    {
        foreach (CARnageModifier mod in getMods())
            mod.onReturnToLand();
    }

    public void onBuildingDestroyed()
    {
        foreach (CARnageModifier mod in getMods())
            mod.onBuildingDestroyed();
    }

    public bool isIgnoringEnemyShield()
    {
        bool check = false;
        foreach (CARnageModifier mod in getMods())
            if (mod.isIgnoringEnemyShield())
                check = true;
        return check;
    }

    public float getCollectedGears_Multiplier(CARnageModifier.GearSource gearSource)
    {
        float mult = 1f;
        foreach (CARnageModifier mod in getMods())
            mult *= mod.getCollectedGears_Multiplier(gearSource);
        return mult;
    }

    public float getDroppedGears_Multiplier()
    {
        float mult = 1f;
        foreach (CARnageModifier mod in getMods())
            mult *= mod.getDroppedGears_Multiplier();
        return mult;
    }

    public void onSecondPassed()
    {
        foreach (CARnageModifier mod in getMods())
            mod.onSecondPassed();
    }

    public void onGearCollected(int amount)
    {
        foreach (CARnageModifier mod in getMods())
            mod.onGearCollected(amount);
    }

    public void onWeaponObtained(CARnageWeapon weapon)
    {
        foreach (CARnageModifier mod in getMods())
            mod.onWeaponObtained(weapon);
    }

    public void onWeaponDropped(CARnageWeapon weapon)
    {
        foreach (CARnageModifier mod in getMods())
            mod.onWeaponDropped(weapon);
    }

    public float getNitroConsumption_Multiplier()
    {
        float mult = 1f;
        foreach (CARnageModifier mod in getMods())
            mult *= mod.getNitroConsumption_Multiplier();
        return mult;
    }

    public float getNitroRegeneration_Multiplier()
    {
        float mult = 1f;
        foreach (CARnageModifier mod in getMods())
            mult *= mod.getNitroRegeneration_Multiplier();
        return mult;
    }

    public float getWeaponMagazine_Multiplier()
    {
        float mult = 1f;
        foreach (CARnageModifier mod in getMods())
            mult *= mod.getWeaponMagazine_Multiplier();
        return mult;
    }

    public CARnageModifier getRandomMod()
    {
        if (getMods().Length == 0)
            return null;
        return getMods()[Random.Range(0, getMods().Length)];
    }

    public void onProjectileShot(ProjectileTrajectory projectile)
    {
        foreach (CARnageModifier mod in getMods())
            mod.onProjectileShot(projectile);
    }

    public void onModifierBought(CARnageModifier modifier)
    {
        foreach (CARnageModifier mod in getMods())
            mod.onModifierBought(modifier);
    }

    public void onShopEntered()
    {
        foreach (CARnageModifier mod in getMods())
            mod.onShopEntered();
    }

    public float getShieldRegeneration_Multiplier()
    {
        float mult = 1f;
        foreach (CARnageModifier mod in getMods())
            mult *= mod.getShieldRegeneration_Multiplier();
        return mult;
    }

    public float getShieldRegenerationOnset_Multiplier()
    {
        float mult = 1f;
        foreach (CARnageModifier mod in getMods())
            mult *= mod.getShieldRegenerationOnset_Multiplier();
        return mult;
    }

    public bool isEmpty()
    {
        if (getMods().Length == 0)
            return true;
        return false;
    }

    public bool isReflectingProjectiles()
    {
        foreach (CARnageModifier mod in getMods())
            if (mod.isReflectingProjectiles())
                return true;
        return false;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CARnageModifier : MonoBehaviour {

    public ModID modID;
    public string modName;
    public string description;
    public Sprite image;

    bool used = false;
    public void resetMod()
    {
        used = false;
    }

    bool finaleActive = false;
    CARnageCar killWill_Enemy;
    // ----- ------ ----- ----- -----
    // ----- ----- ON-HANDLER ----- -----
    // ----- ------ ----- ----- -----

    public void onSpawn()
    {
        if(modID == ModID.TRAFFIC_BYPASSER && !used)
        {
            getCar().transform.localScale = getCar().transform.localScale * 0.5f;
            used = true;
        }

        if(modID == ModID.HEAVY_LOAD && !used)
        {
            getCar().maxGears *= 6;
            used = true;
        }

        if (modID == ModID.MOONWALK && !used)
        {
            Physics.gravity = Physics.gravity * 0.5f;
            used = true;
        }

        if (modID == ModID.COLOSSUS && !used)
        {
            getCar().transform.localScale = getCar().transform.localScale * 2f;
            getCar().maxHP *= 2;
            getCar().currentHP *= 2;
            getCar().updateValues();
            used = true;
        }

        if(modID == ModID.COMMITMENT)
            getCar().replenishShield(getCar().maxHP * 0.5f);
        
        if(modID == ModID.SCREWED_DOWN)
        {
            getCar().replenishShield(getCar().maxHP * 0.99f);
            getCar().maxHP *= 0.01f;
            getCar().currentHP = getCar().maxHP;
            getCar().updateValues();
        }
        
    }

    public void onSecondPassed()
    {
        if (modID == ModID.RED_DEATH)
            if (getCar().currentHP / getCar().maxHP >= 0.5f)
                getCar().repair(1);
            else
                getCar().damageMe(1);

        if (modID == ModID.GROWTH)
            getCar().addGears(1);

        if (modID == ModID.UPWIND && GetComponent<RCC_CarControllerV3>().isInAir)
            getCar().repair(10);
        if (modID == ModID.AIRBORNE && GetComponent<RCC_CarControllerV3>().isInAir)
            getCar().replenishShield(10);
    }

    public void onPickupWeapon(CARnageWeapon weapon)
    {
        if (modID == ModID.IMPROVISE)
            weapon.addRandomUpgrade();
    }

    public void onDestroyingCar(CARnageCar destroyedCar)
    {
        if (modID == ModID.GUN_RACK)
            CARnageAuxiliary.spawnRndWeapon(destroyedCar.transform.position);

        if (modID == ModID.LIFT)
            getCar().replenishShield(getCar().maxHP / 4);
        if (modID == ModID.SPARE_PARTS)
            getCar().repair(getCar().maxHP / 4);

    }

    public void onSelfDestroyed(CARnageCar killerCar)
    {
        if (modID == ModID.LAST_TRIP)
            killerCar.applyDebuff(CARnageCar.Debuff.Acid, getCar(), 2);

        if (modID == ModID.KILL_WILL)
            killWill_Enemy = killerCar;

        if (modID == ModID.MARTYRDOM)
            getCar().addGears(1000);
    }

    public void onShieldDestroyed()
    {
        if (modID == ModID.TASTY_BURGER && !used)
        {
            getCar().replenishShield();
            used = true;
        }
        if (modID == ModID.FINALE)
        {
            finaleActive = true;
            Invoke("disableFinaleActive", 10);
        }
    }

    public void onDMGReceived(float damage)
    {
        if (modID == ModID.DOWNFALL && getCar().currentHP <= getCar().maxHP / 4 && !used)
            CARnageAuxiliary.spawnWeapon(CARnageWeapon.WeaponModel.DOWNFALL, getCar().transform.position);
    }

    public void onDMGDealt(CARnageCar damagedCar, DamageType damageType, float amount)
    {
        if (modID == ModID.HYDROFLUORIC_ACID)
            damagedCar.applyDebuff(CARnageCar.Debuff.Acid, getCar());

        if (modID == ModID.DISASSEMBLY)
            damagedCar.dropGears(1);

        switch (damageType)
        {
            case DamageType.PROJECTILE:
                if (modID == ModID.LOCK)
                    getCar().applyDebuff(CARnageCar.Debuff.Locked, getCar());
                break;

            case DamageType.DEBUFF:
                if (modID == ModID.BLACK_THUMB)
                    getCar().repair(1);
                if (modID == ModID.INCINERATOR)
                    getCar().replenishNitro(10);
                break;

            case DamageType.MELEE:
                if (modID == ModID.BB_BBQ)
                    getCar().replenishShield(amount * 0.1f);
                if (modID == ModID.FORCED_EXTRACTION)
                    damagedCar.dropWeapon();
                break;

            case DamageType.RAM:
                if (modID == ModID.BLIZZARD)
                    damagedCar.applyDebuff(CARnageCar.Debuff.Freeze, getCar());
                if (modID == ModID.OILWAY)
                    damagedCar.applyDebuff(CARnageCar.Debuff.Drain, getCar(), 2);
                //if (modID == ModID.INCINERATOR && getCar().isOnFire()) // Deprecated
                //    damagedCar.applyDebuff(CARnageCar.Debuff.Fire, getCar());
                if (modID == ModID.IMPULSIVE_DEFLORATION && damagedCar.isShielded())
                    damagedCar.breakShield();
                break;
        }
    }

    public void onBuildingDestroyed()
    {
        if (modID == ModID.DEMOLITION)
            getCar().repair();
    }

    public void onGearCollected(int amount)
    {
        if (modID == ModID.SURGEONEER)
            getCar().repair(amount);
        if (modID == ModID.CROP)
            getCar().replenishShield(amount);
    }

    public void onWeaponObtained(CARnageWeapon weapon)
    {
        //if(modID == ModID.BIG_FAT_KILL && )
    }
    // ----- ------ ----- ----- -----
    // ----- ----- DMG RELATED ----- -----
    // ----- ------ ----- ----- -----

    public float getSelfDMG_Multiplier(CARnageCar damager, DamageType damageType)
    {
        float mult = 1f;

        if (modID == ModID.IRON_SHELTER && getCar().isShielded())
            mult *= 0.5f;

        switch (damageType)
        {
            // ----- ----- ----- PROJECTILE ----- ----- ----- 
            case DamageType.PROJECTILE:
                if (modID == ModID.BULLETPROOF && getCar().isShielded())
                    mult = 0f;
                break;

            // ----- ----- ----- EXPLOSION ----- ----- ----- 
            case DamageType.EXPLOSION:
                if (modID == ModID.RECKLESS)
                    mult = 0f;
                break;

            // ----- ----- ----- RAMMING ----- ----- ----- 
            case DamageType.RAM:
                if (modID == ModID.DEATH_PROOF)
                    mult = 0f;
                break;

            // ----- ----- ----- DEBUFFS ----- ----- ----- 
            case DamageType.DEBUFF:
                if (modID == ModID.FURNACED)
                {
                    mult = 0f;
                    getCar().repair(2);
                }
                break;
        }

        return mult;
    }

    public float getDMG_Multiplier(DamageType damageType, CARnageCar damagedCar)
    {
        float mult = 1f;
        if (modID == ModID.RISK_LOVER)
            mult += 1 - (getCar().currentHP / getCar().maxHP);

        if (modID == ModID.KILL_WILL && damagedCar == killWill_Enemy)
            mult += 0.3f;

        if (modID == ModID.SINNER)
            mult += getCar().destroyedCars / 100f;

        if (modID == ModID.BUSINESS_CARD && damagedCar.carColor != CARnageCar.CarColor.WHITE)
            mult += 0.1f;

        if (modID == ModID.AGGRESSIVE_STEREOTYPES && damagedCar.carColor != getCar().carColor)
            mult += 0.1f;

        if (modID == ModID.UTTERLY_INSANE && getCar().isOnNitro())
            mult += 0.2f;

        switch (damageType)
        {
            // ----- ----- ----- PROJECTILE ----- ----- ----- 
            case DamageType.PROJECTILE:
                if (modID == ModID.DRIFT_KING && GetComponent<RCC_CarControllerV3>().isDrifting)
                    mult += 0.2f;

                if (modID == ModID.DOUBLE_DARE)
                {
                    CARnageWeapon[] weapons = getCar().GetComponentsInChildren<CARnageWeapon>();
                    if (weapons.Length == 2)
                        if (weapons[0].weaponModel == weapons[1].weaponModel)
                            mult += 0.3f;
                }

                if (modID == ModID.BAD_ATTITUDE && getCar().currentHP >= getCar().maxHP)
                    mult += 0.3f;
                break;

            // ----- ----- ----- RAM ----- ----- ----- 
            case DamageType.RAM:
                if (modID == ModID.CONCRETE_FRONT)
                    mult += GetComponentInParent<RCC_CarControllerV3>().speed / 100f;
                if (modID == ModID.COWCATCHER && getCar().isShielded())
                    mult += 0.3f;
                if (modID == ModID.THRESH)
                    mult += getCar().currentShield / Mathf.Max(getCar().maxHP,getCar().maxShield);
                if (modID == ModID.FINISHER && damagedCar.currentHP / damagedCar.maxHP <= 0.25f)
                    mult *= 100f;
                if (modID == ModID.FINALE && finaleActive)
                    mult *= 100f;
                break;

            // ----- ----- ----- MELEE ----- ----- ----- 
            case DamageType.MELEE:
                if (modID == ModID.BARE_KNUCKLES)
                    mult += 0.3f;
                break;
        }

        return mult;
    }

    public float getBuildingDMG_Multiplier()
    {
        float damage = 1f;
        if (modID == ModID.TEARDOWN)
            damage += 5;

        return damage;
    }

    public float getWeaponReloadTime_Multiplier()
    {
        float reloadTime = 1f;
        if (modID == ModID.IMPATIENCE)
            reloadTime -= 0.5f;

        return reloadTime;
    }

    public float getShotDelay_Multiplier()
    {
        float mult = 1f;
        if (modID == ModID.RAPID_RUSH)
            mult -= (GetComponent<RCC_CarControllerV3>().speed / 200f);

        return mult;
    }
    // ----- ------ ----- ----- -----
    // ----- ----- PRICE/GEARS ----- -----
    // ----- ------ ----- ----- -----
    public float getModifierPrice_Multiplier()
    {
        if (modID == ModID.CAR_IMPROVEMENT)
            return 0.5f;
        return 1f;
    }
    // when purchasing a Weapon, this will be multiplied with the price
    public float getWeaponPrice_Multiplier()
    {
        float mult = 1f;
        if (modID == ModID.BEST_CUSTOMER)
            mult *= 0.5f;
        if (modID == ModID.MERCHANT_OF_DEATH)
            mult *= 1 - Mathf.Max(1, getCar().destroyedCars / 100f);

        return mult;
    }
    // when repairing, this will be multiplied with the price
    public float getRepairPrice_Multiplier()
    {
        if (modID == ModID.CONSTRUCTION_WORKER)
            return 0.5f;
        return 1f;
    }

    public float getCollectedGears_Multiplier(GearSource gearSource)
    {
        float mult = 1f;
        if (modID == ModID.GOLD_DIGGER && gearSource == GearSource.ENVIRONMENT)
            mult *= 2;
        if (modID == ModID.WELL__PLACED_ADVERTISEMENT && getCar().isShielded())
            mult *= 2;
        if (modID == ModID.HARVEST && gearSource == GearSource.CAR)
            mult *= 2;

        return mult;
    }

    public float getDroppedGears_Multiplier()
    {
        float mult = 1f;
        if (modID == ModID.APPOINTMENT)
            mult *= 0.5f;

        return mult;
    }

    public float getNitroConsumption_Multiplier()
    {
        float mult = 1f;
        if (modID == ModID.GASWORKS)
            mult *= 0.5f;
        return mult;
    }
    public float getNitroRegeneration_Multiplier()
    {
        float mult = 1f;
        if (modID == ModID.STUFFED && getCar().isShielded())
            mult *= 2f;
        return mult;
    }
    // ----- ------ ----- ----- -----
    // ----- ----- IMMUNITIES---
    // ----- ------ ----- ----- -----

    public bool getDebuffImmunity(CARnageCar.Debuff debuff)
    {
        if (modID == ModID.HEROIC)
            return true;

        return false;
    }

    public float getWaterContactDMG()
    {
        float damage = 1f;
        if (modID == ModID.SURFIN_BIRD)
        {
            damage = 0f;
            // TODO: Remodel as Boat
        }
        return damage;
    }
    public void onReturnToLand()
    {
        if (modID == ModID.SURFIN_BIRD)
        {
            // TODO: Remodel back as Car
        }
    }

    public bool isIgnoringEnemyShield()
    {
        if (modID == ModID.STEAMROLLER)
            return true;
        return false;
    }

    // ----- ------ ----- ----- -----
    // ----- ----- HELPER FUNCTIONS ---
    // ----- ------ ----- ----- -----

    void disableFinaleActive()
    {
        finaleActive = false;
    }

    public enum GearSource
    {
        ENVIRONMENT,
        CAR
    }

    public CARnageCar getCar()
    {
        return GetComponentInParent<CARnageCar>();
    }

    public enum ModID
    {
        CAR_IMPROVEMENT, //
        IMPROVISE, //
        DEJA_VU,// TODO: Bases
        DRIFT_KING, //
        GUN_RACK, // TODO: spawnRndWeapon
        TASTY_BURGER, //
        DOUBLE_DARE, //
        DOWNFALL, //
        IMPATIENCE, //
        TRAILER_THRASH, // TODO: Trailer Thrash
        BARE_KNUCKLES, //
        HYDROFLUORIC_ACID, //
        RECKLESS, // 
        BEST_CUSTOMER, //
        TRAFFIC_BYPASSER, //
        SURFIN_BIRD, // TODO: Remodel as boat
        VACATIONIST, // TODO: Bases
        RISK_LOVER, //
        LEAD__FOOTED, // TODO: See if speed can be manipulated at run-time
        LAST_TRIP, //
        SOLITUDE, // TODO: Check if any cars/buildings around
        HEAVY_LOAD, //
        DISASSEMBLY, //
        DEMOLITION, //
        STEAMROLLER, //
        CONCRETE_FRONT, //
        CONSTRUCTION_WORKER, //
        TEARDOWN, //
        GOLD_DIGGER, //
        POLICE_OPPRESSION, // TODO: Weapons belonging to Cars
        CUT_OFF_THE_GAS, // TODO: Mods belonging to Cars
        SURVEILLANCE, // TODO: Mods belonging to Cars
        UNDERCOVER, // TODO: Hide
        RED_DEATH, //
        SURGEONEER, //
        BLIZZARD, // TODO: Freeze FX / Animation / engine blockage
        WELL__PLACED_ADVERTISEMENT, //
        COD, // TODO: Dropping Modifiers
        MERCHANT_OF_DEATH, // 
        DELUSIONS_OF_GRANDEUR, //TODO: Weapons belonging to Cars
        MOONWALK, //
        CHALLENGER, //TODO: Explosions
        SOLDIER_OF_FORTUNE, // TODO: Everything
        BAD_ATTITUDE, //
        RAPID_RUSH, //
        FINAL_MESSAGE, // TODO: Bases
        COWCATCHER, //
        RAPID_PROTOTYPING, // TODO: Mods belonging to Cars
        KILL_WILL, //
        UNEARTH, // TODO: Destroying Cars
        COLOSSUS, //
        BULLETPROOF, //
        LIFT, // TODO: Visual appearance @Towboy
        HOOKED, // TODO: Obi Rope
        LOCK, //
        STOCK, // TODO: Shops
        BLACK_THUMB, //
        IRON_SHELTER, //
        HEROIC, //
        COMMITMENT, //
        STUFFED, // TODO: Nitro
        BB_BBQ, //
        GROWTH, //
        HARVEST, // 
        DEATH_PROOF, //
        FINISHER, //
        TREACHEROUS_FLAVOR, // TODO: Bases
        FORCED_EXTRACTION, // TODO: Drop Weapon
        SINNER, //
        BIG_FAT_KILL, //TODO: on Weapon equipped
        INTUITION, // TODO: Reflection
        GUN_FU, // TODO: Everything
        HALO, // TODO: Everything
        D6, // TODO: Obtain Mods
        RUBBERNECK, // TODO: Everything
        POLICE_TRANSMITTER, // TODO: Map
        FINALE, //
        DESTINATION, // TODO: Bases
        BUSINESS_CARD, // 
        CROP, //
        THRESH, //
        DETONATION, // TODO: Explosions
        OILWAY, //
        SPARE_PARTS, //
        ENDLESS_SUPPLY, // TODO: On change Weapon
        SCREWED_DOWN, //
        BULLETSTORM, // TODO: Everything
        CANNONBALLS, // TODO: On change weapon, projectile size modifier
        MUZZLE__LOADER, // TODO: Weapons, global variables
        HELL_SHELL, // TODO: projectile modifier
        INCINERATOR, //
        FURNACED, //
        IMPULSIVE_DEFLORATION, //
        INERTIA, //TODO: Weapons, global variables
        AGGRESSIVE_STEREOTYPES, //
        MARTYRDOM, // TODO: Weapons (no)
        UPWIND, //
        AIRBORNE, //
        APPOINTMENT, //
        GASWORKS, //
        UTTERLY_INSANE //
    }
}

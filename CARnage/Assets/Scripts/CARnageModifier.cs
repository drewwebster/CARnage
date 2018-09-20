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

    public void deleteMe()
    {
        onDeleted();
        Destroy(gameObject);
    }

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
            getCar().transform.localScale = getCar().transform.localScale * 1.5f;
            getCar().maxHP *= 1.5f;
            getCar().currentHP *= 1.5f;
            getCar().updateValues();
            used = true;
        }
        
        if(modID == ModID.SCREWED_DOWN && !used)
        {
            getCar().replenishShield(getCar().maxHP * 0.99f);
            getCar().maxHP *= 0.01f;
            getCar().currentHP = getCar().maxHP;
            getCar().updateValues();
            used = true;
        }

        if(modID == ModID.D6 && !used)
        {
            ModFactory.spawnRandomMod(getCar());
            ModFactory.spawnRandomMod(getCar());
            used = true;
        }

        if (modID == ModID.BIG_FAT_KILL)
        {
            foreach (CARnageWeapon weapon in getCar().getWeaponController().getAllWeapons())
                if (weapon.damageType == DamageType.MELEE)
                    weapon.transform.localScale *= 2;
        }

    }

    public void onDeleted()
    {
        if (modID == ModID.TRAFFIC_BYPASSER)
            getCar().transform.localScale = getCar().transform.localScale * 2f;

        if (modID == ModID.HEAVY_LOAD)
        {
            getCar().maxGears /= 6;
            getCar().currentGears = Math.Min(getCar().currentGears, getCar().maxGears);
        }
        if (modID == ModID.MOONWALK)
            Physics.gravity = Physics.gravity * 2f;

        if (modID == ModID.COLOSSUS)
        {
            getCar().transform.localScale /= 1.5f;
            getCar().maxHP /= 1.5f;
            getCar().currentHP = Mathf.Min(getCar().currentHP, getCar().maxHP);
            getCar().updateValues();
        }
        if (modID == ModID.SCREWED_DOWN)
        {
            getCar().maxShield -= getCar().maxHP * 100;
            getCar().currentShield = Mathf.Min(getCar().currentShield, getCar().maxShield);
            getCar().maxHP *= 100;
            getCar().updateValues();
        }
        if (modID == ModID.BIG_FAT_KILL)
        {
            foreach (CARnageWeapon weapon in getCar().getWeaponController().getAllWeapons())
                if (weapon.damageType == DamageType.MELEE)
                    weapon.transform.localScale /= 2;
        }
    }

    public void onSecondPassed()
    {
        if (modID == ModID.RED_DEATH)
            if (getCar().currentHP / getCar().maxHP >= 0.5f)
                getCar().repair(2f / 100f * getCar().maxHP);
            else
                getCar().damageMe(2f / 100f * getCar().maxHP, getCar(),DamageType.DIRECT_DAMAGE);

        if (modID == ModID.GROWTH)
            Gear.spawnGears(1, getCar(),GearSource.OTHER, getCar());

        if (modID == ModID.UPWIND && getCar().GetComponent<RCC_CarControllerV3>().isInAir)
            getCar().repair(10);
        if (modID == ModID.AIRBORNE && getCar().GetComponent<RCC_CarControllerV3>().isInAir)
            getCar().replenishShield(10);
    }
    
    public void onDestroyingCar(CARnageCar destroyedCar)
    {
        if (modID == ModID.GUN_RACK)
            WeaponFactory.spawnRndWeapon(destroyedCar.transform.position);

        if (modID == ModID.LIFT)
            getCar().replenishShield(getCar().maxHP / 4);
        if (modID == ModID.SPARE_PARTS)
            getCar().repair(getCar().maxHP / 4);

        if (modID == ModID.CUT_OFF_THE_GAS && destroyedCar.getModController().getRandomMod() != null)
            destroyedCar.getModController().getRandomMod().deleteMe();

        if(modID == ModID.SURVEILLANCE)
        {
            if (destroyedCar.getModController().isEmpty())
                return;
            ModID copyMod = destroyedCar.getModController().getRandomMod().modID;
            ModFactory.spawnMod(copyMod, getCar());
        }

    }

    public void onSelfDestroyed(CARnageCar killerCar)
    {
        if (killerCar == null)
            return;

        if (modID == ModID.LAST_TRIP)
            killerCar.applyDebuff(CARnageCar.Debuff.Acid, getCar(), 2);

        if (modID == ModID.KILL_WILL)
            killWill_Enemy = killerCar;

        if (modID == ModID.MARTYRDOM && getCar().getWeaponController().getAllWeapons().Length == 0)
            getCar().addGears(1000,GearSource.OTHER);

        if(modID == ModID.DETONATION)
        {
            GameObject go = Instantiate(Resources.Load<GameObject>("COMMON_EXPLOSION"), getCar().transform);
            go.GetComponentInChildren<explosionHitbox>().rel_car = getCar();
        }
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
            WeaponFactory.spawnWeapon(CARnageWeapon.WeaponModel.DOWNFALL, getCar());
    }

    float ramDMGTriggerThreshold = 10;
    public void onDMGDealt(CARnageCar damagedCar, DamageType damageType, float amount)
    {
        if (modID == ModID.HYDROFLUORIC_ACID && damageType != DamageType.DEBUFF_ACID)
            damagedCar.applyDebuff(CARnageCar.Debuff.Acid, getCar());

        if (modID == ModID.DISASSEMBLY)
            damagedCar.dropGears(1, getCar());

        switch (damageType)
        {
            case DamageType.PROJECTILE:
                if (modID == ModID.LOCK)
                    getCar().applyDebuff(CARnageCar.Debuff.Locked, getCar());
                break;

            case DamageType.DEBUFF_FIRE:
            case DamageType.DEBUFF_ACID:
            case DamageType.DEBUFF_DRAIN:
                if (modID == ModID.BLACK_THUMB)
                    getCar().repair(2);
                if (modID == ModID.INCINERATOR)
                    getCar().replenishNitro(10);
                break;

            case DamageType.MELEE:
                if (modID == ModID.BB_BBQ)
                    getCar().replenishShield(amount * 0.1f);
                if (modID == ModID.FORCED_EXTRACTION)
                    damagedCar.getWeaponController().dropRandomEquippedWeapon();
                break;

            case DamageType.RAM:
                if (amount < ramDMGTriggerThreshold)
                    return;
                
                if (modID == ModID.BLIZZARD)
                    damagedCar.applyDebuff(CARnageCar.Debuff.Freeze, getCar());
                if (modID == ModID.OILWAY)
                    damagedCar.applyDebuff(CARnageCar.Debuff.Drain, getCar(), 2);
                if(modID == ModID.POLICE_OPPRESSION)
                {
                    CARnageWeapon stolenWeapon = damagedCar.getWeaponController().dropRandomWeapon();
                    getCar().getWeaponController().obtainWeapon(stolenWeapon.gameObject);
                }
                if(modID == ModID.CONCRETE_FRONT)
                {
                    Vector3 knockbackDirection = getCar().transform.forward * 50000;
                    damagedCar.GetComponent<Rigidbody>().AddForce(knockbackDirection, ForceMode.Impulse);
                }
                if (modID == ModID.CHALLENGER)
                {
                    GameObject go = Instantiate(Resources.Load<GameObject>("COMMON_EXPLOSION"));
                    go.transform.position = getCar().transform.position;
                    go.GetComponentInChildren<explosionHitbox>().rel_car = getCar();
                }
                break;
        }
    }

    public void onBuildingDestroyed()
    {
        if (modID == ModID.DEMOLITION)
            getCar().repair(10);
    }

    public void onGearCollected(int amount)
    {
        if (modID == ModID.SURGEONEER)
            getCar().repair(amount*2);
        if (modID == ModID.CROP)
            getCar().replenishShield(amount);
    }

    public void onWeaponObtained(CARnageWeapon weapon)
    {
        //Debug.Log("on weapon obtained: " + weapon.weaponModel.ToString());
        if (modID == ModID.IMPROVISE)
            weapon.addRandomUpgrade();
        if (modID == ModID.BIG_FAT_KILL && weapon.damageType == DamageType.MELEE)
            weapon.transform.localScale *= 1.5f;
    }

    public void onWeaponDropped(CARnageWeapon weapon)
    {
        if (modID == ModID.BIG_FAT_KILL && weapon.damageType == DamageType.MELEE)
            weapon.transform.localScale /= 1.5f;
    }

    public void onProjectileShot(ProjectileTrajectory projectile)
    {
        if(modID == ModID.CANNONBALLS)
            projectile.transform.localScale *= 2;
        if (modID == ModID.BULLETSTORM && getCar().isShielded() && UnityEngine.Random.Range(0, 100f) > 30f)
            projectile.rel_weapon.Invoke("shootProjectile", projectile.rel_weapon.shotDelay/2);
    }

    public void onModifierBought(CARnageModifier modifier)
    {
        if(modID == ModID.RAPID_PROTOTYPING)
        {
            List<CARnageModifier> toDelete = new List<CARnageModifier>();
            foreach (CARnageModifier mod in getCar().getModController().getMods())
                if (mod.modID != ModID.RAPID_PROTOTYPING)
                    toDelete.Add(mod);
            int i = 1; // +1 from rapid prototyping bonus
            foreach(CARnageModifier mod in toDelete)
            {
                mod.deleteMe();
                i++;
            }
            for (int j = 0; j < i; j++)
                ModFactory.spawnRandomMod(getCar());
        }
    }

    public void onShopEntered()
    {
        if(modID == ModID.COD && !used)
        {
            ModFactory.spawnRandomMod(getCar());
            ModFactory.spawnRandomMod(getCar());
            WeaponFactory.spawnRndWeapon(getCar());
            used = true;
        }
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
                if (modID == ModID.DEATH_PROOF && getCar().isShielded())
                    mult = 0f;
                break;

            // ----- ----- ----- DEBUFFS ----- ----- ----- 
            case DamageType.DEBUFF_FIRE:
            case DamageType.DEBUFF_ACID:
            case DamageType.DEBUFF_DRAIN:
                if (modID == ModID.HEROIC)
                    mult = 0f;
                if (modID == ModID.FURNACED && getCar().fireTicks > 0)
                {
                    mult = 0f;
                    getCar().repair(2,true);
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

        if (modID == ModID.DELUSIONS_OF_GRANDEUR)
            mult += 0.05f * getCar().getWeaponController().getAllWeapons().Length;

        switch (damageType)
        {
            // ----- ----- ----- PROJECTILE ----- ----- ----- 
            case DamageType.PROJECTILE:
                if (modID == ModID.DRIFT_KING && getCar().GetComponent<RCC_CarControllerV3>().isDrifting)
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
                //if (modID == ModID.CONCRETE_FRONT)
                //    mult += GetComponentInParent<RCC_CarControllerV3>().speed / 100f; // DEPRECATED
                if (modID == ModID.COWCATCHER && getCar().isShielded())
                    mult += 0.3f;
                if (modID == ModID.THRESH)
                    mult += getCar().currentShield / 100;
                if (modID == ModID.FINISHER && damagedCar.currentHP / damagedCar.maxHP <= 0.25f)
                    mult *= 1000f;
                if (modID == ModID.FINALE && finaleActive)
                    mult *= 1000f;
                if (modID == ModID.IMPULSIVE_DEFLORATION && damagedCar.isShielded())
                    mult *= 1000f;
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
        {
            // 0 ... 200 km/h to 1 ... 1/4 shotDelay
            float speed = Mathf.Clamp(getCar().GetComponent<RCC_CarControllerV3>().speed, 0, 200);
            mult = 1/(((speed / 200) * 3) + 1);
        }

        return mult;
    }

    public float getWeaponMagazine_Multiplier()
    {
        float mult = 1f;
        if (modID == ModID.ENDLESS_SUPPLY)
            mult *= 2;
        return mult;
    }

    public float getShieldRegeneration_Multiplier()
    {
        float mult = 1f;
        if (modID == ModID.COMMITMENT)
            mult *= 2;
        return mult;
    }

    public float getShieldRegenerationOnset_Multiplier()
    {
        float mult = 1f;
        if (modID == ModID.COMMITMENT)
            mult *= 0.5f;
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
        if (modID == ModID.LEAD__FOOTED)
            mult -= 1-(getCar().currentHP / getCar().maxHP);
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

    public bool isReflectingProjectiles()
    {
        if (modID == ModID.INTUITION && getCar().isOnNitro())
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
        CAR,
        OTHER
    }

    public CARnageCar getCar()
    {
        return GetComponentInParent<CARnageCar>();
    }

    public enum ModID
    {
        CAR_IMPROVEMENT, //
        IMPROVISE, // works
        DEJA_VU,// TODO: Bases
        DRIFT_KING, // works
        GUN_RACK, // works
        TASTY_BURGER, // works
        DOUBLE_DARE, // works
        DOWNFALL, // works
        IMPATIENCE, // works
        TRAILER_THRASH, // TODO: Trailer Thrash
        BARE_KNUCKLES, // works
        HYDROFLUORIC_ACID, // works
        RECKLESS, // works
        BEST_CUSTOMER, //
        TRAFFIC_BYPASSER, // works
        SURFIN_BIRD, // TODO: Remodel as boat
        VACATIONIST, // TODO: Bases
        RISK_LOVER, // works
        LEAD__FOOTED, // works
        LAST_TRIP, // works
        SOLITUDE, // TODO: Check if any cars/buildings around
        HEAVY_LOAD, // works
        DISASSEMBLY, // works
        DEMOLITION, //
        STEAMROLLER, // works
        CONCRETE_FRONT, // works
        CONSTRUCTION_WORKER, //
        TEARDOWN, //
        GOLD_DIGGER, //
        POLICE_OPPRESSION, // works
        CUT_OFF_THE_GAS, // works
        SURVEILLANCE, // works
        UNDERCOVER, // TODO: Hide
        RED_DEATH, // works
        SURGEONEER, // works
        BLIZZARD, // TODO: Freeze FX / Animation / engine blockage
        WELL__PLACED_ADVERTISEMENT, // works
        COD, //
        MERCHANT_OF_DEATH, // 
        DELUSIONS_OF_GRANDEUR, // works
        MOONWALK, // works
        CHALLENGER, //TODO: Explosions
        SOLDIER_OF_FORTUNE, // TODO: Everything
        BAD_ATTITUDE, // works
        RAPID_RUSH, // works
        FINAL_MESSAGE, // TODO: Bases
        COWCATCHER, // works
        RAPID_PROTOTYPING, //
        KILL_WILL, //
        UNEARTH, // TODO: resurrecting Cars
        COLOSSUS, // works
        BULLETPROOF, // works
        LIFT, // works, TODO: Visual appearance @Towboy
        HOOKED, // TODO: Obi Rope
        LOCK, //
        STOCK, // TODO: Shops
        BLACK_THUMB, // works
        IRON_SHELTER, // works
        HEROIC, // works
        COMMITMENT, // works
        STUFFED, // works
        BB_BBQ, // works
        GROWTH, // works
        HARVEST, // works
        DEATH_PROOF, // works
        FINISHER, // works
        TREACHEROUS_FLAVOR, // TODO: Bases
        FORCED_EXTRACTION, // works
        SINNER, // works
        BIG_FAT_KILL, // works
        INTUITION, // works
        GUN_FU, // TODO: Everything
        HALO, // TODO: Everything
        D6, // works
        RUBBERNECK, // TODO: Everything
        POLICE_TRANSMITTER, // TODO: Map
        FINALE, // works
        DESTINATION, // TODO: Bases
        BUSINESS_CARD, // works
        CROP, // works
        THRESH, // works
        DETONATION, // works
        OILWAY, // works
        SPARE_PARTS, // works
        ENDLESS_SUPPLY, // works
        SCREWED_DOWN, // works
        BULLETSTORM, // works
        CANNONBALLS, // works
        MUZZLE__LOADER, // works
        HELL_SHELL, // TODO: projectile modifier
        INCINERATOR, // works
        FURNACED, // works
        IMPULSIVE_DEFLORATION, // works
        INERTIA, // works
        AGGRESSIVE_STEREOTYPES, // works
        MARTYRDOM, //
        UPWIND, // works
        AIRBORNE, // works
        APPOINTMENT, //
        GASWORKS, // works
        UTTERLY_INSANE // works
    }
}

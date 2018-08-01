using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CARnageCar : MonoBehaviour {
    public string carName = "";
    public CarColor carColor;

    // stats values:
    public int speed = 5;
    //public int acceleration = 5;
    public int impact = 5;
    public int HP = 5;
    public int shield = 0; // from 0 ... 4 => 0 ... maxHP
    public int nitro = 0; // from 0 ... 4 => 0 ... 100
    public int gearStorage = 5; // from 50 ... 500

    // real time values:
    [HideInInspector]
    public float maxHP;
    [HideInInspector]
    public float currentHP;
    [HideInInspector]
    public float maxShield;
    [HideInInspector]
    public float currentShield;
    [HideInInspector]
    public float currentGears;
    [HideInInspector]
    public float maxGears;
    [HideInInspector]
    public bool destroyed;
    [HideInInspector]
    public int destroyedCars;

    // UIs
    public Image HPBar;
    public Image ShieldBar;
    public Image NitroBar;
    public Image maxHPBar;
    public Image maxShieldBar;
    public Image maxNitroBar;

    public GameObject speedUI;

    public GameObject observingCamera;

    public GameObject goShield;
    public GameObject goShieldShattered;
    public GameObject nakedModel;

    public CARnageAuxiliary.ControllerType controlledBy;

    float lastDamagedTime;
    float shieldRegSeconds = 5;
    float shieldRepairRate = 1;

    private void Start()
    {
        initializeValues();
    }

    float secondCounter;
    private void Update()
    {
        if (speedUI != null)
            speedUI.GetComponent<Text>().text = (int)(GetComponent<RCC_CarControllerV3>().speed) + " km/h";

        if (isShielded() && lastDamagedTime + (shieldRegSeconds * getModController().getShieldRegenerationOnset_Multiplier()) < Time.time)
            regenerateShield(Time.deltaTime * shieldRepairRate * getModController().getShieldRegeneration_Multiplier());

        secondCounter += Time.deltaTime;
        if(secondCounter >= 1)
        {
            secondCounter -= 1;
            getModController().onSecondPassed();
        }
    }

    public float getRCC_speed()
    {
        // range 1 ... 10 to 40 ... 400 km/h
        return 400 * speed / 10;
    }
    public float getRCC_nitro()
    {
        // range 0 ... 4 to 0 ... 100
        return nitro * 25;
    }

    public float getCurrentNitro()
    {
        return GetComponent<RCC_CarControllerV3>().NoS;
    }

    public void initializeValues()
    {
        lastDamagedTime = 0;
        destroyedCars = 0;
        maxHP = 200 * HP / 10;
        currentHP = maxHP;
        maxShield = maxHP * shield / 4;
        currentShield = maxShield;
        secondCounter = 0;
        if (currentShield > 0)
            goShield.SetActive(true);
        else
            goShield.SetActive(false);

        currentGears = 0;
        maxGears = 50 * gearStorage;

        getModController().onSpawn();
        updateValues();
    }

    public void updateValues()
    {
        float maxBar = maxHP;
        if (maxShield > maxHP)
            maxBar = maxShield;

        if (maxHP > 0)
            HPBar.fillAmount = currentHP / maxBar;
        else
            HPBar.fillAmount = 0;
        maxHPBar.fillAmount = maxHP / maxBar;

        if (maxShield > 0)
        {
            ShieldBar.fillAmount = currentShield / maxBar;
            maxShieldBar.fillAmount = maxShield / maxBar;
        }
        else
            ShieldBar.fillAmount = 0;
        GetComponentInChildren<damageCar>().updateHP(this);
        
        if (nitro > 0)
        {
            NitroBar.fillAmount = getCurrentNitro() / (getRCC_nitro());
            maxNitroBar.fillAmount = getRCC_nitro() / 100f;
        }
        else
            NitroBar.fillAmount = 0;

        //foreach (Material m in nakedModel.GetComponent<MeshRenderer>().materials)
        //    if (m.name.Contains("ProgDamageMat"))
        //        m.color = new Color(1, 1, 1, 1-(currentHP/maxHP));

        if (!destroyed && currentHP <= 0 && currentShield <= 0)
            destroyMe();
    }

    CARnageCar lastDamager;
    public void damageMe(float damage, CARnageCar damager, DamageType damageType)
    {
        damage *= getModController().getSelfDMG_Multiplier(damager, damageType);
        damage *= damager.getModController().getDMG_Multiplier(damageType, this);
        Debug.Log("Damage dealt: " + damage);
        if (damage <= 0)
            return;

        lastDamagedTime = Time.time;
        lastDamager = damager;

        if (currentShield > 0 && !damager.getModController().isIgnoringEnemyShield())
        {
            currentShield -= damage;
            if (currentShield <= 0)
            {
                currentShield = 0;
                breakShield();
            }
        }
        else
        {
            currentHP -= damage;
            if (currentHP <= 0)
            {
                currentHP = 0;
                destroyMe();
            }
        }
        updateValues();
        damager.getModController().onDMGDealt(this, damageType, damage);
    }

    public bool isOnFire()
    {
        if (fireTicks > 0)
            return true;
        return false;
    }

    float fireTicks = 0;
    CARnageCar fireApplier;
    public float acidTicks = 0;
    CARnageCar acidApplier;
    float drainTicks = 0;
    CARnageCar drainApplier;
    float freezeTicks = 0;
    float lockedTicks = 0;
    int fireDamage = 5;
    int fireTicksInitial = 5;
    int acidDamage = 2;
    int acidTicksInitial = 10;
    int drainDamage = 2;
    int drainTicksInitial = 5;
    int freezeTicksInitial = 5;
    int lockedTicksInitial = 1;
    public void applyDebuff(Debuff debuff, CARnageCar applier)
    {
        applyDebuff(debuff, applier, 1);
    }

    public void applyDebuff(Debuff debuff, CARnageCar applier, float multiplier)
    {
        switch (debuff)
        {
            case Debuff.Fire:
                if (getModController().getDebuffImmunity(Debuff.Fire))
                    return;
                fireApplier = applier;
                if (fireTicks > 0)
                {
                    fireTicks = Mathf.Max(fireTicks, fireTicksInitial * multiplier);
                    return;
                }
                fireTicks = Mathf.Max(fireTicks, fireTicksInitial * multiplier);
                fireDMG();
                GetComponentInChildren<damageCar>().showFire();
                break;
            case Debuff.Acid:
                if (getModController().getDebuffImmunity(Debuff.Acid))
                    return;
                acidApplier = applier;
                if (acidTicks > 0)
                {
                    acidTicks = Mathf.Max(acidTicks, acidTicksInitial * multiplier);
                    return;
                }
                acidTicks = Mathf.Max(acidTicks, acidTicksInitial * multiplier);
                acidDMG();
                GetComponentInChildren<damageCar>().showAcid();
                break;
            case Debuff.Drain:
                if (getModController().getDebuffImmunity(Debuff.Drain))
                    return;
                drainApplier = applier;
                if (drainTicks > 0)
                {
                    drainTicks = Mathf.Max(drainTicks, drainTicksInitial * multiplier);
                    return;
                }
                drainTicks = Mathf.Max(drainTicks, drainTicksInitial * multiplier);
                drainDMG();
                GetComponentInChildren<damageCar>().showDrain();
                break;
            case Debuff.Freeze:
                if (getModController().getDebuffImmunity(Debuff.Freeze))
                    return;
                if (freezeTicks > 0)
                {
                    freezeTicks = Mathf.Max(freezeTicks, freezeTicksInitial * multiplier);
                    return;
                }
                freezeTicks = Mathf.Max(freezeTicks, freezeTicksInitial * multiplier);
                GetComponentInChildren<damageCar>().showFreeze();
                break;
            case Debuff.Locked:
                if (getModController().getDebuffImmunity(Debuff.Locked))
                    return;
                if (lockedTicks > 0)
                {
                    lockedTicks = Mathf.Max(lockedTicks, lockedTicksInitial * multiplier);
                    return;
                }
                lockedTicks = Mathf.Max(lockedTicks, lockedTicksInitial * multiplier);
                GetComponentInChildren<damageCar>().showLocked();
                break;
        }
    }

    public void endDebuff(Debuff debuff)
    {
        switch(debuff)
        {
            case Debuff.Fire:
                GetComponentInChildren<damageCar>().hideFire();
                break;
            case Debuff.Acid:
                GetComponentInChildren<damageCar>().hideAcid();
                break;
            case Debuff.Drain:
                GetComponentInChildren<damageCar>().hideDrain();
                break;
        }
    }

    void fireDMG()
    {
        damageMe(fireDamage, fireApplier, DamageType.DEBUFF);
        fireTicks--;
        if (fireTicks > 0)
            Invoke("fireDMG", 1);
        else
            endDebuff(Debuff.Fire);
    }
    void acidDMG()
    {
        damageMe(acidDamage, acidApplier, DamageType.DEBUFF);
        acidTicks--;
        if (acidTicks > 0)
            Invoke("acidDMG", 1);
        else
            endDebuff(Debuff.Acid);
    }
    void drainDMG()
    {
        damageMe(drainDamage, drainApplier, DamageType.DEBUFF);
        drainTicks--;
        if (drainTicks > 0)
            Invoke("drainDMG", 1);
        else
            endDebuff(Debuff.Drain);
    }

    public enum Debuff
    {
        Fire,
        Acid,
        Drain,
        Freeze,
        Locked
    }

    public void breakShield()
    {
        goShield.SetActive(false);
        GameObject go = Instantiate(goShieldShattered, goShield.transform.parent);
        go.transform.parent = null;
        Destroy(go, CARnageAuxiliary.destroyAfterSec);
        getModController().onShieldDestroyed();
    }

    public void destroyMe()
    {
        if (lastDamager != null)
            lastDamager.getModController().onDestroyingCar(this);
        destroyed = true;
        GetComponent<RCC_CarControllerV3>().canControl = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        getWeaponController().dropAllEquippedWeapons();
        foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
            mr.enabled = false;
        foreach (Collider c in GetComponentsInChildren<Collider>())
            c.enabled = false;
        foreach (Rigidbody r in GetComponentsInChildren<Rigidbody>())
            r.useGravity = false;
        foreach (Image i in GetComponentsInChildren<Image>())
            i.enabled = false;
        getModController().onSelfDestroyed(lastDamager);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // on Ramming:
        CARnageCar colliCar = collision.gameObject.GetComponent<CARnageCar>();
        if (colliCar != null)
        {
            //Debug.Log(gameObject.GetComponent<Rigidbody>().velocity + " (" + gameObject.GetComponent<Rigidbody>().velocity.magnitude + ") vs. " + GetComponent<RCC_CarControllerV3>().speed);
            float damage = 0.1f * GetComponent<RCC_CarControllerV3>().speed * impact;
            Debug.Log("Damage from " + name + " to " + colliCar.name + " : " + damage);
            colliCar.damageMe(damage, this, DamageType.RAM);
            return;
        }
    }

    public void replenishShield()
    {
        regenerateShield(maxShield); // no overheal
    }
    public void replenishShield(float amount) // with overheal
    {
        // repair not possible while acid is applied
        if (acidTicks > 0)
            return;
        
        goShield.SetActive(true);
        currentShield += amount;
        maxShield = Mathf.Max(maxShield, currentShield);
        updateValues();
    }
    public void regenerateShield(float amount)
    {
        // repair not possible while acid is applied
        if (acidTicks > 0)
            return;

        currentShield = Mathf.Min(currentShield + amount, maxShield);
        updateValues();
    }

    public void replenishNitro(float amount)
    {
        GetComponent<RCC_CarControllerV3>().NoS = Mathf.Min(GetComponent<RCC_CarControllerV3>().NoS + amount, GetComponent<RCC_CarControllerV3>().maxNitro);
        updateValues();
    }

    public bool isShielded()
    {
        if (currentShield > 0)
            return true;
        return false;
    }

    public void dropGears(int Count)
    {

    }

    public void repair()
    {
        repair(-1);
    }

    public void repair(float amount)
    {
        // repair not possible while acid is applied
        if (acidTicks > 0)
            return;

        if (amount == -1)
            amount = (int)maxHP;

        currentHP = Mathf.Min(currentHP + amount, maxHP);
        updateValues();

    }

    public void addGears(int amount)
    {
        currentGears = Mathf.Min(currentGears + amount, gearStorage);
    }

    public enum CarColor
    {
        WHITE,
        BLACK,
        RED,
        YELLOW,
        GREEN,
        BLUE,
        VIOLET,
        BROWN
    }

    public ModController getModController()
    {
        return GetComponentInChildren<ModController>();
    }

    public WeaponController getWeaponController()
    {
        return GetComponentInChildren<WeaponController>();
    }

    public void onWeaponObtained(CARnageWeapon weapon)
    {
        getModController().onWeaponObtained(weapon);
    }

    public bool isOnNitro()
    {
        return GetComponent<RCC_CarControllerV3>().isUsingNitro;
    }
}

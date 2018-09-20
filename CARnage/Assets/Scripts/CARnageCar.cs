using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CARnageCar : MonoBehaviour {
    public string carName = "";
    public CarModel carModel;
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
    public int currentGears;
    [HideInInspector]
    public int maxGears;
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
    
    public GameObject observingCamera;

    public GameObject goShield;
    public GameObject goShieldShattered;
    public GameObject nakedModel;

    public CARnageAuxiliary.ControllerType controlledBy;

    float lastDamagedTime;
    float shieldRegSeconds = 5;
    float shieldRepairRate = 1;
    public bool isIndestructible;

    private void Start()
    {
        initializeValues();
    }

    float secondCounter;
    private void Update()
    {
        //if (speedUI != null)
        //    speedUI.GetComponent<Text>().text = (int)(GetComponent<RCC_CarControllerV3>().speed) + " km/h";

        if (isShielded() && lastDamagedTime + (shieldRegSeconds * getModController().getShieldRegenerationOnset_Multiplier()) < Time.time)
            regenerateShield(Time.deltaTime * shieldRepairRate * getModController().getShieldRegeneration_Multiplier());

        secondCounter += Time.deltaTime;
        if(secondCounter >= 1)
        {
            secondCounter -= 1;
            getModController().onSecondPassed();
        }

        if (controlledBy == CARnageAuxiliary.ControllerType.MouseKeyboard && Input.GetKeyDown(KeyCode.R))
            resetStart = Time.time;
        if (controlledBy == CARnageAuxiliary.ControllerType.MouseKeyboard && Input.GetKey(KeyCode.R))
            if (Time.time - resetStart >= 3)
            {
                transform.position += Vector3.up * 30;
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                resetStart = float.PositiveInfinity;
            }
    }
    float resetStart;

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
        {
            ShieldBar.fillAmount = 0;
            maxShieldBar.fillAmount = 0;
        }
        GetComponentInChildren<damageCar>().updateHP(this);
        
        if (nitro > 0)
        {
            NitroBar.fillAmount = getCurrentNitro() / 100f;
            maxNitroBar.fillAmount = getRCC_nitro() / 100f;
        }
        else
        {
            NitroBar.fillAmount = 0;
            maxNitroBar.fillAmount = 0;
        }

        //foreach (Material m in nakedModel.GetComponent<MeshRenderer>().materials)
        //    if (m.name.Contains("ProgDamageMat"))
        //        m.color = new Color(1, 1, 1, 1-(currentHP/maxHP));

        if (!destroyed && currentHP <= 0 && currentShield <= 0)
            destroyMe();
    }

    [HideInInspector]
    public CARnageCar lastDamager;
    [HideInInspector]
    public DamageType lastDamageType = DamageType.DIRECT_DAMAGE;
    public void damageMe(float damage, CARnageCar damager, DamageType damageType)
    {
        if (isIndestructible) // preview screen
            return;

        damage *= getModController().getSelfDMG_Multiplier(damager, damageType);
        if(damager != null)
            damage *= damager.getModController().getDMG_Multiplier(damageType, this);
        //Debug.Log("Damage dealt: " + damage);
        //if (damage <= 0) // TODO: See if this works out
        //    return;

        lastDamagedTime = Time.time;
        if(damager)
            lastDamager = damager;
        lastDamageType = damageType;

        bool isIgnoringEnemyShield = (damager != null) ? damager.getModController().isIgnoringEnemyShield() : false;

        if (currentShield > 0 && !isIgnoringEnemyShield && damageType != DamageType.DIRECT_DAMAGE)
        {
            currentShield -= damage;
            if (currentShield <= 0)
            {
                currentShield = 0;
                maxShield = 0;
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
        if (damager != null)
            damager.getModController().onDMGDealt(this, damageType, damage);
        getModController().onDMGReceived(damage);

        GameObject dmgDisplay = Instantiate(Resources.Load<GameObject>("DMGDisplay"), GetComponentInChildren<damageCar>().transform);
        dmgDisplay.GetComponent<DMGdisplay>().display((int)damage, damageType);
    }

    public bool isOnFire()
    {
        if (fireTicks > 0)
            return true;
        return false;
    }

    public bool isFreezed()
    {
        if (freezeTicks > 0)
            return true;
        return false;
    }

    public bool isLocked()
    {
        if (lockedTicks > 0)
            return true;
        return false;
    }

    [HideInInspector]
    public float fireTicks = 0;
    CARnageCar fireApplier;
    [HideInInspector]
    public float acidTicks = 0;
    CARnageCar acidApplier;
    [HideInInspector]
    public float drainTicks = 0;
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
    int lockedTicksInitial = 2;
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
                freezeDMG();
                GetComponentInChildren<damageCar>().showFreeze();
                GetComponent<Rigidbody>().velocity = Vector3.zero;
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
                lockedDMG();
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
            case Debuff.Freeze:
                GetComponentInChildren<damageCar>().hideFreeze();
                break;
            case Debuff.Locked:
                GetComponentInChildren<damageCar>().hideLocked();
                break;
        }
    }

    void fireDMG()
    {
        if (fireTicks == -1) // ended from another source
            return;

        if(isFreezed()) // fire melts freeze
        {
            freezeTicks = -1;
            endDebuff(Debuff.Freeze);
        }

        damageMe(fireDamage, fireApplier, DamageType.DEBUFF_FIRE);
        fireTicks--;
        if (fireTicks > 0)
            Invoke("fireDMG", 1);
        else
            endDebuff(Debuff.Fire);
    }
    void acidDMG()
    {
        if (acidTicks == -1) // ended from another source
            return;
        damageMe(acidDamage, acidApplier, DamageType.DEBUFF_ACID);
        acidTicks--;
        if (acidTicks > 0)
            Invoke("acidDMG", 1);
        else
            endDebuff(Debuff.Acid);
    }
    void drainDMG()
    {
        if (drainTicks == -1) // ended from another source
            return;
        damageMe(drainDamage, drainApplier, DamageType.DEBUFF_DRAIN);
        drainTicks--;
        if (drainTicks > 0)
            Invoke("drainDMG", 1);
        else
            endDebuff(Debuff.Drain);
    }
    void freezeDMG()
    {
        if (freezeTicks == -1) // ended from another source
            return;
        freezeTicks--;
        if (freezeTicks > 0)
            Invoke("freezeDMG", 1);
        else
            endDebuff(Debuff.Freeze);
    }
    void lockedDMG()
    {
        if (lockedTicks == -1) // ended from another source
            return;
        lockedTicks--;
        if (lockedTicks > 0)
            Invoke("lockedDMG", 1);
        else
            endDebuff(Debuff.Locked);
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
        //GlobalModifiers.sloMoCounter += 1;
    }

    public void destroyMe()
    {
        if (destroyed)
            return;
        if (lastDamager != null)
            lastDamager.getModController().onDestroyingCar(this);
        getModController().onSelfDestroyed(lastDamager);
        destroyed = true;
        GetComponent<RCC_CarControllerV3>().canControl = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        getWeaponController().dropAllEquippedWeapons();
        dropGears((int)(currentGears * getModController().getDroppedGears_Multiplier()), lastDamager);

        foreach (RCC_WheelCollider wc in GetComponent<RCC_CarControllerV3>().allWheelColliders)
        {
            wc.transform.parent = null;
            wc.enabled = false;
            //wc.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        GetComponent<RCC_CarControllerV3>().FrontLeftWheelTransform.parent = null;
        GetComponent<RCC_CarControllerV3>().FrontRightWheelTransform.parent = null;
        GetComponent<RCC_CarControllerV3>().RearLeftWheelTransform.parent = null;
        GetComponent<RCC_CarControllerV3>().RearRightWheelTransform.parent = null;



        foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
            mr.enabled = false;
        foreach (Collider c in GetComponentsInChildren<Collider>())
            c.enabled = false;
        foreach (Rigidbody r in GetComponentsInChildren<Rigidbody>())
            r.useGravity = false;
        foreach (Image i in GetComponentsInChildren<Image>())
            i.enabled = false;

        GetComponent<RCC_CarControllerV3>().FrontLeftWheelTransform.gameObject.AddComponent<Rigidbody>();
        GetComponent<RCC_CarControllerV3>().FrontRightWheelTransform.gameObject.AddComponent<Rigidbody>();
        GetComponent<RCC_CarControllerV3>().RearLeftWheelTransform.gameObject.AddComponent<Rigidbody>();
        GetComponent<RCC_CarControllerV3>().RearRightWheelTransform.gameObject.AddComponent<Rigidbody>();
        GetComponent<RCC_CarControllerV3>().FrontLeftWheelTransform.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<RCC_CarControllerV3>().FrontRightWheelTransform.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<RCC_CarControllerV3>().RearLeftWheelTransform.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<RCC_CarControllerV3>().RearRightWheelTransform.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<RCC_CarControllerV3>().FrontLeftWheelTransform.gameObject.AddComponent<CapsuleCollider>();
        GetComponent<RCC_CarControllerV3>().FrontRightWheelTransform.gameObject.AddComponent<CapsuleCollider>();
        GetComponent<RCC_CarControllerV3>().RearLeftWheelTransform.gameObject.AddComponent<CapsuleCollider>();
        GetComponent<RCC_CarControllerV3>().RearRightWheelTransform.gameObject.AddComponent<CapsuleCollider>();
        GetComponent<RCC_CarControllerV3>().FrontLeftWheelTransform.gameObject.GetComponent<Rigidbody>().AddForce(Random.Range(-10, 10), Random.Range(0, 10), Random.Range(-10, 10), ForceMode.Impulse);
        GetComponent<RCC_CarControllerV3>().FrontRightWheelTransform.gameObject.GetComponent<Rigidbody>().AddForce(Random.Range(-10, 10), Random.Range(0, 10), Random.Range(-10, 10), ForceMode.Impulse);
        GetComponent<RCC_CarControllerV3>().RearLeftWheelTransform.gameObject.GetComponent<Rigidbody>().AddForce(Random.Range(-10, 10), Random.Range(0, 10), Random.Range(-10, 10), ForceMode.Impulse);
        GetComponent<RCC_CarControllerV3>().RearRightWheelTransform.gameObject.GetComponent<Rigidbody>().AddForce(Random.Range(-10, 10), Random.Range(0, 10), Random.Range(-10, 10), ForceMode.Impulse);

        GameObject.Find("SPAWNLOGIC").GetComponent<SpawnLogic>().onCarDestroyed(gameObject);

    }

    private void OnCollisionEnter(Collision collision)
    {
        // on Ramming:
        CARnageCar colliCar = collision.gameObject.GetComponent<CARnageCar>();
        if (colliCar != null)
        {
            //Debug.Log(gameObject.GetComponent<Rigidbody>().velocity + " (" + gameObject.GetComponent<Rigidbody>().velocity.magnitude + ") vs. " + GetComponent<RCC_CarControllerV3>().speed);
            float damage = 0.1f * GetComponent<RCC_CarControllerV3>().speed * impact;
            //Debug.Log("Damage from " + name + " to " + colliCar.name + " : " + damage);
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
        if (!isRepairable())
            return;
        
        goShield.SetActive(true);
        currentShield += amount;
        maxShield = Mathf.Max(maxShield, currentShield);
        updateValues();
        GameObject dmgDisplay = Instantiate(Resources.Load<GameObject>("DMGDisplay"), GetComponentInChildren<damageCar>().transform);
        dmgDisplay.GetComponent<DMGdisplay>().displayRepair((int)amount, true);
    }
    public void regenerateShield(float amount)
    {
        amount = Mathf.Min(amount, maxShield - currentShield);
        replenishShield(amount);
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

    public void dropGears(int amount, CARnageCar gearReceiver)
    {
        amount = Mathf.Min(amount, currentGears);
        if (amount <= 0)
            return;
        currentGears -= amount;
        Gear.spawnGears(amount, this, CARnageModifier.GearSource.CAR, gearReceiver);
    }

    public void repair()
    {
        repair(-1);
    }

    public bool isRepairable()
    {
        if (acidTicks > 0)
            return false;
        return true;
    }

    public void repair(float amount)
    {
        repair(amount, false);
    }
    public void repair(float amount, bool overrideAcid)
    {
        // repair not possible while acid is applied
        if (!isRepairable() && !overrideAcid)
            return;

        if (amount == -1)
            amount = (int)maxHP - currentHP;
        else
            amount = Mathf.Min(amount, maxHP - currentHP);
        currentHP += amount;
        updateValues();

        GameObject dmgDisplay = Instantiate(Resources.Load<GameObject>("DMGDisplay"), GetComponentInChildren<damageCar>().transform);
        dmgDisplay.GetComponent<DMGdisplay>().displayRepair((int)amount, false);
    }

    public bool canCarryGears()
    {
        if (currentGears < maxGears)
            return true;
        return false;
    }

    public void addGears(int amount, CARnageModifier.GearSource source)
    {
        amount *= (int)getModController().getCollectedGears_Multiplier(source);
        currentGears = Mathf.Min(currentGears + amount, maxGears);
    }

    public enum CarColor
    {
        WHITE,
        BLACK,
        RED,
        YELLOW,
        ORANGE,
        GREEN,
        BLUE,
        VIOLET,
        BROWN
    }

    public void setColor(CarColor newColor)
    {
        carColor = newColor;
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

    private void OnParticleCollision(GameObject other)
    {
        if(other.GetComponentInParent<CARnageWeapon>() != null)
            other.GetComponentInParent<CARnageWeapon>().doParticleDMG(this);
    }
}

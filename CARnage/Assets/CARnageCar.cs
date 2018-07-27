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
    public int shield = 0; // from 0 ... 4
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

    public GameObject speedUI;

    public GameObject observingCamera;

    public GameObject goShield;
    public GameObject goShieldShattered;
    public GameObject nakedModel;

    public CARnageAuxiliary.ControllerType controlledBy;

    public List<GameObject> modList;

    private void Start()
    {
        initializeValues();
    }

    private void Update()
    {
        if (speedUI != null)
            speedUI.GetComponent<Text>().text = (int)(GetComponent<RCC_CarControllerV3>().speed) + " km/h";

        // debug: J
        if (Input.GetKey(KeyCode.J))
        {
            damageMe(Time.deltaTime * 10);
        }
        if (Input.GetKey(KeyCode.K))
        {
            damageMe(Time.deltaTime * 100);
        }
    }

    public float getRCC_speed()
    {
        // range 1 ... 10 to 40 ... 400 km/h
        return 400 * speed / 10;
    }
    //public float getRCC_acceleration()
    //{
    //    return 2f* acceleration / 10f;
    //}

    public void initializeValues()
    {
        //GetComponent<RCC_CarControllerV3>().orgMaxSpeed = 400 * speed / 10;
        // TODO: Acceleration
        destroyedCars = 0;
        maxHP = 200 * HP / 10;
        currentHP = maxHP;
        maxShield = maxHP * shield / 4;
        currentShield = maxShield;
        if (currentShield > 0)
            goShield.SetActive(true);
        else
            goShield.SetActive(false);

        currentGears = 0;
        maxGears = 50 * gearStorage;

        foreach (GameObject modGO in modList)
            modGO.GetComponent<CARnageModifier>().resetMod();
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

        if (maxShield > 0)
            ShieldBar.fillAmount = currentShield / maxBar;
        else
            ShieldBar.fillAmount = 0;
        GetComponentInChildren<damageCar>().updateHP(this);

        //foreach (Material m in nakedModel.GetComponent<MeshRenderer>().materials)
        //    if (m.name.Contains("ProgDamageMat"))
        //        m.color = new Color(1, 1, 1, 1-(currentHP/maxHP));

        if (currentHP <= 0 && currentShield <= 0)
            destroyMe();
    }

    public void damageMe(float damage)
    {
        if (currentShield > 0)
        {
            currentShield -= damage;
            if (currentShield <= 0)
            {
                breakShield();
                currentShield = 0;
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
    }

    public bool isOnFire()
    {
        if (fireTicks > 0)
            return true;
        return false;
    }

    float fireTicks = 0;
    CARnageCar fireApplier;
    float acidTicks = 0;
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
                if (freezeTicks > 0)
                {
                    freezeTicks = Mathf.Max(freezeTicks, freezeTicksInitial * multiplier);
                    return;
                }
                freezeTicks = Mathf.Max(freezeTicks, freezeTicksInitial * multiplier);
                GetComponentInChildren<damageCar>().showFreeze();
                break;
            case Debuff.Locked:
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
        damageMe(fireDamage);
        fireTicks--;
        if (fireTicks > 0)
            Invoke("fireDMG", 1);
        else
            endDebuff(Debuff.Fire);
    }
    void acidDMG()
    {
        damageMe(acidDamage);
        acidTicks--;
        if (acidTicks > 0)
            Invoke("acidDMG", 1);
        else
            endDebuff(Debuff.Acid);
    }
    void drainDMG()
    {
        damageMe(drainDamage);
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
    }

    public void destroyMe()
    {
        destroyed = true;
        GetComponent<RCC_CarControllerV3>().canControl = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        foreach (MeshRenderer mr in gameObject.GetComponentsInChildren<MeshRenderer>())
            mr.enabled = false;
        foreach (Collider c in gameObject.GetComponentsInChildren<Collider>())
            c.enabled = false;
        foreach (Rigidbody r in gameObject.GetComponentsInChildren<Rigidbody>())
            r.useGravity = false;
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
            colliCar.damageMe(damage);
            return;
        }
    }

    public void replenishShield()
    {
        replenishShield(-1);
    }
    public void replenishShield(float amount)
    {
        currentShield += amount;
        maxShield = Mathf.Max(maxShield, currentShield);
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

    public void dropWeapon()
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
        VIOLET
    }
}

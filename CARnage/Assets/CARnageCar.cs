using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CARnageCar : MonoBehaviour {
    public string carName = "";

    // stats values:
    public int speed = 5;
    public int acceleration = 5;
    public int impact = 5;
    public int HP = 5;
    public int shield = 0; // from 0 ... 4

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
    public bool destroyed;

    // UIs
    public Image HPBar;
    public Image ShieldBar;

    public GameObject speedUI;

    public GameObject observingCamera;

    public GameObject goShield;
    public GameObject goShieldShattered;
    public GameObject nakedModel;

    public CARnageAuxiliary.ControllerType controlledBy;

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

    public void initializeValues()
    {
        GetComponent<RCC_CarControllerV3>().orgMaxSpeed = 400 * speed / 10;
        // TODO: Acceleration
        maxHP = 200 * HP / 10;
        currentHP = maxHP;
        maxShield = maxHP * shield / 4;
        currentShield = maxShield;
        if (currentShield > 0)
            goShield.SetActive(true);
        else
            goShield.SetActive(false);

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

        foreach (Material m in nakedModel.GetComponent<MeshRenderer>().materials)
            if (m.name.Contains("ProgDamageMat"))
                m.color = new Color(1, 1, 1, 1-(currentHP/maxHP));

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
}

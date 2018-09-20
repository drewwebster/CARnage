using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CarSelectionLogic : MonoBehaviour {

    enum SortOrder
    {
        ARRAY,
        NAME,
        COLOR,
        SPEED,
        IMPACT,
        STORAGE,
        HP,
        SHIELD,
        NITRO
    }

    enum CarSelectionState
    {
        SELECT_CAR,
        SELECT_COLOR,
        SELECT_WEAPON_1_UPGRADE,
        SELECT_WEAPON_2_UPGRADE,
        SELECT_MOD_1,
        SELECT_MOD_2
    }

    public GameObject select_car_GO;
    bool select_car_init;
    public GameObject select_color_GO;
    public GameObject select_weapon_upgrade_GO;
    public GameObject select_mod_GO;
    bool select_mod_init;

    public GameObject UpgradeOption_NONE;
    public GameObject UpgradeOption_MAGAZINE;
    public GameObject UpgradeOption_SCOPE;
    public GameObject UpgradeOption_SILENCER;
    public GameObject UpgradeOption_COMPENSATOR;
    public GameObject UpgradeOption_LIGHT;
    public GameObject UpgradeOption_AUTOMATIC;

    List<GameObject> carList;
    GameObject selectedCar;
    CarSelectionState currentState;
    string selectingPlayer;

    // Use this for initialization
    void Start () {
        currentState = CarSelectionState.SELECT_CAR;
        selectingPlayer = PlayerPrefs.GetString("selectingPlayer");
        if (selectingPlayer.Equals(""))
            selectingPlayer = "Player0";
        updateCarSelectionState();
    }

    private void updateCarSelectionState()
    {
        select_car_GO.SetActive(false);
        select_color_GO.SetActive(false);
        select_weapon_upgrade_GO.SetActive(false);
        select_mod_GO.SetActive(false);

        switch (currentState)
        {
            case CarSelectionState.SELECT_CAR:
                select_car_GO.SetActive(true);
                GameObject.Find("CarSelectionState_Text").GetComponent<Text>().text = "Select Car";
                if(!select_car_init)
                {
                    // create list
                    Array values = Enum.GetValues(typeof(CarModel));
                    carList = new List<GameObject>();
                    foreach (CarModel model in values)
                    {
                        GameObject go = Instantiate(Resources.Load<GameObject>("CarIcon"), select_car_GO.transform);

                        go.GetComponentsInChildren<Image>()[1].sprite = Resources.Load<Sprite>("CarIcons/" + model);
                        GameObject CARgo = spawnCar(model);

                        go.GetComponent<CarIcon>().rel_car = CARgo;
                        carList.Add(go);

                        if (!selectingPlayer.Equals(""))
                        {
                            if (PlayerPrefs.GetString(selectingPlayer + "_Car").Equals(CARgo.GetComponent<CARnageCar>().carModel.ToString()))
                                selectedCar = go;
                            CarFactory.modifyCarToPlayerPrefs(CARgo, selectingPlayer);
                        }
                        CARgo.SetActive(false);
                    }
                    // display
                    currSortOrder = SortOrder.ARRAY;
                    sortCars();
                    // show selected car
                    if(selectedCar == null)
                        selectedCar = carList[0];
                    
                    showSelectedCar();
                    select_car_init = true;
                }               
                break;
            case CarSelectionState.SELECT_COLOR:
                select_color_GO.SetActive(true);
                selectedColor = selectedCar.GetComponent<CarIcon>().rel_car.GetComponent<CARnageCar>().carColor;
                showSelectedColor();
                GameObject.Find("CarSelectionState_Text").GetComponent<Text>().text = "Select Color";
                break;
            case CarSelectionState.SELECT_WEAPON_1_UPGRADE:
                select_weapon_upgrade_GO.SetActive(true);
                selectWeaponUpgradeInit(selectedCar.GetComponent<CarIcon>().rel_car.GetComponent<CARnageCar>().getWeaponController().getLeftWeapon());
                break;
            case CarSelectionState.SELECT_WEAPON_2_UPGRADE:
                select_weapon_upgrade_GO.SetActive(true);
                selectWeaponUpgradeInit(selectedCar.GetComponent<CarIcon>().rel_car.GetComponent<CARnageCar>().getWeaponController().getRightWeapon());
                break;
            case CarSelectionState.SELECT_MOD_1:
                select_mod_GO.SetActive(true);
                selectModInit(0);
                break;
            case CarSelectionState.SELECT_MOD_2:
                select_mod_GO.SetActive(true);
                selectModInit(1);
                break;
        }
    }
    GameObject spawnCar(CarModel model)
    {
        GameObject CARgo = Instantiate(Resources.Load<GameObject>(model.ToString()), GameObject.Find("CAR_PREVIEW_SPAWN").transform);
        CARgo.GetComponent<CARnageCar>().enabled = false;
        CARgo.GetComponent<RCC_CarControllerV3>().enabled = false;
        CARgo.GetComponent<CARnageCar>().isIndestructible = true;

        foreach (Rigidbody rb in CARgo.GetComponentsInChildren<Rigidbody>())
            rb.useGravity = false;
        foreach (Rigidbody rb in CARgo.GetComponentsInChildren<Rigidbody>())
            rb.isKinematic = true;
        CARgo.transform.localPosition = Vector3.zero;

        foreach (scaleEmission se in CARgo.GetComponentsInChildren<scaleEmission>())
            se.gameObject.SetActive(false);
        Destroy(GameObject.Find("WorldCanvasHPShieldNitro"));

        return CARgo;
    }

    SortOrder currSortOrder;
    void sortCars()
    {
        switch (currSortOrder)
        {
            case SortOrder.NAME:
                carList.Sort(new nameSorter());
                break;
            case SortOrder.SPEED:
                carList.Sort(new speedSorter());
                break;
            case SortOrder.IMPACT:
                carList.Sort(new impactSorter());
                break;
            case SortOrder.STORAGE:
                carList.Sort(new storageSorter());
                break;
            case SortOrder.HP:
                carList.Sort(new hpSorter());
                break;
            case SortOrder.SHIELD:
                carList.Sort(new shieldSorter());
                break;
            case SortOrder.NITRO:
                carList.Sort(new nitroSorter());
                break;
            case SortOrder.COLOR:
                carList.Sort(new colorSorter());
                break;
        }

        int x = 0;
        int y = 0;
        int lastValue = -1;
        foreach (GameObject go in carList)
        {
            bool stepBreak = false;
            int val = -1;
            switch(currSortOrder)
            {
                case SortOrder.SPEED:
                    val = go.GetComponentInChildren<CARnageCar>().speed;
                    break;
                case SortOrder.IMPACT:
                    val = go.GetComponentInChildren<CARnageCar>().impact;
                    break;
                case SortOrder.STORAGE:
                    val = go.GetComponentInChildren<CARnageCar>().gearStorage;
                    break;
                case SortOrder.HP:
                    val = go.GetComponentInChildren<CARnageCar>().HP;
                    break;
                case SortOrder.SHIELD:
                    val = go.GetComponentInChildren<CARnageCar>().shield;
                    break;
                case SortOrder.NITRO:
                    val = go.GetComponentInChildren<CARnageCar>().nitro;
                    break;
                case SortOrder.COLOR:
                    val = (int)go.GetComponentInChildren<CARnageCar>().carColor;
                    break;
            }
            if(val != -1)
            {
                if (lastValue != -1 && val != lastValue)
                    stepBreak = true;
                lastValue = val;
            }

            if(stepBreak && x > 0)
            {
                x = 0;
                y++;
            }

            go.transform.position = new Vector3(910, 1095, 0) + new Vector3(x * 120, -y * 105);
            x++;

            if (x == 8)
            {
                x = 0;
                y++;
            }
        }
    }

    public class nameSorter : IComparer<GameObject>
    {
        public int Compare(GameObject g1, GameObject g2)
        {
            return g1.GetComponentInChildren<CARnageCar>().carName.CompareTo(g2.GetComponentInChildren<CARnageCar>().carName);
        }
    }
    public class speedSorter : IComparer<GameObject>
    {
        public int Compare(GameObject g1, GameObject g2)
        {
            return -g1.GetComponentInChildren<CARnageCar>().speed.CompareTo(g2.GetComponentInChildren<CARnageCar>().speed);
        }
    }
    public class impactSorter : IComparer<GameObject>
    {
        public int Compare(GameObject g1, GameObject g2)
        {
            return -g1.GetComponentInChildren<CARnageCar>().impact.CompareTo(g2.GetComponentInChildren<CARnageCar>().impact);
        }
    }
    public class storageSorter : IComparer<GameObject>
    {
        public int Compare(GameObject g1, GameObject g2)
        {
            return -g1.GetComponentInChildren<CARnageCar>().gearStorage.CompareTo(g2.GetComponentInChildren<CARnageCar>().gearStorage);
        }
    }
    public class hpSorter : IComparer<GameObject>
    {
        public int Compare(GameObject g1, GameObject g2)
        {
            return -g1.GetComponentInChildren<CARnageCar>().HP.CompareTo(g2.GetComponentInChildren<CARnageCar>().HP);
        }
    }
    public class shieldSorter : IComparer<GameObject>
    {
        public int Compare(GameObject g1, GameObject g2)
        {
            return -g1.GetComponentInChildren<CARnageCar>().shield.CompareTo(g2.GetComponentInChildren<CARnageCar>().shield);
        }
    }
    public class nitroSorter : IComparer<GameObject>
    {
        public int Compare(GameObject g1, GameObject g2)
        {
            return -g1.GetComponentInChildren<CARnageCar>().nitro.CompareTo(g2.GetComponentInChildren<CARnageCar>().nitro);
        }
    }
    public class colorSorter : IComparer<GameObject>
    {
        public int Compare(GameObject g1, GameObject g2)
        {
            return g1.GetComponentInChildren<CARnageCar>().carColor.CompareTo(g2.GetComponentInChildren<CARnageCar>().carColor);
        }
    }

    void showSelectedCar()
    {
        selectedCar.GetComponentInChildren<Image>().color = Color.white;
        selectedCar.GetComponentsInChildren<Image>()[1].color = new Color(50 / 255f, 50 / 255f, 50 / 255f);
        GameObject carGO = selectedCar.GetComponent<CarIcon>().rel_car;
        carGO.SetActive(true);
        CARnageCar car = carGO.GetComponent<CARnageCar>();
        GameObject.Find("CarName_Text").GetComponent<Text>().text = car.carName;

        displayMods(selectedCar.GetComponent<CarIcon>().rel_car);
        displayWeapon(car.getWeaponController().getLeftWeapon(), "L");
        displayWeapon(car.getWeaponController().getRightWeapon(), "R");
        displayCarStats(car);
    }
    public static void displayCarStats(CARnageCar car)
    {
        displayCarStats(car, GameObject.Find("Canvas").transform);
    }
    public static void displayCarStats(CARnageCar car, Transform playerTrans)
    {
        int i = 1;
        foreach (Image img in CARnageAuxiliary.FindDeepChild(playerTrans, "SpeedBlocks").GetComponentsInChildren<Image>())
            if (img.name.Equals("Filled"))
            {
                if (car.speed >= i)
                    img.enabled = true;
                else
                    img.enabled = false;
                i++;
            }
        i = 1;
        foreach (Image img in CARnageAuxiliary.FindDeepChild(playerTrans, "ImpactBlocks").GetComponentsInChildren<Image>())
            if (img.name.Equals("Filled"))
            {
                if (car.impact >= i)
                    img.enabled = true;
                else
                    img.enabled = false;
                i++;
            }
        i = 1;
        foreach (Image img in CARnageAuxiliary.FindDeepChild(playerTrans, "StorageBlocks").GetComponentsInChildren<Image>())
            if (img.name.Equals("Filled"))
            {
                if (car.gearStorage >= i)
                    img.enabled = true;
                else
                    img.enabled = false;
                i++;
            }
        i = 1;
        foreach (Image img in CARnageAuxiliary.FindDeepChild(playerTrans, "HPBlocks").GetComponentsInChildren<Image>())
            if (img.name.Equals("Filled"))
            {
                if (car.HP >= i)
                    img.enabled = true;
                else
                    img.enabled = false;
                i++;
            }
        i = 1;
        foreach (Image img in CARnageAuxiliary.FindDeepChild(playerTrans, "ShieldBlocks").GetComponentsInChildren<Image>())
            if (img.name.Equals("Filled"))
            {
                if (car.shield >= i)
                    img.enabled = true;
                else
                    img.enabled = false;
                i++;
            }
        i = 1;
        foreach (Image img in CARnageAuxiliary.FindDeepChild(playerTrans, "NitroBlocks").GetComponentsInChildren<Image>())
            if (img.name.Equals("Filled"))
            {
                if (car.nitro >= i)
                    img.enabled = true;
                else
                    img.enabled = false;
                i++;
            }
    }

    CARnageCar.CarColor selectedColor;
    void showSelectedColor()
    {
        foreach (RectTransform rt in select_color_GO.GetComponentsInChildren<RectTransform>())
        {
            if (rt.name.Contains("Color"))
                if (rt.name.Contains(selectedColor.ToString()))
                {
                    rt.GetComponentInChildren<Text>().color = Color.white;
                    rt.GetComponentInChildren<Image>().rectTransform.localScale = new Vector3(0.8761683f, 0.4672897f, 1);
                }
                else
                {
                    rt.GetComponentInChildren<Text>().color = new Color(1, 1, 1, 100f / 255f);
                    rt.GetComponentInChildren<Image>().rectTransform.localScale = new Vector3(0.5841122f, 0.4672897f, 1);
                }
        }
    }
    public static void displayWeapon(CARnageWeapon weapon, string handSide)
    {
        displayWeapon(weapon, handSide, GameObject.Find("Canvas").transform);
    }

    public static void displayWeapon(CARnageWeapon weapon, string handSide, Transform playerTrans)
    {
        if(weapon != null)
        {
            string dmgType = "";
            string delayType = "";
            CARnageAuxiliary.FindDeepChild(playerTrans, "Weapon" + handSide + "Name").GetComponent<Text>().text = weapon.weaponName.Replace("_WEAPON","");
            switch (weapon.damageType)
            {
                case DamageType.PROJECTILE:
                    dmgType = " (Projectile)";
                    delayType = "Shot delay:";
                    break;
                case DamageType.EXPLOSION:
                    dmgType = " (Explosion)";
                    delayType = "Throw delay:";
                    break;
                case DamageType.MELEE:
                    dmgType = " (Melee)";
                    delayType = "Hit delay:";
                    break;
            }
            string reloadStr = weapon.reloadTime > 0 ? "Reload time:\r\n" : "";
            string reloadTime = weapon.reloadTime > 0 ? weapon.reloadTime + "<size=9> sec</size>" : "";
            string delayAuto = weapon.automatic || weapon.getWeaponMod_automatic() ? " <size=9>(auto)</size>" : "";
            string magazineStr = weapon.magazineSize > 0 ? "Magazine:\r\n" : "";
            string magazineSize = weapon.magazineSize > 0 ? weapon.magazineSize * weapon.getWeaponMod_magazine_multiplier() + "\r\n" : "";
            CARnageAuxiliary.FindDeepChild(playerTrans, "Weapon" + handSide + "StatsText").GetComponent<Text>().text = weapon.Damage * weapon.getWeaponMod_damage_multiplier() + "<size=11>" + dmgType + "</size>" + "\r\n" + weapon.shotDelay * weapon.getWeaponMod_shotDelay_multiplier() + "<size=9> sec</size>" + delayAuto + "\r\n" + magazineSize + reloadTime;
            CARnageAuxiliary.FindDeepChild(playerTrans, "Weapon" + handSide + "Text").GetComponent<Text>().text = "DMG:\r\n" + delayType + "\r\n" + magazineStr + reloadStr;
            CARnageAuxiliary.FindDeepChild(playerTrans, "Weapon" + handSide + "ExtraText").GetComponent<Text>().text = CARnageAuxiliary.colorMe(weapon.getExtraDescription());

            string weaponImageUpgradeString = "";
            if (weapon.upgraded_magazine)
                weaponImageUpgradeString = "+MAGAZINE";
            if (weapon.upgraded_scope)
                weaponImageUpgradeString = "+SCOPE";
            if (weapon.upgraded_silencer)
                weaponImageUpgradeString = "+SILENCER";
            if (weapon.upgraded_damage)
                weaponImageUpgradeString = "+COMPENSATOR";
            if (weapon.upgraded_light)
                weaponImageUpgradeString = "+LIGHT";
            if (weapon.upgraded_automatic)
                weaponImageUpgradeString = "+AUTOMATIC";
            CARnageAuxiliary.FindDeepChild(playerTrans, "Weapon" + handSide + "Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("WeaponIcons/" + weapon.weaponModel + weaponImageUpgradeString);
            CARnageAuxiliary.FindDeepChild(playerTrans, "Weapon" + handSide + "Image").GetComponent<Image>().enabled = true;
        }
        else
        {
            CARnageAuxiliary.FindDeepChild(playerTrans, "Weapon" + handSide + "Name").GetComponent<Text>().text = "";
            CARnageAuxiliary.FindDeepChild(playerTrans, "Weapon" + handSide + "StatsText").GetComponent<Text>().text = "";
            CARnageAuxiliary.FindDeepChild(playerTrans, "Weapon" + handSide + "Text").GetComponent<Text>().text = "";
            CARnageAuxiliary.FindDeepChild(playerTrans, "Weapon" + handSide + "ExtraText").GetComponent<Text>().text = "";
            CARnageAuxiliary.FindDeepChild(playerTrans, "Weapon" + handSide + "Image").GetComponent<Image>().enabled = false;
        }
    }
    public static void displayMods(GameObject carGO)
    {
        displayMods(carGO, GameObject.Find("Canvas").transform);
    }    

    public static void displayMods(GameObject carGO, Transform playerTrans)
    {
        CARnageModifier[] mods = carGO.GetComponentsInChildren<CARnageModifier>();
        if (mods != null && mods.Length > 0)
        {
            CARnageAuxiliary.FindDeepChild(playerTrans, "ModLImage").GetComponent<Image>().sprite = mods[0].image;
            CARnageAuxiliary.FindDeepChild(playerTrans, "ModLName").GetComponent<Text>().text = mods[0].modName;
            CARnageAuxiliary.FindDeepChild(playerTrans, "ModLText").GetComponent<Text>().text = CARnageAuxiliary.colorMe(mods[0].description);
        }
        else
        {
            CARnageAuxiliary.FindDeepChild(playerTrans, "ModLImage").GetComponent<Image>().sprite = null;
            CARnageAuxiliary.FindDeepChild(playerTrans, "ModLName").GetComponent<Text>().text = "";
            CARnageAuxiliary.FindDeepChild(playerTrans, "ModLText").GetComponent<Text>().text = "";
        }
        if (mods != null && mods.Length > 1)
        {
            CARnageAuxiliary.FindDeepChild(playerTrans, "ModRImage").GetComponent<Image>().sprite = mods[1].image;
            CARnageAuxiliary.FindDeepChild(playerTrans, "ModRName").GetComponent<Text>().text = mods[1].modName;
            CARnageAuxiliary.FindDeepChild(playerTrans, "ModRText").GetComponent<Text>().text = CARnageAuxiliary.colorMe(mods[1].description);
        }
        else
        {
            CARnageAuxiliary.FindDeepChild(playerTrans, "ModRImage").GetComponent<Image>().sprite = null;
            CARnageAuxiliary.FindDeepChild(playerTrans, "ModRName").GetComponent<Text>().text = "";
            CARnageAuxiliary.FindDeepChild(playerTrans, "ModRText").GetComponent<Text>().text = "";
        }
    }

    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.E))
        //{
        //    switch(currSortOrder)
        //    {
        //        case SortOrder.NAME:
        //            currSortOrder = SortOrder.SPEED;
        //            break;
        //        case SortOrder.SPEED:
        //            currSortOrder = SortOrder.IMPACT;
        //            break;
        //        case SortOrder.IMPACT:
        //            currSortOrder = SortOrder.STORAGE;
        //            break;
        //        case SortOrder.STORAGE:
        //            currSortOrder = SortOrder.HP;
        //            break;
        //        case SortOrder.HP:
        //            currSortOrder = SortOrder.SHIELD;
        //            break;
        //        case SortOrder.SHIELD:
        //            currSortOrder = SortOrder.NITRO;
        //            break;
        //        case SortOrder.NITRO:
        //            currSortOrder = SortOrder.COLOR;
        //            break;
        //        case SortOrder.COLOR:
        //            currSortOrder = SortOrder.NAME;
        //            break;
        //    }
        //    sortCars();
        //}
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            switch (currentState)
            {
                case CarSelectionState.SELECT_CAR:
                    selectNextCar("right");
                    break;
                case CarSelectionState.SELECT_COLOR:
                    selectNextColor("right");
                    break;
                case CarSelectionState.SELECT_WEAPON_1_UPGRADE:
                    selectNextWeaponUpgrade(selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().getWeaponController().getLeftWeapon());
                    displayWeapon(selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().getWeaponController().getLeftWeapon(), "L");
                    break;
                case CarSelectionState.SELECT_WEAPON_2_UPGRADE:
                    selectNextWeaponUpgrade(selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().getWeaponController().getRightWeapon());
                    displayWeapon(selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().getWeaponController().getRightWeapon(), "R");
                    break;
                case CarSelectionState.SELECT_MOD_1:
                case CarSelectionState.SELECT_MOD_2:
                    selectNextMod("right");
                    break;
            }
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            switch (currentState)
            {
                case CarSelectionState.SELECT_CAR:
                    selectNextCar("left");
                    break;
                case CarSelectionState.SELECT_COLOR:
                    selectNextColor("left");
                    break;
                case CarSelectionState.SELECT_WEAPON_1_UPGRADE:
                    selectPreviousWeaponUpgrade(selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().getWeaponController().getLeftWeapon());
                    displayWeapon(selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().getWeaponController().getLeftWeapon(), "L");
                    break;
                case CarSelectionState.SELECT_WEAPON_2_UPGRADE:
                    selectPreviousWeaponUpgrade(selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().getWeaponController().getRightWeapon());
                    displayWeapon(selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().getWeaponController().getRightWeapon(), "R");
                    break;
                case CarSelectionState.SELECT_MOD_1:
                case CarSelectionState.SELECT_MOD_2:
                    selectNextMod("left");
                    break;
            }
        }
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            switch (currentState)
            {
                case CarSelectionState.SELECT_CAR:
                    selectNextCar("up");
                    break;
                case CarSelectionState.SELECT_COLOR:
                    selectNextColor("up");
                    break;
                case CarSelectionState.SELECT_WEAPON_1_UPGRADE:
                    selectPreviousWeaponUpgrade(selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().getWeaponController().getLeftWeapon());
                    displayWeapon(selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().getWeaponController().getLeftWeapon(), "L");
                    break;
                case CarSelectionState.SELECT_WEAPON_2_UPGRADE:
                    selectPreviousWeaponUpgrade(selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().getWeaponController().getRightWeapon());
                    displayWeapon(selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().getWeaponController().getRightWeapon(), "R");
                    break;
                case CarSelectionState.SELECT_MOD_1:
                case CarSelectionState.SELECT_MOD_2:
                    selectNextMod("up");
                    break;
            }
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            switch (currentState)
            {
                case CarSelectionState.SELECT_CAR:
                    selectNextCar("down");
                    break;
                case CarSelectionState.SELECT_COLOR:
                    selectNextColor("down");
                    break;
                case CarSelectionState.SELECT_WEAPON_1_UPGRADE:
                    selectNextWeaponUpgrade(selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().getWeaponController().getLeftWeapon());
                    displayWeapon(selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().getWeaponController().getLeftWeapon(), "L");
                    break;
                case CarSelectionState.SELECT_WEAPON_2_UPGRADE:
                    selectNextWeaponUpgrade(selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().getWeaponController().getRightWeapon());
                    displayWeapon(selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().getWeaponController().getRightWeapon(), "R");
                    break;
                case CarSelectionState.SELECT_MOD_1:
                case CarSelectionState.SELECT_MOD_2:
                    selectNextMod("down");
                    break;
            }
        }
        if (Input.GetKeyDown(KeyCode.Return))
            nextCarSelectionState();
        if (Input.GetKeyDown(KeyCode.Backspace))
            previousCarSelectionState();
        if(Input.GetKeyDown(KeyCode.X))
        {
            // reset car
            
            PlayerPrefs.SetString(selectingPlayer+"_CarColor_" + selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().carModel.ToString(), "");
            if (selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().getWeaponController().getLeftWeapon())
                PlayerPrefs.SetString(selectingPlayer+"_WeaponUpgrade_" + selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().getWeaponController().getLeftWeapon().weaponName, "");
            if (selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().getWeaponController().getRightWeapon())
                PlayerPrefs.SetString(selectingPlayer+"_WeaponUpgrade_" + selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().getWeaponController().getRightWeapon().weaponName, "");
            PlayerPrefs.SetString(selectingPlayer+"_CarMod_" + selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().carModel.ToString() + "_0", "");
            PlayerPrefs.SetString(selectingPlayer+"_CarMod_" + selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().carModel.ToString() + "_1", "");

            GameObject newCar = spawnCar(selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().carModel);
            DestroyImmediate(selectedCar.GetComponentInChildren<CarIcon>().rel_car);
            selectedCar.GetComponent<CarIcon>().rel_car = newCar;
            currentState = CarSelectionState.SELECT_CAR;
            updateCarSelectionState();
            showSelectedCar();
        }
    }

    public void nextCarSelectionState()
    {
        switch (currentState)
        {
            case CarSelectionState.SELECT_CAR:
                currentState = CarSelectionState.SELECT_COLOR;
                break;
            case CarSelectionState.SELECT_COLOR:
                currentState = CarSelectionState.SELECT_WEAPON_1_UPGRADE;
                CARnageWeapon weapon = selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().getWeaponController().getLeftWeapon();
                if (weapon == null || !weapon.isUpgradeable())
                {
                    nextCarSelectionState();
                    return;
                }
                break;
            case CarSelectionState.SELECT_WEAPON_1_UPGRADE:
                currentState = CarSelectionState.SELECT_WEAPON_2_UPGRADE;
                CARnageWeapon weaponR = selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().getWeaponController().getRightWeapon();
                if (weaponR == null || !weaponR.isUpgradeable())
                {
                    nextCarSelectionState();
                    return;
                }
                break;
            case CarSelectionState.SELECT_WEAPON_2_UPGRADE:
                currentState = CarSelectionState.SELECT_MOD_1;
                break;
            case CarSelectionState.SELECT_MOD_1:
                currentState = CarSelectionState.SELECT_MOD_2;
                break;
            case CarSelectionState.SELECT_MOD_2:
                finishSelection();
                // switch to next scene
                break;
        }
        updateCarSelectionState();
    }
    public void previousCarSelectionState()
    {
        switch (currentState)
        {
            case CarSelectionState.SELECT_CAR:
                // go back to menu where you are from
                break;
            case CarSelectionState.SELECT_COLOR:
                currentState = CarSelectionState.SELECT_CAR;
                break;
            case CarSelectionState.SELECT_WEAPON_1_UPGRADE:
                currentState = CarSelectionState.SELECT_COLOR;
                break;
            case CarSelectionState.SELECT_WEAPON_2_UPGRADE:
                currentState = CarSelectionState.SELECT_WEAPON_1_UPGRADE;
                CARnageWeapon weapon = selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().getWeaponController().getLeftWeapon();
                if (weapon == null || !weapon.isUpgradeable())
                {
                    previousCarSelectionState();
                    return;
                }
                break;
            case CarSelectionState.SELECT_MOD_1:
                currentState = CarSelectionState.SELECT_WEAPON_2_UPGRADE;
                CARnageWeapon weaponR = selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().getWeaponController().getRightWeapon();
                if (weaponR == null || !weaponR.isUpgradeable())
                {
                    previousCarSelectionState();
                    return;
                }
                break;
            case CarSelectionState.SELECT_MOD_2:
                currentState = CarSelectionState.SELECT_MOD_1;
                break;
        }
        updateCarSelectionState();
    }

    void selectNextCar(string direction)
    {
        selectedCar.GetComponentInChildren<Image>().color = new Color(50 / 255f, 50 / 255f, 50 / 255f);
        selectedCar.GetComponentsInChildren<Image>()[1].color = Color.white;
        selectedCar.GetComponent<CarIcon>().rel_car.SetActive(false);
        int index = carList.IndexOf(selectedCar);
        
        switch (direction)
        {
            case "right":
                index++;
                break;
            case "left":
                index--;
                break;
            case "up":
                index -= 8;
                break;
            case "down":
                index += 8;
                break;
        }
        
        if (index >= carList.Count)
            index = index % carList.Count;
        if (index < 0)
            index += carList.Count;
        selectedCar = carList[index];
        showSelectedCar();
        
    }

    void selectNextColor(string direction)
    {
        switch(direction)
        {
            case "right":
            case "down":
                switch(selectedColor)
                {
                    case CARnageCar.CarColor.RED:
                        selectedColor = CARnageCar.CarColor.ORANGE;
                        break;
                    case CARnageCar.CarColor.ORANGE:
                        selectedColor = CARnageCar.CarColor.YELLOW;
                        break;
                    case CARnageCar.CarColor.YELLOW:
                        selectedColor = CARnageCar.CarColor.GREEN;
                        break;
                    case CARnageCar.CarColor.GREEN:
                        selectedColor = CARnageCar.CarColor.BLUE;
                        break;
                    case CARnageCar.CarColor.BLUE:
                        selectedColor = CARnageCar.CarColor.VIOLET;
                        break;
                    case CARnageCar.CarColor.VIOLET:
                        selectedColor = CARnageCar.CarColor.BROWN;
                        break;
                    case CARnageCar.CarColor.BROWN:
                        selectedColor = CARnageCar.CarColor.BLACK;
                        break;
                    case CARnageCar.CarColor.BLACK:
                        selectedColor = CARnageCar.CarColor.WHITE;
                        break;
                    case CARnageCar.CarColor.WHITE:
                        selectedColor = CARnageCar.CarColor.RED;
                        break;
                }
                break;
            case "left":
            case "up":
                switch (selectedColor)
                {
                    case CARnageCar.CarColor.RED:
                        selectedColor = CARnageCar.CarColor.WHITE;
                        break;
                    case CARnageCar.CarColor.ORANGE:
                        selectedColor = CARnageCar.CarColor.RED;
                        break;
                    case CARnageCar.CarColor.YELLOW:
                        selectedColor = CARnageCar.CarColor.ORANGE;
                        break;
                    case CARnageCar.CarColor.GREEN:
                        selectedColor = CARnageCar.CarColor.YELLOW;
                        break;
                    case CARnageCar.CarColor.BLUE:
                        selectedColor = CARnageCar.CarColor.GREEN;
                        break;
                    case CARnageCar.CarColor.VIOLET:
                        selectedColor = CARnageCar.CarColor.BLUE;
                        break;
                    case CARnageCar.CarColor.BROWN:
                        selectedColor = CARnageCar.CarColor.VIOLET;
                        break;
                    case CARnageCar.CarColor.BLACK:
                        selectedColor = CARnageCar.CarColor.BROWN;
                        break;
                    case CARnageCar.CarColor.WHITE:
                        selectedColor = CARnageCar.CarColor.BLACK;
                        break;
                }
                break;
        }
        selectedCar.GetComponent<CarIcon>().rel_car.GetComponent<CARnageCar>().setColor(selectedColor);
        showSelectedColor();
    }

    List<GameObject> weaponUpgradeList;
    GameObject selectedWeaponUpgrade;
    void selectWeaponUpgradeInit(CARnageWeapon weapon)
    {
        weaponUpgradeList = new List<GameObject>();
        weaponUpgradeList.Add(UpgradeOption_NONE);
        selectedWeaponUpgrade = UpgradeOption_NONE;
        UpgradeOption_NONE.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("WeaponIcons/" + weapon.weaponModel);

        //check: already upgrade on?
        if (weapon.upgraded_magazine)
            selectedWeaponUpgrade = UpgradeOption_MAGAZINE;
        if (weapon.upgraded_scope)
            selectedWeaponUpgrade = UpgradeOption_SCOPE;
        if (weapon.upgraded_silencer)
            selectedWeaponUpgrade = UpgradeOption_SILENCER;
        if (weapon.upgraded_damage)
            selectedWeaponUpgrade = UpgradeOption_COMPENSATOR;
        if (weapon.upgraded_light)
            selectedWeaponUpgrade = UpgradeOption_LIGHT;
        if (weapon.upgraded_automatic)
            selectedWeaponUpgrade = UpgradeOption_AUTOMATIC;

        GameObject.Find("CarSelectionState_Text").GetComponent<Text>().text = "Upgrade " + weapon.weaponName.Replace("_WEAPON", "");
        if (weapon.upgrade_MagazineGO != null)
        {
            UpgradeOption_MAGAZINE.SetActive(true);
            weaponUpgradeList.Add(UpgradeOption_MAGAZINE);
            UpgradeOption_MAGAZINE.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("WeaponIcons/" + weapon.weaponModel + "+MAGAZINE");
        }
        else
            UpgradeOption_MAGAZINE.SetActive(false);
        if (weapon.upgrade_ScopeGO != null)
        {
            UpgradeOption_SCOPE.SetActive(true);
            weaponUpgradeList.Add(UpgradeOption_SCOPE);
            UpgradeOption_SCOPE.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("WeaponIcons/" + weapon.weaponModel + "+SCOPE");
        }
        else
            UpgradeOption_SCOPE.SetActive(false);
        if (weapon.upgrade_SilencerGO != null)
        {
            UpgradeOption_SILENCER.SetActive(true);
            weaponUpgradeList.Add(UpgradeOption_SILENCER);
            UpgradeOption_SILENCER.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("WeaponIcons/" + weapon.weaponModel + "+SILENCER");
        }
        else
            UpgradeOption_SILENCER.SetActive(false);
        if (weapon.upgrade_DamageGO != null)
        {
            UpgradeOption_COMPENSATOR.SetActive(true);
            weaponUpgradeList.Add(UpgradeOption_COMPENSATOR);
            UpgradeOption_COMPENSATOR.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("WeaponIcons/" + weapon.weaponModel + "+COMPENSATOR");
        }
        else
            UpgradeOption_COMPENSATOR.SetActive(false);
        if (weapon.upgrade_LightGO != null)
        {
            UpgradeOption_LIGHT.SetActive(true);
            weaponUpgradeList.Add(UpgradeOption_LIGHT);
            UpgradeOption_LIGHT.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("WeaponIcons/" + weapon.weaponModel + "+LIGHT");
        }
        else
            UpgradeOption_LIGHT.SetActive(false);
        if (weapon.upgrade_AutomaticGO != null)
        {
            UpgradeOption_AUTOMATIC.SetActive(true);
            weaponUpgradeList.Add(UpgradeOption_AUTOMATIC);
            UpgradeOption_AUTOMATIC.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("WeaponIcons/" + weapon.weaponModel + "+AUTOMATIC");
        }
        else
            UpgradeOption_AUTOMATIC.SetActive(false);

        showWeaponUpgrade(weapon);
    }

    void showWeaponUpgrade(CARnageWeapon weapon)
    {
        foreach(GameObject go in weaponUpgradeList)
        {
            go.GetComponentInChildren<Image>().enabled = false;
            go.GetComponentInChildren<Text>().color = new Color(1,1,1,50f/255f);
        }
        selectedWeaponUpgrade.GetComponentInChildren<Image>().enabled = true;
        selectedWeaponUpgrade.GetComponentInChildren<Text>().color = Color.white;

        weapon.removeAllUpgrades();
        switch(selectedWeaponUpgrade.name)
        {
            case "MAGAZINE":
                weapon.addUpgrade(CARnageWeapon.UpgradeTypes.MAGAZINE);
                break;
            case "SCOPE":
                weapon.addUpgrade(CARnageWeapon.UpgradeTypes.SCOPE);
                break;
            case "SILENCER":
                weapon.addUpgrade(CARnageWeapon.UpgradeTypes.SILENCER);
                break;
            case "COMPENSATOR":
                weapon.addUpgrade(CARnageWeapon.UpgradeTypes.COMPENSATOR);
                break;
            case "LIGHT":
                weapon.addUpgrade(CARnageWeapon.UpgradeTypes.LIGHT);
                break;
            case "AUTOMATIC":
                weapon.addUpgrade(CARnageWeapon.UpgradeTypes.AUTO);
                break;
        }
    }

    void selectNextWeaponUpgrade(CARnageWeapon weapon)
    {
        int index = weaponUpgradeList.IndexOf(selectedWeaponUpgrade);
        if (index == weaponUpgradeList.Count - 1)
            index = 0;
        else
            index++;
        selectedWeaponUpgrade = weaponUpgradeList[index];

        showWeaponUpgrade(weapon);
    }
    void selectPreviousWeaponUpgrade(CARnageWeapon weapon)
    {
        int index = weaponUpgradeList.IndexOf(selectedWeaponUpgrade);
        if (index == 0)
            index = weaponUpgradeList.Count - 1;
        else
            index--;
        selectedWeaponUpgrade = weaponUpgradeList[index];

        showWeaponUpgrade(weapon);
    }

    List<GameObject> modList;
    GameObject selectedMod;
    void selectModInit(int modPosition)
    {
        if(modPosition == 0)
            GameObject.Find("CarSelectionState_Text").GetComponent<Text>().text = "Select 1st Mod";
        else
            GameObject.Find("CarSelectionState_Text").GetComponent<Text>().text = "Select 2nd Mod";

        if (!select_mod_init)
        {
            // create list
            Array values = Enum.GetValues(typeof(CARnageModifier.ModID));
            modList = new List<GameObject>();
            foreach (CARnageModifier.ModID mod in values)
            {
                GameObject go = Instantiate(Resources.Load<GameObject>("ModIcon"), select_mod_GO.transform);
                GameObject MODgo = Instantiate(Resources.Load<GameObject>("MODSResources/" + mod), go.transform);

                go.GetComponentsInChildren<Image>()[1].sprite = MODgo.GetComponent<CARnageModifier>().image;

                modList.Add(go);
            }            
            select_mod_init = true;
        }

        // show selected 
        if(selectedMod)
        {
            selectedMod.GetComponentsInChildren<Image>()[0].color = new Color(50 / 255f, 50 / 255f, 50 / 255f);
            selectedMod.GetComponentsInChildren<Image>()[1].color = Color.white;
        }
        int i = 0;
        foreach (CARnageModifier cm in selectedCar.GetComponent<CarIcon>().rel_car.GetComponent<CARnageCar>().getModController().getMods())
        {
            if (modPosition == i)
            {
                foreach (GameObject go in modList)
                {
                    if (go.GetComponentInChildren<CARnageModifier>().modID == cm.modID)
                    {
                        selectedMod = go;
                        break;
                    }
                }
            }
            i++;
            if (i >= 2)
                break;
        }

        showSelectedMod();
    }

    void showSelectedMod()
    {
        //foreach (GameObject go in modList)
        //{
        //    go.GetComponentsInChildren<Image>()[0].color = new Color(50 / 255f, 50 / 255f, 50 / 255f);
        //    go.GetComponentsInChildren<Image>()[1].color = Color.white;
        //}
        selectedMod.GetComponentsInChildren<Image>()[0].color = Color.white;
        selectedMod.GetComponentsInChildren<Image>()[1].color = new Color(50 / 255f, 50 / 255f, 50 / 255f);
    }

    void selectNextMod(string direction)
    {
        selectedMod.GetComponentsInChildren<Image>()[0].color = new Color(50 / 255f, 50 / 255f, 50 / 255f);
        selectedMod.GetComponentsInChildren<Image>()[1].color = Color.white;

        int index = modList.IndexOf(selectedMod);
        switch (direction)
        {
            case "right":
                index++;
                break;
            case "left":
                index--;
                break;
            case "up":
                index -= 10;
                break;
            case "down":
                index += 10;
                break;
        }
        if (index >= modList.Count)
            index = index % modList.Count;
        if (index < 0)
            index += modList.Count;
        selectedMod = modList[index];

        GameObject MODgo = Instantiate(Resources.Load<GameObject>("MODSResources/" + selectedMod.GetComponentInChildren<CARnageModifier>().modID), selectedCar.GetComponent<CarIcon>().rel_car.GetComponent<CARnageCar>().getModController().transform);
        if (currentState == CarSelectionState.SELECT_MOD_1)
        {
            DestroyImmediate(selectedCar.GetComponent<CarIcon>().rel_car.GetComponent<CARnageCar>().getModController().getMods()[0].gameObject);
            MODgo.transform.SetAsFirstSibling();
        }
        if (currentState == CarSelectionState.SELECT_MOD_2)
        {
            DestroyImmediate(selectedCar.GetComponent<CarIcon>().rel_car.GetComponent<CARnageCar>().getModController().getMods()[1].gameObject);
            MODgo.transform.SetAsLastSibling();
        }
        displayMods(selectedCar.GetComponent<CarIcon>().rel_car);
        showSelectedMod();        
    }

    void finishSelection()
    {
        // save preferred selection
        switch(selectingPlayer)
        {
            case "Player0":
                PlayerPrefs.SetString(selectingPlayer + "_controlledBy", "MouseKeyboard");
                break;
            case "Player1":
                PlayerPrefs.SetString(selectingPlayer + "_controlledBy", "Controller1");
                break;
            case "Player2":
                PlayerPrefs.SetString(selectingPlayer + "_controlledBy", "Controller2");
                break;
            case "Player3":
                PlayerPrefs.SetString(selectingPlayer + "_controlledBy", "Controller3");
                break;
        }
        
        PlayerPrefs.SetString(selectingPlayer+"_Car", selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().carModel.ToString());
        PlayerPrefs.SetString(selectingPlayer+"_CarColor_" + selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().carModel.ToString(), selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().carColor.ToString());
        if(selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().getWeaponController().getLeftWeapon())
            PlayerPrefs.SetString(selectingPlayer+"_WeaponUpgrade_" + selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().getWeaponController().getLeftWeapon().weaponName, selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().getWeaponController().getLeftWeapon().getFirstUpgrade().ToString());
        
        if (selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().getWeaponController().getRightWeapon())
            PlayerPrefs.SetString(selectingPlayer+"_WeaponUpgrade_" + selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().getWeaponController().getRightWeapon().weaponName, selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().getWeaponController().getRightWeapon().getFirstUpgrade().ToString());
        PlayerPrefs.SetString(selectingPlayer+"_CarMod_" + selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().carModel.ToString() + "_0", selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().getModController().getMods()[0].modID.ToString());
        PlayerPrefs.SetString(selectingPlayer+"_CarMod_" + selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().carModel.ToString() + "_1", selectedCar.GetComponentInChildren<CarIcon>().rel_car.GetComponent<CARnageCar>().getModController().getMods()[1].modID.ToString());

        SceneManager.LoadScene("PLAYER_SELECTION");
    }
}

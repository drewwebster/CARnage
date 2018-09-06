using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

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
    
    List<GameObject> carList;
    GameObject selectedCar;
    int gridX = 0;
    int gridY = 0;
    GameObject[,] carGrid;

    // Use this for initialization
    void Start () {
        // create list
        carGrid = new GameObject[10, 10];
        Array values = Enum.GetValues(typeof(CarModel));
        GameObject carIconBG = GameObject.Find("TransBG_Cars_Grid");
        carList = new List<GameObject>();
        foreach (CarModel model in values)
        {
            GameObject go = Instantiate(Resources.Load<GameObject>("CarIcon"), carIconBG.transform);

            go.GetComponentsInChildren<Image>()[1].sprite = Resources.Load<Sprite>("CarIcons/" + model);
            GameObject CARgo = Instantiate(Resources.Load<GameObject>(model.ToString()), go.transform);

            CARgo.transform.parent = GameObject.Find("CAR_PREVIEW_SPAWN").transform;

            //CARgo.GetComponent<CARnageCar>().controlledBy = CARnageAuxiliary.ControllerType.NONE;
            CARgo.GetComponent<CARnageCar>().enabled = false;
            CARgo.GetComponent<RCC_CarControllerV3>().enabled = false;
            CARgo.GetComponent<CARnageCar>().isIndestructible = true;

            foreach (Rigidbody rb in CARgo.GetComponentsInChildren<Rigidbody>())
                rb.useGravity = false;
            foreach (Rigidbody rb in CARgo.GetComponentsInChildren<Rigidbody>())
                rb.isKinematic = true;
            CARgo.transform.localPosition = Vector3.zero;

            //foreach (CARnageWeapon weapon in carGO.GetComponent<CARnageCar>().getWeaponController().getAllWeapons())
            //    weapon.gameObject.SetActive(false);
            foreach (scaleEmission se in CARgo.GetComponentsInChildren<scaleEmission>())
                se.gameObject.SetActive(false);
            Destroy(GameObject.Find("WorldCanvasHPShieldNitro"));

            //go.GetComponentInChildren<Text>().text = CARgo.GetComponent<CARnageCar>().carName;
            
            carList.Add(go);
            go.GetComponent<CarIcon>().rel_car = CARgo;
            CARgo.SetActive(false);

            //GameObject carGO = CarFactory.spawnCar(model, position);
            //carGO.transform.Rotate(0, 180, 0);
            //position += Vector3.right * 3;
            //carGO.GetComponent<CARnageCar>().enabled = false;
            //foreach (Rigidbody rb in carGO.GetComponentsInChildren<Rigidbody>())
            //    rb.useGravity = false;
            //foreach (CARnageWeapon weapon in carGO.GetComponent<CARnageCar>().getWeaponController().getAllWeapons())
            //    weapon.gameObject.SetActive(false);
        }

        // display
        currSortOrder = SortOrder.ARRAY;
        sortCars();



        // show selected car
        selectedCar = carList[0];
        showSelectedCar();

    }

    SortOrder currSortOrder;
    void sortCars()
    {
        carGrid = new GameObject[10,10];
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
            carGrid[x, y] = go;
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
        //selectedCar.GetComponentInChildren<Text>().enabled = true;
        //Debug.Log("Instantiate: " + selectedCar.GetComponentInChildren<CARnageCar>().carModel);
        //GameObject carGO = Instantiate(Resources.Load<GameObject>(car.carModel.ToString()), GameObject.Find("CAR_PREVIEW_SPAWN").transform);
        GameObject carGO = selectedCar.GetComponent<CarIcon>().rel_car;
        carGO.SetActive(true);
        CARnageCar car = carGO.GetComponent<CARnageCar>();
        GameObject.Find("CarName_Text").GetComponent<Text>().text = car.carName;

        CARnageModifier[] mods = car.GetComponentsInChildren<CARnageModifier>();
        if (mods != null && mods.Length > 0)
        {
            GameObject.Find("ModLImage").GetComponent<Image>().sprite = mods[0].image;
            GameObject.Find("ModLName").GetComponent<Text>().text = mods[0].modName;
            GameObject.Find("ModLText").GetComponent<Text>().text = CARnageAuxiliary.colorMe(mods[0].description);
        }
        else
        {
            GameObject.Find("ModLImage").GetComponent<Image>().sprite = null;
            GameObject.Find("ModLName").GetComponent<Text>().text = "";
            GameObject.Find("ModLText").GetComponent<Text>().text = "";
        }
        if (mods != null && mods.Length > 1)
        {
            GameObject.Find("ModRImage").GetComponent<Image>().sprite = mods[1].image;
            GameObject.Find("ModRName").GetComponent<Text>().text = mods[1].modName;
            GameObject.Find("ModRText").GetComponent<Text>().text = CARnageAuxiliary.colorMe(mods[1].description);
        }
        else
        {
            GameObject.Find("ModRImage").GetComponent<Image>().sprite = null;
            GameObject.Find("ModRName").GetComponent<Text>().text = "";
            GameObject.Find("ModRText").GetComponent<Text>().text = "";
        }
        displayWeapon(car.getWeaponController().getLeftWeapon(), "L");
        displayWeapon(car.getWeaponController().getRightWeapon(), "R");

        int i = 1;
        foreach (Image img in GameObject.Find("SpeedBlocks").GetComponentsInChildren<Image>())
            if (img.name.Equals("Filled"))
            {
                if (car.speed >= i)
                    img.enabled = true;
                else
                    img.enabled = false;
                i++;
            }
        i = 1;
        foreach (Image img in GameObject.Find("ImpactBlocks").GetComponentsInChildren<Image>())
            if (img.name.Equals("Filled"))
            {
                if (car.impact >= i)
                    img.enabled = true;
                else
                    img.enabled = false;
                i++;
            }
        i = 1;
        foreach (Image img in GameObject.Find("StorageBlocks").GetComponentsInChildren<Image>())
            if (img.name.Equals("Filled"))
            {
                if (car.gearStorage >= i)
                    img.enabled = true;
                else
                    img.enabled = false;
                i++;
            }
        i = 1;
        foreach (Image img in GameObject.Find("HPBlocks").GetComponentsInChildren<Image>())
            if (img.name.Equals("Filled"))
            {
                if (car.HP >= i)
                    img.enabled = true;
                else
                    img.enabled = false;
                i++;
            }
        i = 1;
        foreach (Image img in GameObject.Find("ShieldBlocks").GetComponentsInChildren<Image>())
            if (img.name.Equals("Filled"))
            {
                if (car.shield >= i)
                    img.enabled = true;
                else
                    img.enabled = false;
                i++;
            }
        i = 1;
        foreach (Image img in GameObject.Find("NitroBlocks").GetComponentsInChildren<Image>())
            if (img.name.Equals("Filled"))
            {
                if (car.nitro >= i)
                    img.enabled = true;
                else
                    img.enabled = false;
                i++;
            }
    }

    public void displayWeapon(CARnageWeapon weapon, string handSide)
    {
        if(weapon != null)
        {
            string dmgType = "";
            string delayType = "";
            GameObject.Find("Weapon" + handSide + "Name").GetComponent<Text>().text = weapon.weaponName.Replace("_WEAPON","");
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
            string delayAuto = weapon.automatic ? " <size=9>(auto)</size>" : "";
            string magazineStr = weapon.magazineSize > 0 ? "Magazine:\r\n" : "";
            string magazineSize = weapon.magazineSize > 0 ? weapon.magazineSize + "\r\n" : "";
            GameObject.Find("Weapon" + handSide + "StatsText").GetComponent<Text>().text = weapon.Damage + "<size=11>" + dmgType + "</size>" + "\r\n" + weapon.shotDelay + "<size=9> sec</size>" + delayAuto + "\r\n" + magazineSize + reloadTime;
            GameObject.Find("Weapon" + handSide + "Text").GetComponent<Text>().text = "DMG:\r\n" + delayType + "\r\n" + magazineStr + reloadStr;
            GameObject.Find("Weapon" + handSide + "ExtraText").GetComponent<Text>().text = CARnageAuxiliary.colorMe(weapon.getExtraDescription());
        }
        else
        {
            GameObject.Find("Weapon" + handSide + "Name").GetComponent<Text>().text = "";
            GameObject.Find("Weapon" + handSide + "StatsText").GetComponent<Text>().text = "";
            GameObject.Find("Weapon" + handSide + "Text").GetComponent<Text>().text = "";
            GameObject.Find("Weapon"+handSide+"ExtraText").GetComponent<Text>().text = "";
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
            selectedCar.GetComponentInChildren<Image>().color = new Color(50 / 255f, 50 / 255f, 50 / 255f);
            selectNextCar("right");
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selectedCar.GetComponentInChildren<Image>().color = new Color(50 / 255f, 50 / 255f, 50 / 255f);
            selectNextCar("left");
        }
    }

    void selectNextCar(string direction)
    {
        switch(direction)
        {
            case "right":
                gridX++;
                Debug.Log("trying to show pos: " + gridX + ", " + gridY);
                if (gridX == carGrid.GetLength(0))
                {
                    gridX = 0;
                    gridY++;
                }
                if (gridY == carGrid.GetLength(0))
                {
                    gridY = 0;
                }
                break;
            case "left":
                gridX--;
                Debug.Log("trying to show pos: " + gridX + ", " + gridY);
                if (gridX < 0)
                {
                    gridX = carGrid.GetLength(0)-1;
                    gridY--;
                }
                if (gridY < 0)
                {
                    gridY = carGrid.GetLength(0)-1;
                }
                break;
        }
        if (carGrid[gridX, gridY] != null)
        {
            //selectedCar.GetComponentInChildren<Text>().enabled = false;
            //Destroy(GameObject.Find("CAR_PREVIEW_SPAWN").GetComponentInChildren<CARnageCar>().gameObject);
            selectedCar.GetComponent<CarIcon>().rel_car.SetActive(false);
            selectedCar = carGrid[gridX, gridY];
            showSelectedCar();
        }
        else
            selectNextCar(direction);
    }
}

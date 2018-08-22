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

	// Use this for initialization
	void Start () {
        // create list

        Array values = Enum.GetValues(typeof(CarModel));
        GameObject carIconBG = GameObject.Find("TransBG_Cars");
        carList = new List<GameObject>();
        foreach (CarModel model in values)
        {
            Debug.Log("spawning: " + model);
            GameObject go = Instantiate(Resources.Load<GameObject>("CarIcon"), carIconBG.transform);

            go.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("CarIcons/" + model);
            GameObject CARgo = Instantiate(Resources.Load<GameObject>(model.ToString()), go.transform);
            //CARgo.SetActive(false);

            go.GetComponentInChildren<Text>().text = CARgo.GetComponent<CARnageCar>().carName;
            
            carList.Add(go);

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
        currSortOrder = SortOrder.NAME;
        sortCars();

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
            if(go.GetComponent<CARnageCar>())
            {
                //go.transform.Rotate(0, 180, 0);
                go.GetComponent<CARnageCar>().enabled = false;
                go.GetComponent<RCC_CarControllerV3>().enabled = false;

                foreach (Rigidbody rb in go.GetComponentsInChildren<Rigidbody>())
                    rb.useGravity = false;
                foreach (CARnageWeapon weapon in go.GetComponent<CARnageCar>().getWeaponController().getAllWeapons())
                    weapon.gameObject.SetActive(false);
                foreach (scaleEmission se in go.GetComponentsInChildren<scaleEmission>())
                    se.gameObject.SetActive(false);
                Destroy(GameObject.Find("WorldCanvasHPShieldNitro"));
            }

    }

    SortOrder currSortOrder;
    void sortCars()
    {
        switch(currSortOrder)
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
        foreach (GameObject go in carList)
        {
            go.transform.localPosition = Vector3.zero;
            go.transform.position += new Vector3(x * 125, -y * 125);
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
            return g1.GetComponentInChildren<CARnageCar>().speed.CompareTo(g2.GetComponentInChildren<CARnageCar>().speed);
        }
    }
    public class impactSorter : IComparer<GameObject>
    {
        public int Compare(GameObject g1, GameObject g2)
        {
            return g1.GetComponentInChildren<CARnageCar>().impact.CompareTo(g2.GetComponentInChildren<CARnageCar>().impact);
        }
    }
    public class storageSorter : IComparer<GameObject>
    {
        public int Compare(GameObject g1, GameObject g2)
        {
            return g1.GetComponentInChildren<CARnageCar>().gearStorage.CompareTo(g2.GetComponentInChildren<CARnageCar>().gearStorage);
        }
    }
    public class hpSorter : IComparer<GameObject>
    {
        public int Compare(GameObject g1, GameObject g2)
        {
            return g1.GetComponentInChildren<CARnageCar>().HP.CompareTo(g2.GetComponentInChildren<CARnageCar>().HP);
        }
    }
    public class shieldSorter : IComparer<GameObject>
    {
        public int Compare(GameObject g1, GameObject g2)
        {
            return g1.GetComponentInChildren<CARnageCar>().shield.CompareTo(g2.GetComponentInChildren<CARnageCar>().shield);
        }
    }
    public class nitroSorter : IComparer<GameObject>
    {
        public int Compare(GameObject g1, GameObject g2)
        {
            return g1.GetComponentInChildren<CARnageCar>().nitro.CompareTo(g2.GetComponentInChildren<CARnageCar>().nitro);
        }
    }
    public class colorSorter : IComparer<GameObject>
    {
        public int Compare(GameObject g1, GameObject g2)
        {
            return g1.GetComponentInChildren<CARnageCar>().carColor.ToString().CompareTo(g2.GetComponentInChildren<CARnageCar>().carColor.ToString());
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            switch(currSortOrder)
            {
                case SortOrder.NAME:
                    currSortOrder = SortOrder.SPEED;
                    break;
                case SortOrder.SPEED:
                    currSortOrder = SortOrder.IMPACT;
                    break;
                case SortOrder.IMPACT:
                    currSortOrder = SortOrder.STORAGE;
                    break;
                case SortOrder.STORAGE:
                    currSortOrder = SortOrder.HP;
                    break;
                case SortOrder.HP:
                    currSortOrder = SortOrder.SHIELD;
                    break;
                case SortOrder.SHIELD:
                    currSortOrder = SortOrder.NITRO;
                    break;
                case SortOrder.NITRO:
                    currSortOrder = SortOrder.COLOR;
                    break;
                case SortOrder.COLOR:
                    currSortOrder = SortOrder.NAME;
                    break;
            }
            sortCars();
        }
    }
}

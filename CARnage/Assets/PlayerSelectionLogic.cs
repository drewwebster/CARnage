using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerSelectionLogic : MonoBehaviour {

    public Sprite carUnknownSprite;

	// Use this for initialization
	void Start () {
        if (PlayerPrefs.GetString("Player0_controlledBy").Equals(""))
            displayPlayerInactive(0);
        else
            displayPlayer(0);
        if (PlayerPrefs.GetString("Player1_controlledBy").Equals(""))
            displayPlayerInactive(1);
        else
            displayPlayer(1);
        if (PlayerPrefs.GetString("Player2_controlledBy").Equals(""))
            displayPlayerInactive(2);
        else
            displayPlayer(2);
        if (PlayerPrefs.GetString("Player3_controlledBy").Equals(""))
            displayPlayerInactive(3);
        else
            displayPlayer(3);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            PlayerPrefs.SetString("selectingPlayer", "Player0");
            SceneManager.LoadScene("CAR_SELECTION");
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayerPrefs.SetString("selectingPlayer", "Player1");
            SceneManager.LoadScene("CAR_SELECTION");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayerPrefs.SetString("selectingPlayer", "Player2");
            SceneManager.LoadScene("CAR_SELECTION");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PlayerPrefs.SetString("selectingPlayer", "Player3");
            SceneManager.LoadScene("CAR_SELECTION");
        }
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            SceneManager.LoadScene("SCENE_SELECTION_RANDOM");
    }

    void displayPlayer(int playerNr)
    {
        Transform playerTrans = GameObject.Find("Player" + playerNr).transform;
        
        GameObject carGO = CarFactory.spawnCarForPlayer("Player" + playerNr, null, false);
        CARnageAuxiliary.FindDeepChild(playerTrans,"CarIcon").GetComponentsInChildren<Image>()[1].sprite = Resources.Load<Sprite>("CarIcons/" + PlayerPrefs.GetString("Player" + playerNr + "_Car"));
        
        if(PlayerPrefs.GetString("Player" + playerNr + "_Car").Equals("RANDOM_CAR"))
        {
            displayPlayerInactive(playerNr);
            CARnageAuxiliary.FindDeepChild(playerTrans, "CarName_Text").GetComponent<Text>().text = "RANDOM CAR";
            CARnageAuxiliary.FindDeepChild(playerTrans, "CarIcon").GetComponentsInChildren<Image>()[0].color = new Color(50f / 255f, 50f / 255f, 50f / 255f);
            CARnageAuxiliary.FindDeepChild(playerTrans, "CarIcon").GetComponentsInChildren<Image>()[1].color = Color.white;
            CARnageAuxiliary.FindDeepChild(playerTrans, "CarIcon").GetComponentsInChildren<Image>()[1].sprite = Resources.Load<Sprite>("CarIcons/RANDOM_CAR");
            CARnageAuxiliary.FindDeepChild(playerTrans, "CarNamePanel").GetComponent<Image>().color = new Color(50f / 255f, 50f / 255f, 50f / 255f);
            CARnageAuxiliary.FindDeepChild(playerTrans, "CarNamePanel").GetComponentInChildren<Text>().color =  Color.white;
        }
        else
        {
            CARnageCar car = carGO.GetComponent<CARnageCar>();
            CARnageAuxiliary.FindDeepChild(playerTrans, "CarName_Text").GetComponent<Text>().text = car.carName;

            CarSelectionLogic.displayMods(carGO, playerTrans);
            CarSelectionLogic.displayWeapon(car.getWeaponController().getLeftWeapon(), "L", playerTrans);
            CarSelectionLogic.displayWeapon(car.getWeaponController().getRightWeapon(), "R", playerTrans);
            CarSelectionLogic.displayCarStats(car, playerTrans);
        }

        carGO.SetActive(false);
    }

    void displayPlayerInactive(int playerNr)
    {
        Color inactiveColor = new Color(1, 1, 1, 100f / 255f);
        Transform playerTrans = GameObject.Find("Player" + playerNr).transform;
        CARnageAuxiliary.FindDeepChild(playerTrans, "CarIcon").GetComponentsInChildren<Image>()[0].color = inactiveColor;
        CARnageAuxiliary.FindDeepChild(playerTrans, "CarIcon").GetComponentsInChildren<Image>()[1].sprite = carUnknownSprite;
        CARnageAuxiliary.FindDeepChild(playerTrans, "CarIcon").GetComponentsInChildren<Image>()[1].color = new Color(1, 1, 1, 50f / 255f);
        CARnageAuxiliary.FindDeepChild(playerTrans, "CarIcon").GetComponentsInChildren<Image>()[1].transform.localScale = Vector3.one;
        CARnageAuxiliary.FindDeepChild(playerTrans, "CarNamePanel").GetComponent<Image>().color = inactiveColor;
        CARnageAuxiliary.FindDeepChild(playerTrans, "CarNamePanel").GetComponentInChildren<Text>().text = "PRESS "+playerNr+" TO JOIN";
        CARnageAuxiliary.FindDeepChild(playerTrans, "CarNamePanel").GetComponentInChildren<Text>().color = new Color(100f / 255f, 100f / 255f, 100f / 255f, 200f / 255f);
        CARnageAuxiliary.FindDeepChild(playerTrans, "CarStatsPanel").GetComponent<Image>().color = inactiveColor;
        foreach (Text t in CARnageAuxiliary.FindDeepChild(playerTrans, "CarStatsPanel").GetComponentsInChildren<Text>())
            t.enabled = false;
        foreach (Image i in CARnageAuxiliary.FindDeepChild(playerTrans, "CarStatsPanel").GetComponentsInChildren<Image>())
            i.enabled = false;
        
        foreach (Image i in CARnageAuxiliary.FindDeepChild(playerTrans, "TransBG_ModLeft").GetComponentsInChildren<Image>())
            i.enabled = false;
        foreach (Text t in CARnageAuxiliary.FindDeepChild(playerTrans, "TransBG_ModLeft").GetComponentsInChildren<Text>())
            t.enabled = false;
        foreach (Image i in CARnageAuxiliary.FindDeepChild(playerTrans, "TransBG_ModRight").GetComponentsInChildren<Image>())
            i.enabled = false;
        foreach (Text t in CARnageAuxiliary.FindDeepChild(playerTrans, "TransBG_ModRight").GetComponentsInChildren<Text>())
            t.enabled = false;
        foreach (Image i in CARnageAuxiliary.FindDeepChild(playerTrans, "TransBG_WeaponLeft").GetComponentsInChildren<Image>())
            i.enabled = false;
        foreach (Text t in CARnageAuxiliary.FindDeepChild(playerTrans, "TransBG_WeaponLeft").GetComponentsInChildren<Text>())
            t.enabled = false;
        foreach (Image i in CARnageAuxiliary.FindDeepChild(playerTrans, "TransBG_WeaponRight").GetComponentsInChildren<Image>())
            i.enabled = false;
        foreach (Text t in CARnageAuxiliary.FindDeepChild(playerTrans, "TransBG_WeaponRight").GetComponentsInChildren<Text>())
            t.enabled = false;
    }
}

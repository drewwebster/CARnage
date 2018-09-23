using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnLogic : MonoBehaviour {

    public Transform[] spawns;
    List<GameObject> carList;
    GameObject UI;

    // Use this for initialization
    void Start ()
    {
        if (spawns.Length == 0)
            Debug.LogWarning("WARN: No spawns on this map.");
        carList = CarFactory.spawnCarsForAllPlayers(true, spawns);

        for(int i = 0; i < CARnageAuxiliary.getPlayersPlayingCount(); i++)
        {
            carList[i].GetComponent<CARnageCar>().relPlayerUI = GameObject.Find("UI_Player" + i).GetComponent<PlayerUI>();
            GameObject.Find("UI_Player" + i).GetComponent<PlayerUI>().init(carList[i].GetComponent<CARnageCar>());
        }
        UI = GameObject.Find("CARnageMultiplayerCanvas");
        UI.SetActive(false);
        Invoke("startRound", 4f);
    }
	
	public void onCarDestroyed(GameObject car)
    {
        carList.Remove(car);
        if (carList.Count == 0)
            SceneManager.LoadScene("SCENE_SELECTION_RANDOM");
    }

    private void Update()
    {
        if (!CARnageAuxiliary.isStarted && Input.GetKeyDown(KeyCode.Escape))
            startRound();
    }
    
    public void startRound()
    {
        if (CARnageAuxiliary.isStarted)
            return;
        UI.SetActive(true);
        CARnageAuxiliary.isStarted = true;
        GameObject.Find("ZoomCamera").SetActive(false);
    }
}

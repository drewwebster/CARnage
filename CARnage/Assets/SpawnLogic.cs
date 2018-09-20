using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnLogic : MonoBehaviour {

    public Transform[] spawns;
    List<GameObject> carList;

    // Use this for initialization
    void Start ()
    {
        if (spawns.Length == 0)
            Debug.LogWarning("WARN: No spawns on this map.");
        carList = CarFactory.spawnCarsForAllPlayers(true, spawns);

        for(int i = 0; i < CARnageAuxiliary.getPlayersPlayingCount(); i++)
            GameObject.Find("UI_Player" + i).GetComponent<PlayerUI>().init(carList[i].GetComponent<CARnageCar>());
    }
	
	public void onCarDestroyed(GameObject car)
    {
        carList.Remove(car);
        if (carList.Count == 0)
            SceneManager.LoadScene("SCENE_SELECTION_RANDOM");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSelection : MonoBehaviour {
    
	void Start () {
        CARnageAuxiliary.isStarted = false;
        List<string> possibleMaps = new List<string>();

        possibleMaps.Add("Level_Volcano");

        string mapToLoad = possibleMaps[Random.Range(0, possibleMaps.Count)];
        SceneManager.LoadScene(mapToLoad);
	}
	
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalUI : MonoBehaviour {

    public GameObject menuGO;

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape))
        {
            CARnageAuxiliary.togglePause();
            if (CARnageAuxiliary.isPaused) // PAUSE
            {
                foreach (PlayerUI pui in GetComponentsInChildren<PlayerUI>())
                    pui.onPauseScreen();
                menuGO.SetActive(true);
                Time.timeScale = 0;
            }
            else          // CONTINUE
            {
                foreach (PlayerUI pui in GetComponentsInChildren<PlayerUI>())
                    pui.onPauseScreenEnd();
                menuGO.SetActive(false);
                Time.timeScale = 1;
            }
        }
        if (Input.GetKeyDown(KeyCode.Backspace) && CARnageAuxiliary.isPaused)
        {
            SceneManager.LoadScene("PLAYER_SELECTION");
            Time.timeScale = 1;
        }

    }
}

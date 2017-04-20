using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoBuildShortcuts : MonoBehaviour {

	void Update ()
    {
		if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
        {
            // Reset Game
            SceneManager.LoadScene(0);
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Escape))
        {
            // Quit Game
            Application.Quit();
        }
    }
}

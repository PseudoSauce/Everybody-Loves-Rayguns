using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickCommands : MonoBehaviour {

	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            Application.LoadLevel(Application.loadedLevel);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
	}
}

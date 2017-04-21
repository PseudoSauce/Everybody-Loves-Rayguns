using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoBuildShortcuts : MonoBehaviour {

    public bool m_isMainMenu = false;
    public int m_hubIndex = 1;
    public int m_mainMenuIndex = 5;
    public GameObject m_Instructions;

	void Update ()
    {
        if (m_isMainMenu)
        {
            if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
            {
                // Start Game
                SceneManager.LoadScene(m_hubIndex);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                // Reset Game
                SceneManager.LoadScene(m_mainMenuIndex);
            }

            if(m_Instructions != null)
            {
                if (Input.GetKey(KeyCode.T))
                {
                    // Toggle Instructions
                    if(m_Instructions.activeInHierarchy == true)
                    {
                        m_Instructions.SetActive(false);
                    }
                    else
                    {
                        m_Instructions.SetActive(true);
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Quit Game
            Debug.Log("Quitting...");
            Application.Quit();
        }
    }
}

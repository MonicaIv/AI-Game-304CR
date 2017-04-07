using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class menuScript : MonoBehaviour {

    public Button startText;
    public Button exitText;

	void Start ()
    {
        startText = startText.GetComponent<Button>();
        exitText = exitText.GetComponent<Button>();
    }

    //public void ExitPress()
    //{
    //    startText.enabled = false;
    //    exitText.enabled = false;
    //}

    public void StartLevel()
    {
        // SceneManager.LoadScene("Pathfinding");
        Application.LoadLevel(1);
        print("starting");
    }
	
    public void ExitGame()
    {
        Application.Quit();
    }

}

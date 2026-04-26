using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour 
{

	public string sceneName;
	private bool IAmEnabled = false;
	// Use this for initialization
	void Start () 
	{
		IAmEnabled = false;
		Invoke("EnableMe",1.5f);
	}
	
	void EnableMe()
    {
		IAmEnabled = true;
    }
	// Update is called once per frame
	void Update () 
	{
		if (IAmEnabled)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
				Application.Quit();
			else if (Input.anyKeyDown)
			{
				EventRegistry.reinit();
				GameManager.reinit();
				HUDManager.reinit();
				if (sceneName != "")
					SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
			}
		}
	}
}

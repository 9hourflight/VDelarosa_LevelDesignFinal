using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventListener_ChangeLevel : MonoBehaviour 
{

	public string eventToListenFor;

    public string nextLevel; // = "GameOver";

	void Start () 
	{
		if(eventToListenFor != "")
		{
			EventRegistry.AddEvent(eventToListenFor, endLevelOnEvent, gameObject);
		}
	}
	

	void endLevelOnEvent (string eventName, GameObject obj) 
	{
        if ((obj != null) && (obj != this.gameObject))
            return;
        if (nextLevel != "")
		{
			SceneManager.LoadScene(nextLevel,LoadSceneMode.Single);
			CheckpointBehavior.ResetCheckpoints();
			NPCManager.Reset();
		}
		else
			Application.Quit();
	}
}

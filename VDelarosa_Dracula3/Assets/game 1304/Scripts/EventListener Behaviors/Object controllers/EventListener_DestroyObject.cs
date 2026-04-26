using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventListener_DestroyObject : MonoBehaviour 
{

    [Header("Event Listening")]
    public List<string> eventsToTriggerThis;

    void Start () 
	{
        foreach (string s in eventsToTriggerThis)
        {
            EventRegistry.AddEvent(s, DestroyObjectOnEvent, gameObject);
		}
	}
	

	void DestroyObjectOnEvent(string eventName, GameObject obj) 
	{
        if ((obj != null) && (obj != this.gameObject))
            return;
        Destroy(gameObject);
	}
}

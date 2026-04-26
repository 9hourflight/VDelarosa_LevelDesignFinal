using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class debugKeyEvent
{
    public KeyCode key;
    [FormerlySerializedAsAttribute("eventsToSend")]    
    public List<EventPackage> eventsToSend;
}

public class DebugKeyListener : MonoBehaviour
{
    public List<debugKeyEvent> debugKeyEvents;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	
	void Update ()
    {
        foreach (debugKeyEvent dke in debugKeyEvents)
        {
            if (Input.GetKeyDown(dke.key))
            {                
                foreach (EventPackage ep in dke.eventsToSend)
                    EventRegistry.SendEvent(ep,this.gameObject);
            }
        }
	}
}

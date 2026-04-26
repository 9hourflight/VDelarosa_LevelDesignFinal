using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class RelayPackage
{
	public string incomingEvent;
	public List<EventPackage> eventsToSend;
}
public class EventRelay : MonoBehaviour 
{
	[Header("Event Listening")]
	public List<RelayPackage> eventRelayPackages;	

    void Start () 
	{			
		foreach(RelayPackage rp in eventRelayPackages)
		{
			EventRegistry.AddEvent(rp.incomingEvent, ProcessEvent, gameObject);
		}		        
    }	

	public void ProcessEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        foreach(RelayPackage rp in eventRelayPackages)
		{
			if(rp.incomingEvent == eventName)
			{
				foreach(EventPackage ep in rp.eventsToSend)
				{
                    EventRegistry.SendEvent(ep, this.gameObject);
                }
			}
		}
			
	}

	
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class OnGameStartEventRelay : MonoBehaviour
{
    [Header("Event Sending")]
    public List<EventPackage> EventsToSendOnStart;
    public float delay = 0f;

    void Start()
    {
        Invoke("doEvents", 0);
    }

    void doEvents()
    {         
        foreach (EventPackage ep in EventsToSendOnStart)
            EventRegistry.SendEvent(ep, this.gameObject);
    }
	
	
}

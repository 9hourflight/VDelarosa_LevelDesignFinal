using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EventRelay_Gate : MonoBehaviour 
{
    [Header("Event Listening")]    
    [Tooltip("Any of these events will cause the gate's output events to be sent.")]
    public List<string> eventsToListenFor;
    [Tooltip("When this event is sent, the gate will close and not allow any signals through.")]
    public string eventToCloseGate;
    public string eventToOpenGate;
    public string eventToResetGateCount;

    [Header("Event Sending")]
    
    [FormerlySerializedAs("outputEvents")]
    //public List<string> eventsToSend;
    public List<EventPackage> eventsToSend;
	
    [Header("State Properties")]
    public bool startOpen = true;
    [Tooltip("Gate will close after this many signals. 0 means infinite.")]
	public int autoCloseCount = 0;
	private bool isOpen;
	private int gateUseCount = 0;

	void Start () 
	{
		isOpen = startOpen;		
        foreach(string s in eventsToListenFor)
            EventRegistry.AddEvent(s, gateInputOnEvent, gameObject);
        EventRegistry.AddEvent(eventToCloseGate, closeGateOnEvent, gameObject);
		EventRegistry.AddEvent(eventToOpenGate, openGateOnEvent, gameObject);
		EventRegistry.AddEvent(eventToResetGateCount, resetGateOnEvent, gameObject);
	}
	
    void closeGateOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        isOpen = false;
    }

    void openGateOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        isOpen = true;
    }

    void gateInputOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        if (isOpen)
        {
            foreach (EventPackage ep in eventsToSend)
            {
                EventRegistry.SendEvent(ep, this.gameObject);
            }
        }
        gateUseCount += 1;
        if ((gateUseCount >= autoCloseCount) && (autoCloseCount > 0))
        {
            isOpen = false;
        }
    }

    void resetGateOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        gateUseCount = 0;
    }	
		
}

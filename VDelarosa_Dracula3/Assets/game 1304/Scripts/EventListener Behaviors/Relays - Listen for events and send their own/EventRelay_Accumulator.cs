using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventRelay_Accumulator : MonoBehaviour 
{

    [Header("Event Listening")]
    [Tooltip("Listening for this event from the event manager.")]
    public List<string> eventsToListenFor;
    [Tooltip("This event resets the accumulator.")]
    public string eventToResetAccumulator;
    [Tooltip("Listening for the event to fire this many times before firing our own events.")]
    public int accumulationThreshold;
    [Tooltip("Set the count back to zero after the number is reached and the events are fired.")]
    public bool resetOnAccumulation;
    [Tooltip("Set this token to the value of the accumulator.")]
    public string tokenName;
    [Header("Event Sending")]
    public List<EventPackage> eventsToSend;
    
    
	private int currentAccumulatorCount;

	void Start () 
	{
		currentAccumulatorCount = 0;
        foreach(string s in eventsToListenFor)
		    EventRegistry.AddEvent(s, accumulateOnEvent, gameObject);
        EventRegistry.AddEvent(eventToResetAccumulator, ResetOnEvent, gameObject);
	}
	

	void accumulateOnEvent(string eventName, GameObject obj) 
	{
        if ((obj != null) && (obj != this.gameObject))
            return;
        currentAccumulatorCount += 1;
		if(currentAccumulatorCount == accumulationThreshold)
		{
			
            foreach (EventPackage ep in eventsToSend)
                EventRegistry.SendEvent(ep, this.gameObject);
            if (resetOnAccumulation)
				currentAccumulatorCount = 0;
		}

	}
    private void ResetOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        currentAccumulatorCount = 0;
    }
}

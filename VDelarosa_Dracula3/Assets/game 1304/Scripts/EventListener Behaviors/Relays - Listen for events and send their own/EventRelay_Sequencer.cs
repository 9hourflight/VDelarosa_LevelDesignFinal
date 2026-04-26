using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using System;

[System.Serializable]
public class sequenceEntry
{
    [Tooltip("Duration from the previous event to call this one.")]
    public float delay;
    public List<EventPackage> eventsToSend;    
}
public class EventRelay_Sequencer : MonoBehaviour
{
    public bool preventInterruption = true;

    [Tooltip("These events will initialize the sequencer")]
    public List<string> eventsToListenFor;
    public List<sequenceEntry> sequenceOfEvents;

    private float currentTimer;
    private bool timerActive = false;
    private int currentEntryIndex;
    [Tooltip("If this is true, only the next event in the list will be called each time. If it's false, then every list of events will be called in sequence.")]
    public bool onlyAdvanceOnEvent = true;
    [Tooltip("If this is true, once the last event list is called, it will start again with the first.")]
    public bool isLooping = false;
   
	// Use this for initialization
	void Start ()
    {
        foreach (string s in eventsToListenFor)
            EventRegistry.AddEvent(s,executeSequence, gameObject);
        currentEntryIndex = 0;
    }

    void Update()
    {        
        if(timerActive)
        {
            currentTimer += Time.deltaTime;
            if(currentTimer>= sequenceOfEvents[currentEntryIndex].delay)
            {
                currentTimer -= sequenceOfEvents[currentEntryIndex].delay;
                foreach (EventPackage ep in sequenceOfEvents[currentEntryIndex].eventsToSend)
                {
                    if (ep.scope == eventScope.privateEvent)
                    {
                        EventRegistry.SendEvent(ep.eventName, this.gameObject);
                    }
                    else
                        EventRegistry.SendEvent(ep.eventName);
                }
                currentEntryIndex++;
                if (currentEntryIndex>=sequenceOfEvents.Count)
                {
                    if (isLooping)
                        currentEntryIndex = 0;
                    else
                        timerActive = false;
                }
            }
        }
    }
    public void executeSequence(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        if (preventInterruption && timerActive)
            return;
        if (sequenceOfEvents.Count > 0)
        {
            if (onlyAdvanceOnEvent)
            {
                if (currentEntryIndex < sequenceOfEvents.Count)
                {                    
                    foreach (EventPackage ep in sequenceOfEvents[currentEntryIndex].eventsToSend)
                    {   
                        if(ep.scope == eventScope.privateEvent)
                        {
                            EventRegistry.SendEvent(ep.eventName,this.gameObject);
                        }
                        else
                            EventRegistry.SendEvent(ep.eventName);
                    }
                    currentEntryIndex++;
                    if (currentEntryIndex >= sequenceOfEvents.Count)
                    {
                        if (isLooping)
                            currentEntryIndex = 0;
                    }
                }
            }
            else
            {
                currentEntryIndex = 0;
                timerActive = true;
            }
        }
    }
}

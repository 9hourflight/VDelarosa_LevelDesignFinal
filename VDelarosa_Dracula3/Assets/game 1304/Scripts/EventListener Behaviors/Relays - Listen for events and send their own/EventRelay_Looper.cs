using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EventRelay_Looper : MonoBehaviour 
{
	public int loopCount;
	public string eventToListenFor;
    [Header ("Event Sending")]
	[FormerlySerializedAs("eventsToSend")]
    public List<EventPackage> eventsToSendDuringLoop;
    public List<EventPackage> eventsToSendWhenFinished;
    private int loopIndex=0;
	private bool isLooping = false;    
    // Use this for initialization
    void Start () 
	{
		EventRegistry.AddEvent(eventToListenFor, loopEvents, gameObject);
	}

	void loopEvents(string eventName, GameObject obj)
	{
        if ((obj != null) && (obj != this.gameObject))
            return;
        isLooping = true;
		loopIndex = 0;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(isLooping)
		{							
           foreach (EventPackage ep in eventsToSendDuringLoop)                
                EventRegistry.SendEvent(ep, this.gameObject);
            if (loopCount != -1)
            {
                loopIndex += 1;
				if (loopIndex > loopCount)
				{
					isLooping = false;
                    foreach (EventPackage ep in eventsToSendWhenFinished)
                        EventRegistry.SendEvent(ep, this.gameObject);
                }
            }
		}
	}
}

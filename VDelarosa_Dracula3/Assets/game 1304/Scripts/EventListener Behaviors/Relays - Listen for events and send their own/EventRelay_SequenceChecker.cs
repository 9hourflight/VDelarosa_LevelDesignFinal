using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventRelay_SequenceChecker : MonoBehaviour 
{

	public List<string> eventSequenceToCheck;
    public List<EventPackage> eventsToSendOnSequenceSuccess;
    public List<EventPackage> eventsToSendOnSequenceFail;

    public bool resetOnIncorrect = true;
	public bool resetOnCorrect = false;
	private int eventIndex = 0;
    [Header("Deprecated")]
    public List<string> eventsOnCorrectSequence;
    public List<string> eventsOnIncorrectSequence;
    void Start () 
	{
		foreach(string s in eventSequenceToCheck)
			EventRegistry.AddEvent(s, checkSequence, gameObject);
	}

	void checkSequence(string eventName, GameObject obj)
	{
		if(eventSequenceToCheck.Count <= 0)
			return;
		if(eventName == eventSequenceToCheck[eventIndex])
		{
			eventIndex += 1;
			if(eventIndex >= eventSequenceToCheck.Count)
			{
				foreach(string s in eventsOnCorrectSequence)
					EventRegistry.SendEvent(s);
                foreach (EventPackage ep in eventsToSendOnSequenceSuccess)
                    EventRegistry.SendEvent(ep, this.gameObject);
                if (resetOnCorrect)
					eventIndex = 0;
			}
		}
		else
		{
			foreach(string s in eventsOnIncorrectSequence)
				EventRegistry.SendEvent(s);
            foreach (EventPackage ep in eventsToSendOnSequenceFail)
                EventRegistry.SendEvent(ep,this.gameObject);
            
            if (resetOnIncorrect)
				eventIndex = 0;
		}
	}

	void Update () 
	{
		
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[System.Serializable]
public class randomEventEntry
{
    public int weight;
    public List<EventPackage> eventsToSend;
}

public class EventRelay_RandomChoice : MonoBehaviour
{    
    public List<string> eventsToListenFor;
    public List<randomEventEntry> randomEvents;

    private List<List<EventPackage>> eventPool;
    

    void Start()
    {
        int lcv;
        if (eventsToListenFor.Count > 0)
        {
            foreach (string s in eventsToListenFor)
            {
                if (s != "")
                    EventRegistry.AddEvent(s, makeRandomChoice, gameObject);
            }
        }
        eventPool = new List<List<EventPackage>>();
        foreach(randomEventEntry ree in randomEvents)
        {
            //TODO:Make this less clunky
            for(lcv=0;lcv<ree.weight;lcv++)
            {
                eventPool.Add(ree.eventsToSend);
            }
        }
    }

    void makeRandomChoice(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != gameObject))
            return;
        int index = Random.Range(0, eventPool.Count - 1);
        foreach(EventPackage ep in eventPool[index])
        {
            EventRegistry.SendEvent(ep, this.gameObject);
        }
        
    }

    

}

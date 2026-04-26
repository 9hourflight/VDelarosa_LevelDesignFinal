using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.VisualScripting;

//public delegate void eventListenerEvent(string eventName);
public delegate void eventListenerEvent(string eventName,GameObject obj);

public enum eventScope { publicEvent, privateEvent, siblingsOnly, childrenOnly,instigator, visualScriptingPrivate, visualScriptingInstigator, visualScriptingChildren, visualScriptingSiblings};

[System.Serializable]
//[SerializeField]
public class eventListenerData
{
    public eventListenerEvent eventEntry;
    public GameObject eventObject;
}

[System.Serializable]
//[SerializeField]
public class EventPackage
{
    public string eventName;
    public eventScope scope;
}


public static class EventRegistry
{
	private static bool isInitialized = false;
//	private static List<eventRegistryEntry> eventList;
	private static Dictionary<string, List<eventListenerData>> eventDictionary;

	public static void reinit()
	{
		isInitialized = false;
		Init ();
	}
	public static void Init() 
	{
		if (isInitialized) return;
		isInitialized = true;
		eventDictionary = new Dictionary<string, List<eventListenerData>> ();	
	}

	public static void AddEvent(string eventName, eventListenerEvent listenerEvent, GameObject obj)
	{
        eventListenerData elData;
        if (eventName == null)
            return;
        if (eventName == "")
			return;
        if (listenerEvent == null)
            return;
		Init();
		List<eventListenerData> eleList;
        elData = new eventListenerData()
        {
            eventEntry = listenerEvent,
            eventObject = obj
        };

        if (!eventDictionary.ContainsKey(eventName))
		{
			eleList = new List<eventListenerData>();
			eleList.Add(elData);
			eventDictionary.Add(eventName, eleList);	
		}
		else
		{
			eventDictionary[eventName].Add(elData);
		}
		
	}
    

    public static void SendEvent(EventPackage ep, GameObject obj)
    {
        Debug.Log("Sending: " + ep.eventName+" "+ep.scope.ToString()+" scope");
        switch (ep.scope)
        {
            case eventScope.childrenOnly:
                {
                    SendEvent(ep.eventName, obj);
                    foreach (Transform child in obj.transform)
                    {
                        SendEvent(ep, child.gameObject);
                    }
                }
                break;
            case eventScope.instigator:
            case eventScope.privateEvent:
                {
                    SendEvent(ep.eventName, obj);
                }
                break;
            case eventScope.visualScriptingInstigator:
            case eventScope.visualScriptingPrivate:
                {
                    CustomEvent.Trigger(obj, ep.eventName);
                }
                break;
            case eventScope.visualScriptingChildren:
                {
                    foreach (Transform child in obj.transform)
                    {
                        CustomEvent.Trigger(child.gameObject, ep.eventName);
                    }

                }
                break;
            case eventScope.publicEvent:
                {
                    SendEvent(ep.eventName);
                }
                break;
            case eventScope.visualScriptingSiblings:
                {
                    EventPackage newEP = new EventPackage();
                    newEP.eventName = ep.eventName;
                    newEP.scope = eventScope.visualScriptingChildren;
                    SendEvent(newEP, obj.transform.root.gameObject);
                    /*foreach (Transform child in obj.transform.root.transform)
                    {
                        SendEvent(ep.eventName, child.gameObject);
                    }*/
                }
                break;
            case eventScope.siblingsOnly:
                {
                    EventPackage newEP = new EventPackage();
                    newEP.eventName = ep.eventName;
                    newEP.scope = eventScope.childrenOnly;
                    SendEvent(newEP, obj.transform.root.gameObject);
                    /*foreach (Transform child in obj.transform.root.transform)
                    {
                        SendEvent(ep.eventName, child.gameObject);
                    }*/
                }
                break;
        }                    
    }

    public static void SendEvent(string eventName)
    {
        SendEvent(eventName, null);
    }

    public static void SendEvent(string eventName, GameObject obj)
	{
		if(eventName == "")
			return;
		int n;
		Debug.Log("Sending: "+eventName);
		if (eventDictionary.ContainsKey(eventName))
		{
			List<eventListenerData> elist = eventDictionary[eventName];		
			foreach(eventListenerData eld in elist)
			{
                if (eld.eventObject != null)
                {
                    //TODO: fix this so the receiver will state its gameobject name regardless of the delegate doing the receiving 
                    if (obj != null)
                        Debug.Log("Event received: " + eventName + " by " + obj.name);
                    else
                        Debug.Log("Event received: " + eventName);

                    eld.eventEntry(eventName, obj);
                }
			}
		}
	}


}

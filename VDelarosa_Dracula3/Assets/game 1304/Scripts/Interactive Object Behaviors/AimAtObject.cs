using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class EventTargetPair
{
    public GameObject target;
    public string eventName;
}

public class AimAtObject : MonoBehaviour
{

    public bool startEnabled = true;
    bool isEnabled;
    public GameObject target;
    [Header("Event Listening")]
    public List<string> eventsToEnable;
    public List<string> eventsToDisable;
    public List<EventTargetPair> newTargetEvents;
    GameObject currentTarget;

    //TODO: add maximum move speed
    void Start()
    {
        isEnabled = startEnabled;
        currentTarget = target;

       foreach(string s in eventsToEnable)
            EventRegistry.AddEvent(s, EnableOnEvent, gameObject);
        foreach (string s in eventsToDisable)
            EventRegistry.AddEvent(s, DisableOnEvent, gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(isEnabled)
        {
            transform.LookAt(target.transform);
        }
    }

    void EnableOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        isEnabled = true;
    }

    void DisableOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        isEnabled = false;
    }

    void ChangeTargetOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        foreach (EventTargetPair etp in newTargetEvents)
        {
            if(etp.eventName == eventName)
            {
                currentTarget = etp.target;
            }
        }
    }
}

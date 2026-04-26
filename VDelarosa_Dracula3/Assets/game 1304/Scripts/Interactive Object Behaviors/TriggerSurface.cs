using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TriggerSurface : MonoBehaviour
{
    

    [Header("Event Sending")]
    public List<EventPackage> EventsToSendOnTouch;
    public List<EventPackage> EventsToSendOnUnTouch;

    [Header("Event Listening")]
    public string eventToEnableThis;
    public string eventToDisableThis;

    public bool startEnabled = true;
    //public bool PlayerOnly = true;
    private bool isEnabled;
    public bool onlyTriggerOnPlayer = false;
    public List<GameObject> onlyTriggerOnTheseObjects;
    public int maxTriggerCount = 0;
    private int currentTriggerCount;
    
    void Start()
    {
        isEnabled = startEnabled;
        if (eventToEnableThis != "")
            EventRegistry.AddEvent(eventToEnableThis, enableThisOnEvent, gameObject);
        if (eventToDisableThis != "")
            EventRegistry.AddEvent(eventToDisableThis, disableThisOnEvent, gameObject);
        currentTriggerCount = 0;
    }

    void Update()
    {

    }

    public void enableThis()
    {
        isEnabled = true;
    }
    private void enableThisOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        enableThis();
    }

    public void disableThis()
    {
        isEnabled = false;
    }

    private void disableThisOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        disableThis();
    }

    private bool objectChecksOut(GameObject go)
    {
        if (onlyTriggerOnPlayer)
        {
            if (go == GameManager.player)
                return true;
            else
                return false;
        }

        if (onlyTriggerOnTheseObjects.Count > 0)
        {
            foreach (GameObject g in onlyTriggerOnTheseObjects)
            {
                if (go == g)
                    return true;
            }
            return false;
        }
        else
            return true;
    }

    void OnCollisionEnter(Collision collision)
    {        
        if (isEnabled)
        {
            if (objectChecksOut(collision.gameObject))
            {
                if (maxTriggerCount > 0)
                {
                    currentTriggerCount += 1;
                    if (currentTriggerCount > maxTriggerCount)
                        isEnabled = false;
                }
                if (EventsToSendOnTouch.Count > 0)
                {
                    foreach (EventPackage ep in EventsToSendOnTouch)
                    {
                        if((ep.scope == eventScope.instigator)||(ep.scope == eventScope.visualScriptingInstigator))
                            EventRegistry.SendEvent(ep, collision.gameObject);
                        else
                            EventRegistry.SendEvent(ep, this.gameObject);
                    }
                }
            }
        }
    }

    void OnCollisionExit(Collision collision)
     {
        if (isEnabled)
        {
            if (objectChecksOut(collision.gameObject))
            {
                if (EventsToSendOnUnTouch.Count > 0)
                {
                    foreach (EventPackage ep in EventsToSendOnUnTouch)
                    {
                        if ((ep.scope == eventScope.instigator) || (ep.scope == eventScope.visualScriptingInstigator))
                            EventRegistry.SendEvent(ep, collision.gameObject);
                        else
                            EventRegistry.SendEvent(ep, this.gameObject);
                    }
                }
            }
        }
    }

}

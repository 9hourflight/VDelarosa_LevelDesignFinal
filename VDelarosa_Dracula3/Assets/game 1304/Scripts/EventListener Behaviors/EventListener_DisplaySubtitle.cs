using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class subtitlePackage
{
    public string text;
    [Tooltip("If left at 0, duration will be calculated.")]
    public float duration;
    
}

public class EventListener_DisplaySubtitle : MonoBehaviour
{    
    [Header("Event Listening")]
    public List<string> eventsToListenFor;
    [TextArea(15, 5)]
    public string textToDisplay;
    public float duration;
    //public bool useAutomatedDuration;    
   

    void Start()
    {
        if (eventsToListenFor.Count > 0)
        {
            foreach (string s in eventsToListenFor)
            {
                if (s != "")
                    EventRegistry.AddEvent(s, displaySubtitleOnEvent, gameObject);
            }
        }        
        
    }

    private void displaySubtitleOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != gameObject))
            return;
        SubtitleManager.DisplaySubtitle(textToDisplay, duration); //, useAutomatedDuration);
    }

    

}

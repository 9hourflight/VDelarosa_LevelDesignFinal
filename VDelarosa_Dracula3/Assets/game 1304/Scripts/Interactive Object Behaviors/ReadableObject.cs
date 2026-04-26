using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadableObject : MonoBehaviour 
{
    [Header("Text Content and Formatting")]
    public string titleText;	
    [TextArea(15, 10)]
    public string bodyText;
    public Color titleFontColor = Color.white;
    public Color bodyFontColor = Color.white;
    public Color backgroundColor = Color.white;
    public int bodyFontSize = 18;
    public bool isFullScreen = false;
    [Header("Ineraction")]
    public string readingVerbLabel = "Read: ";	
    [HideInInspector]
    public bool isEnabled;
    public bool onlyTriggerOnce = false;
    [Header("Event Sending")]
    public List<EventPackage> EventsToSendOnRead;
    [Header("Event Listening")]
    public string eventToForcePopup;
	public string eventToEnable;
	public string eventToDisable;
	public bool startEnabled = true;    

    void Start () 
	{
		isEnabled = startEnabled;
		if(eventToForcePopup != "")
		{
			EventRegistry.AddEvent(eventToForcePopup, forcePopup, gameObject);
		}	
		if (eventToEnable != "")
		{
			EventRegistry.AddEvent (eventToEnable, enableThis, gameObject);
		}
		if (eventToDisable != "")
		{
			EventRegistry.AddEvent (eventToDisable, disableThis, gameObject);
		}
	}


	public void enableThis(string eventName, GameObject obj)
	{
        if ((obj != null) && (obj != this.gameObject))
            return;
        isEnabled = true;
	}

	public void disableThis(string eventName, GameObject obj)
	{
        if ((obj != null) && (obj != this.gameObject))
            return;
        isEnabled = false;
	}

	void forcePopup(string eventName, GameObject obj)
	{
        if ((obj != null) && (obj != this.gameObject))
            return;
        if (isEnabled)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
                return;
            ObjectInteractionBehavior oib = player.GetComponent<ObjectInteractionBehavior>();
            if (oib == null)
                return;
            oib.forceRead(this);
        }
	}

		
}

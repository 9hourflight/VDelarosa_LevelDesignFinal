using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//public enum countDirection {CountUp,CountDown};


[Serializable]
public class timerModificationEntry
{
	public string eventName;
	public float amount;
	public operationType operation;
}

public class EventRelay_Timer : MonoBehaviour 
{
	[Header("Event Listening")]
	public string startTimerEvent;
	public string pauseTimerEvent;
	public string resetTimerEvent;
	public List<timerModificationEntry> timerModificationEvents;
	[Header("Timer properties")]
	public bool resetOnStart = true;
    
	//public GameObject timerUITextPrefab;
	//public GameObject timerCanvas;
	//private Text timerUIText;

	
	public float timerDuration;
	[Header("Event sending")]
	public List<EventPackage> eventsToSend;

	[Header("HUD Settings")]
	public bool showTimerOnHUD;
	public string timerLabel = "Timer: ";
	public float timerYOffset = 0f;
	

	private bool timerIsTicking;
	private float currentTime;
	private float frameAccumulator;
    	
    
    private Text timerUIText;

    void Start () 
	{	
		//GameObject HUDTextObject;
		if(startTimerEvent != "")
		{
			EventRegistry.AddEvent(startTimerEvent, startTimer, gameObject);
		}
		if(pauseTimerEvent != "")
		{
			EventRegistry.AddEvent(pauseTimerEvent  , pauseTimer, gameObject);
		}
		if(resetTimerEvent != "")
		{
			EventRegistry.AddEvent(resetTimerEvent  , resetTimer, gameObject);
		}

		foreach(timerModificationEntry tme in timerModificationEvents)
		{
			EventRegistry.AddEvent(tme.eventName, modifyTimer, gameObject);
		}

		timerIsTicking = false;
		currentTime = 0;
		frameAccumulator = 0;

        
        if (showTimerOnHUD)
		{
            //timerUI
            //HUDTextObject = Instantiate(timerUITextPrefab, timerCanvas.transform);
            // HUDTextObject.GetComponent<Text>();
            timerUIText = GameManager.player.GetComponent<GAME1304PlayerController>().timerText;
            if (timerUIText != null)
            {
                timerUIText.enabled = false;             
            }
			//HUDTextObject.transform.position += new Vector3(0, timerYOffset, 0);
		}
        

    }

	public void modifyTimer(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        if (timerIsTicking)
		{
			foreach(timerModificationEntry tme in timerModificationEvents)
			{
				if(tme.eventName == eventName)
				{
					switch(tme.operation)
					{
					case operationType.add:
						currentTime -= tme.amount;
						break;
					case operationType.multiply:
						currentTime /= tme.amount;
						break;
					case operationType.set:
						currentTime = timerDuration-tme.amount;
						break;
					case operationType.subtract:
						currentTime += tme.amount;
						break;
					}
				}
			}
		}
	}

	public void startTimer(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        if ((!timerIsTicking)||(timerIsTicking && resetOnStart))
		{
			timerIsTicking = true;
			if(timerUIText != null)
				timerUIText.enabled = true;
			currentTime = 0;
			frameAccumulator = 0;
		}
			
	}

	public void pauseTimer(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        timerIsTicking = false;
	}

	public void resetTimer(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        timerIsTicking = false;
		currentTime = 0;
		if(timerUIText != null)
			timerUIText.enabled = false;
	}

	void timerExpires()
	{
		timerIsTicking = false;
		currentTime = 0;		

        foreach (EventPackage ep in eventsToSend)
            EventRegistry.SendEvent(ep, this.gameObject);

        if (timerUIText != null)
			timerUIText.enabled = false;
	}

	void Update () 
	{
		if(timerIsTicking)
		{				
			currentTime += Time.deltaTime;
			frameAccumulator += Time.deltaTime;
			if(frameAccumulator >= 0.5f)
			{
				if (timerUIText != null)
				{
					//TODO: support multiple timers
					//if (timerUIText.text == "")
						timerUIText.text = timerLabel + (timerDuration - Mathf.Round(currentTime)).ToString();
					/*else
                    {
						timerUIText.text += "\n" + timerLabel + (timerDuration - Mathf.Round(currentTime)).ToString();
					}*/

				}
				frameAccumulator = 0;
			}
			if(currentTime >= timerDuration)
			{
				timerExpires();
			}
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EventListener_Stopwatch : MonoBehaviour 
{
	[Header("Event Listening")]
	public string startTimerEvent;
	public string pauseTimerEvent;
	public string resetTimerEvent;
	public string addLapEvent;
	public string showTimerEvent;
	public string hideTimerEvent;
	
	[Header("Timer properties")]
	public bool resetOnStart = true;
	public string tokenToModify;

	[Header("HUD Settings")]
	public bool showStopwatchOnHUD = true;
	public string timerLabel = "Timer: ";
	public float timerYOffset = 0f;
	public string lapLabel = "Lap ";
	public int labelSize = 24;
	private List<float> laps;
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
		if (addLapEvent != "")
		{
			EventRegistry.AddEvent(addLapEvent, AddLapOnEvent, gameObject);
		}

		timerIsTicking = false;
		currentTime = 0;
		frameAccumulator = 0;
		laps = new List<float>();

        if (showStopwatchOnHUD)
		{
            //timerUI
            //HUDTextObject = Instantiate(timerUITextPrefab, timerCanvas.transform);
            // HUDTextObject.GetComponent<Text>();
            timerUIText = GameManager.player.GetComponent<GAME1304PlayerController>().timerText;
            if (timerUIText != null)
            {
                timerUIText.enabled = false;
				timerUIText.fontSize = labelSize;
            }
			//HUDTextObject.transform.position += new Vector3(0, timerYOffset, 0);
		}
        
    }

	public void startTimer(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        if ((!timerIsTicking)||(timerIsTicking && resetOnStart))
		{
			timerIsTicking = true;
			if((timerUIText != null)&&(showStopwatchOnHUD))
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
		/*if(timerUIText != null)
			timerUIText.enabled = false;*/
	}
	
	public void AddLapOnEvent(string eventName, GameObject obj)
    {
		if ((obj != null) && (obj != this.gameObject))
			return;
		laps.Add(currentTime);
	}

	public static string MakeHoursMinutesSeconds(float inputSeconds)
    {		
		int minutes = (int)inputSeconds / 60;
		int seconds = (int)inputSeconds % 60;
		int hours = minutes / 60;
		minutes = minutes % 60;
		return PrependAZero(hours)+":"+PrependAZero(minutes)+":"+PrependAZero(seconds);
    }

	public static string PrependAZero(int num)
    {
		if (num < 10)
			return "0" + num;
		else
			return num.ToString();
    }
	void Update () 
	{
		if(timerIsTicking)
		{				
			currentTime += Time.deltaTime;
			frameAccumulator += Time.deltaTime;
			if(frameAccumulator >= 0.5f)
			{
				TokenRegistry.modifyToken(tokenToModify, (int)Mathf.Round(currentTime), operationType.set);
				if (timerUIText != null)
				{
					timerUIText.text = timerLabel + MakeHoursMinutesSeconds(Mathf.Round(currentTime)).ToString();
					foreach(float f in laps)
                    {
						timerUIText.text += "\n" + lapLabel + MakeHoursMinutesSeconds(f);
                    }
				}
				frameAccumulator = 0;
			}
		
		}
	}
}

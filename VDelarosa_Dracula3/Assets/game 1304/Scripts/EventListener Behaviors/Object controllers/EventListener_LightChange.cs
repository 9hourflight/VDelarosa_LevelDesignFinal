using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LightColorEntry
{
	public string eventName;
	public bool changeColor = true;
	public Color colorChange;
	public float timeToChange = 0f;
	public bool changeIntensity = false;
	public float newIntensity;
	
}

[RequireComponent(typeof(Light))]
public class EventListener_LightChange : MonoBehaviour 
{
	[Tooltip("Events to listen for that will turn the light on.")]
	public string TurnOnEventName;
	[Tooltip("Events to listen for that will turn the light off.")]
	public string TurnOffEventName;
	[Tooltip("Events to listen for that will toggle the state of the light, off->on, on->off.")]
	public string ToggleEventName;
	[Tooltip("Events to listen for that will change the color of the light.")]
	public  List<LightColorEntry> ColorEvents;
	[Tooltip("Determines whether or not the light is off at the start of the level")]
	public bool startOn = true;
	// Use this for initialization
	private Light _light;
	private bool isChanging;
	private Color prevColor;
	private Color nextColor;
	private float prevIntensity;
	private float nextIntensity;
	private float currentTransitionTime;
	private float transitionDuration;

	void Start () 
	{
		_light = GetComponent<Light>();


		foreach(LightColorEntry lce in ColorEvents)
		{
			EventRegistry.AddEvent(lce.eventName, LightColor, gameObject);
		}

		if (TurnOnEventName != "")
		{
			EventRegistry.AddEvent (TurnOnEventName, LightOn, gameObject);
		}

		if (TurnOffEventName != "")
		{
			EventRegistry.AddEvent (TurnOffEventName, LightOff, gameObject);
		}

		if (ToggleEventName != "")
		{
			EventRegistry.AddEvent (ToggleEventName, LightToggle, gameObject);
		}
		if(_light != null)
			_light.enabled = startOn;
			
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(isChanging)
        {
			currentTransitionTime += Time.deltaTime;
			_light.color = Color.Lerp(prevColor,nextColor,currentTransitionTime/transitionDuration);
			_light.intensity = Mathf.Lerp(prevIntensity, nextIntensity, currentTransitionTime / transitionDuration);
		}
		if (currentTransitionTime >= transitionDuration)
			isChanging = false;
	}

	void LightOff(string eventName, GameObject obj)
	{
        if ((obj != null) && (obj != gameObject))
            return;
        _light.enabled = false;
	}

	void LightOn(string eventName, GameObject obj)
	{
        if ((obj != null) && (obj != gameObject))
            return;
        _light.enabled = true;
	}

	void LightToggle(string eventName, GameObject obj)
	{
        if ((obj != null) && (obj != gameObject))
            return;
        _light.enabled = !_light.enabled;
	}

	void LightColor(string eventName, GameObject obj)
	{
        if ((obj != null) && (obj != gameObject))
            return;
        foreach (LightColorEntry lce in ColorEvents)
		{
			if (lce.eventName == eventName)
			{
				if (lce.timeToChange == 0)
				{
					if (lce.changeColor)
						_light.color = lce.colorChange;
					if (lce.changeIntensity)
						_light.intensity = lce.newIntensity;
					isChanging = false;
				}
				else
				{
					isChanging = true;
					transitionDuration = lce.timeToChange;
					currentTransitionTime = 0;
					prevColor = _light.color;
					if (lce.changeColor)
					{
						nextColor = lce.colorChange;
					}
					else
					{
						nextColor = prevColor;
					}
					prevIntensity = _light.intensity;
					if (lce.changeIntensity)
					{
						nextIntensity = lce.newIntensity;
					}
					else
					{
						nextIntensity = prevIntensity;
					}
				}
			}
		}
		
	}
}
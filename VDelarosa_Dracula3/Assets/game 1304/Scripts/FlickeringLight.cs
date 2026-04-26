using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{

    public float minimumIntensity = 0;
    public float maximumIntensity = 1;
    public bool flickerRadius = false;
    public float minimumRadius;
    public float maximumRadius;
    private Light _light;    
    public float minimumSmoothing = 0;
    public float maximumSmoothing = 0;
    private float smoothingCounter;
    private float smoothingInterval;
    private float nextIntensity, nextRange;
    private float oldIntensity, oldRange;

    // Use this for initialization
    void Start ()
    {
        _light = GetComponent<Light>();
        
        resetSmoothing();
	}
	
    private void resetSmoothing()
    {
        smoothingInterval = UnityEngine.Random.Range(minimumSmoothing, maximumSmoothing);
        smoothingCounter = smoothingInterval;
        oldIntensity = _light.intensity;
        oldRange = _light.range;
        nextIntensity = UnityEngine.Random.Range(minimumIntensity, maximumIntensity);
        nextRange =  UnityEngine.Random.Range(minimumRadius, maximumRadius);
    }
	// Update is called once per frame
	void Update ()
    {
		if(_light != null)
        {

            _light.intensity = Mathf.Lerp(oldIntensity,nextIntensity, Mathf.InverseLerp(0, smoothingInterval, smoothingCounter));
            if (flickerRadius)
                _light.range = Mathf.Lerp(oldRange, nextRange, Mathf.InverseLerp(0, smoothingInterval, smoothingCounter));

            if (smoothingCounter <= 0)
            {                 
                resetSmoothing();
            }
            else
                smoothingCounter -= Time.deltaTime;
        }
	}
}

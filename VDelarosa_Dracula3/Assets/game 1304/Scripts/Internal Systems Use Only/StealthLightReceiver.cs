using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthLightReceiver : MonoBehaviour
{
    public bool SetLightsAtStart = false;
    public bool isLightBasedStealthActive = false;
    public string tokenToOutputLightValueTo;
    private List<StealthLightBehavior> affectingLights;
    private List<string> lightContributions;
    private float _visibilityValue;
    private Light skyLight;
    Vector3 skylightAngle;
    private LayerMask antiPlayerMask;

    public float visibilityValue
    {
        get { return _visibilityValue; }
    }

    void Start()
    {
        antiPlayerMask =~ LayerMask.GetMask("Entity");
        _visibilityValue = 0f;
        affectingLights = new List<StealthLightBehavior>();
        
        foreach (Light sl in FindObjectsOfType<Light>())
        {
            if (SetLightsAtStart)
            {
                if (!sl.GetComponent<StealthLightBehavior>())
                {
                    sl.gameObject.AddComponent<StealthLightBehavior>();
                    sl.GetComponent<StealthLightBehavior>().Init();
                }
            }
            if(sl.type == LightType.Directional)
            {
                skyLight = sl;
                skylightAngle = sl.transform.forward; //.eulerAngles;
            }
        }
        
        
    }
    
    public void AddLight(StealthLightBehavior slb)
    {
        affectingLights.Add(slb);
    }

    public void RemoveLight(StealthLightBehavior slb)
    {
        affectingLights.Remove(slb);
    }

    void Update()
    {
        if (isLightBasedStealthActive)
        {
            //iterate through the lights
            //calculate a total visibility based on the aggregate of affecting lights
            //Average? Total? Highest?
            _visibilityValue = 0;
            lightContributions = new List<string>();
            foreach (StealthLightBehavior slb in affectingLights)
            {
                if (slb.visibilityContribution > 0)
                    lightContributions.Add(slb.gameObject.name + " : " + slb.visibilityContribution);
                else
                {
                    lightContributions.Add(slb.gameObject.name + " Blocked" + slb.getBlockerName());
                }
                if (slb.visibilityContribution > _visibilityValue)
                    _visibilityValue = slb.visibilityContribution;
            }
            //see if we're hit by the sun/moon
            if (skyLight != null)
            {
                if (!Physics.Raycast(transform.position, -skylightAngle, 100000f, antiPlayerMask, QueryTriggerInteraction.Ignore))
                {
                    if (skyLight.intensity > _visibilityValue)
                        _visibilityValue = skyLight.intensity;
                }
            }
            if (tokenToOutputLightValueTo != "")
                TokenRegistry.setToken(tokenToOutputLightValueTo, (int)Mathf.Clamp(_visibilityValue * 100, 0, 100));
        }
    }
}

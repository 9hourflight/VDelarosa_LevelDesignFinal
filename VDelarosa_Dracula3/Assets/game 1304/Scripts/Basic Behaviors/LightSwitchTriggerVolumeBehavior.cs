using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class LightSwitchTriggerVolumeBehavior : MonoBehaviour
{
    public List<GameObject> lights;
        
    [Header("On Enter Behavior")]    
    
    public lightInteractionModes interactionModeOnEnter = lightInteractionModes.toggleOnOff;
    [Header("On Exit Behavior")]
    public lightInteractionModes interactionModeOnExit = lightInteractionModes.toggleOnOff;

    public bool playerOnly = true;

    void OnTriggerEnter(Collider other)
    {
        if (playerOnly && other.gameObject.GetComponent<GAME1304PlayerController>() == null)
            return;
        if (other.gameObject.GetComponent<GAME1304PlayerController>() != null)
        {
            foreach (GameObject l in lights)
            {
                if (l != null)
                {
                    Light lb = l.GetComponent<Light>();
                    if (lb != null)
                    {
                        switch(interactionModeOnEnter)
                        {
                            case lightInteractionModes.toggleOnOff:
                                lb.enabled = !lb.enabled;
                                break;
                            case lightInteractionModes.turnOff:
                                lb.enabled = false;
                                break;
                            case lightInteractionModes.turnOn:
                                lb.enabled = true;
                                break;
                        }                        
                    }
                }
            }
        }
        //base.OnTriggerEnter();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<GAME1304PlayerController>() != null)
        {
            foreach (GameObject l in lights)
            {
                if (l != null)
                {
                    Light lb = l.GetComponent<Light>();
                    if (lb != null)
                    {
                        switch (interactionModeOnExit)
                        {
                            case lightInteractionModes.toggleOnOff:
                                lb.enabled = !lb.enabled;
                                break;
                            case lightInteractionModes.turnOff:
                                lb.enabled = false;
                                break;
                            case lightInteractionModes.turnOn:
                                lb.enabled = true;
                                break;
                        }
                    }
                }
            }
        }
        //base.OnTriggerEnter();
    }
}

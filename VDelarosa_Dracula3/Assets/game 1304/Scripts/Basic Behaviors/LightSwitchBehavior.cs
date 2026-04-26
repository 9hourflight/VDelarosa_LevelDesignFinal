using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum lightInteractionModes {doNothing, turnOn, turnOff, toggleOnOff };
public class LightSwitchBehavior : InteractiveObject
{
    public List<GameObject> lights;
    public lightInteractionModes interactionMode = lightInteractionModes.toggleOnOff;

    public override void interact()
    {
        base.interact();
        if (UseOnce && _used)
            return;
        if (!isEnabled)
            return;
        foreach (GameObject l in lights)
        {
            if (l != null)
            {
                Light lb = l.GetComponent<Light>();
                if (lb != null)
                {
                    switch (interactionMode)
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
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum teleporterInteractionModes { doNothing,turnOn,turnOff,toggleOnOff};

public class TeleporterButtonBehavior : InteractiveObject
{
    public List<TeleporterVolume> teleporterVolumes;
    public teleporterInteractionModes interactionMode;

    public override void interact()
    {        
        base.interact();
        if (UseOnce && _used)
            return;
        if (!isEnabled)
            return;
        foreach (TeleporterVolume tv in teleporterVolumes)
        {
            if (tv != null)
            {             
                tv.ProcessInteraction(interactionMode);                
            }
        }
    }
}

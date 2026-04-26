using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum conveyorInteractionModes { doNothing,reverse,turnOn,turnOff,toggleOnOff};

public class TreadmillButtonBehavior : InteractiveObject
{
    public List<ConveyorBehavior> treadmills;
    public conveyorInteractionModes interactionMode;

    public override void interact()
    {
        base.interact();
        if (UseOnce && _used)
            return;
        if (!isEnabled)
            return;
        foreach (ConveyorBehavior tb in treadmills)
        {
            if (tb != null)
            {
                
                    tb.processsInteraction(interactionMode);
                

            }
        }
    }
}

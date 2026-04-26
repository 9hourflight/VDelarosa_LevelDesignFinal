using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum moverInteractionModes {doNothing,goToA,goToB,goToNextStop,turnOn,turnOff,toggleOnOff,resetToA,resetToB};

public class MovingPlatformButtonBehavior : InteractiveObject
{
    public List<GameObject> movingPlatforms;
    public moverInteractionModes interactionMode;

    public override void interact()
    {
        base.interact();
        /*if (UseOnce && _used)
            return;
        if (!isEnabled)
            return;*/
        foreach (GameObject mp in movingPlatforms)
        {
            if (mp != null)
            {
                MoverBehavior mb = mp.GetComponent<MoverBehavior>();
                if (mb != null)
                {
                    mb.processInteractionInput(interactionMode);
                }

                AdvancedMoverBehavior amb = mp.GetComponent<AdvancedMoverBehavior>();
                if (amb != null)
                {
                    amb.processInteractionInput(interactionMode);
                }

                RotatingMoverBehavior rmb = mp.GetComponent<RotatingMoverBehavior>();
                if(rmb!=null)
                {
                    rmb.processInteractionInput(interactionMode);
                }

            }
        }
    }
}

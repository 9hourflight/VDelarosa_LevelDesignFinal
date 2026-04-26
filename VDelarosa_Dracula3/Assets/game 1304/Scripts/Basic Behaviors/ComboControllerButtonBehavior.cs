using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class lightInteractionPackage
{
    public Light light;
    public lightInteractionModes interactionMode;
}

[Serializable]
public class movingPlatformInteractionPackage
{
    public GameObject movingPlatform;
    public moverInteractionModes moverInteractionMode;
}

[Serializable]
public class teleporterInteractionPackage
{
    public TeleporterVolume teleporter;
    public teleporterInteractionModes interactionMode;
}

[Serializable]
public class conveyorInteractionPackage
{
    public ConveyorBehavior conveyor;    
    public conveyorInteractionModes interactionMode;
}

[Serializable]
public class doorInteractionPackage
{
    public GameObject door;
    public doorInteractionMode interactionMode = doorInteractionMode.unlockDoor;
}

[Serializable]
public class laserInteractionPackage
{
    public LaserEmitter laserEmitter;
    public laserInteractionModes interactionMode;
}


public class ComboControllerButtonBehavior : InteractiveObject
{
    
    [Header("Lights")]
    public List<lightInteractionPackage> lights;    

    [Header("Moving Platforms")]
    public List<movingPlatformInteractionPackage> movingPlatforms;
    
    [Header("Conveyor Belts")]
    public List<conveyorInteractionPackage> conveyors;

    [Header("Teleporters")]
    public List<teleporterInteractionPackage> teleporters;

    [Header("Doors")]
    public List<doorInteractionPackage> doors;

    [Header("Laser Emitters")]
    public List<laserInteractionPackage> lasers;

    public override void interact()
    {
        DoorknobBehavior dkb;
        base.interact();
        if (UseOnce && _used)
            return;
        if (!isEnabled)
            return;
        foreach (lightInteractionPackage lip in lights)
        {
            if (lip != null)
            {
                if (lip.light != null)
                {

                    switch (lip.interactionMode)
                    {
                        case lightInteractionModes.toggleOnOff:
                            lip.light.enabled = !lip.light.enabled;
                            break;
                        case lightInteractionModes.turnOff:
                            lip.light.enabled = false;
                            break;
                        case lightInteractionModes.turnOn:
                            lip.light.enabled = true;
                            break;
                    }

                }
            }
        }
        foreach (movingPlatformInteractionPackage mpip in movingPlatforms)
        {
            if (mpip != null)
            {
                GameObject mp = mpip.movingPlatform;
                if (mp != null)
                {
                    MoverBehavior mb = mp.GetComponent<MoverBehavior>();
                    if (mb != null)
                    {
                        mb.processInteractionInput(mpip.moverInteractionMode);
                    }

                    AdvancedMoverBehavior amb = mp.GetComponent<AdvancedMoverBehavior>();
                    if (amb != null)
                    {
                        amb.processInteractionInput(mpip.moverInteractionMode);
                    }

                    RotatingMoverBehavior rmb = mp.GetComponent<RotatingMoverBehavior>();
                    if (rmb != null)
                    {
                        rmb.processInteractionInput(mpip.moverInteractionMode);
                    }

                }
            }
        }
        foreach (conveyorInteractionPackage cip in conveyors)
        {
            if (cip != null)
            {
                if (cip.conveyor != null)
                {
                    cip.conveyor.processsInteraction(cip.interactionMode);
                }
            }
        }
        foreach (teleporterInteractionPackage tpip in teleporters)
        {
            if (tpip != null)
            {
                if (tpip.teleporter != null)
                {
                    tpip.teleporter.ProcessInteraction(tpip.interactionMode);
                }
            }
        }
        foreach (doorInteractionPackage dip in doors)
        {
            dkb = dip.door.GetComponent<DoorknobBehavior>();
            if (dkb == null)
                dkb = dip.door.GetComponentInChildren<DoorknobBehavior>();
            if (dkb != null)
            {
                affectDoor(dkb, dip.interactionMode);
            }
        }
        foreach (laserInteractionPackage lip in lasers)
        {
            if (lip != null)
            {
                if (lip.laserEmitter != null)
                {
                    lip.laserEmitter.processInteraction(lip.interactionMode);
                }
            }
        }
    }
    private void affectDoor(DoorknobBehavior dkb, doorInteractionMode interactionMode)
    {
        if (dkb == null)
            return;
        switch (interactionMode)
        {
            case doorInteractionMode.closeDoor:
                dkb.closeThis("", null);
                break;
            case doorInteractionMode.forceCloseDoor:
                dkb.canOpenWithScriptIfLocked = true;
                dkb.closeThis("", null);
                break;
            case doorInteractionMode.forceOpenDoor:
                dkb.canOpenWithScriptIfLocked = true;
                dkb.OpenThis("", null);
                break;
            case doorInteractionMode.lockDoor:
                dkb.lockThis("", null);
                break;
            case doorInteractionMode.openDoor:
                dkb.OpenThis("", null);
                break;
            case doorInteractionMode.unlockDoor:
                dkb.unlockThis("", null);
                break;
        }
    }
}
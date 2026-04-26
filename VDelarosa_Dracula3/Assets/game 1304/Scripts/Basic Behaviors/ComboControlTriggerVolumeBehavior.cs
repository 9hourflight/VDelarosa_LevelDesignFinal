using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class ComboControlTriggerVolumeBehavior : MonoBehaviour
{
    public bool onlyTriggerOnPlayer = true;
    int contentsCount = 0;

    [Header("On Enter Behavior")]
    
    [Header("Lights")]
    public List<lightInteractionPackage> onEnterLights;

    [Header("Moving Platforms")]
    public List<movingPlatformInteractionPackage> onEnterMovingPlatforms;

    [Header("Conveyor Belts")]
    public List<conveyorInteractionPackage> onEnterConveyors;

    [Header("Teleporters")]
    public List<teleporterInteractionPackage> onEnterTeleporters;

    [Header("Doors")]
    public List<doorInteractionPackage> onEnterDoors;

    [Header("Laser Emitters")]
    public List<laserInteractionPackage> onEnterLasers;


    [Header("On Exit Behavior")]
    [Header("Lights")]
    public List<lightInteractionPackage> onExitLights;

    [Header("Moving Platforms")]
    public List<movingPlatformInteractionPackage> onExitMovingPlatforms;

    [Header("Conveyor Belts")]
    public List<conveyorInteractionPackage> onExitConveyors;

    [Header("Teleporters")]
    public List<teleporterInteractionPackage> onExitTeleporters;

    [Header("Doors")]
    public List<doorInteractionPackage> onExitDoors;

    [Header("Laser Emitters")]
    public List<laserInteractionPackage> onExitLasers;

    [Header("On Empty Behavior")]
    [Header("Lights")]
    public List<lightInteractionPackage> onEmptyLights;

    [Header("Moving Platforms")]
    public List<movingPlatformInteractionPackage> onEmptyMovingPlatforms;

    [Header("Conveyor Belts")]
    public List<conveyorInteractionPackage> onEmptyConveyors;

    [Header("Teleporters")]
    public List<teleporterInteractionPackage> onEmptyTeleporters;

    [Header("Doors")]
    public List<doorInteractionPackage> onEmptyDoors;

    [Header("Laser Emitters")]
    public List<laserInteractionPackage> onEmptyLasers;


    void OnTriggerEnter(Collider other)
    {

        if ((other.gameObject.GetComponent<GAME1304PlayerController>() != null)||(!onlyTriggerOnPlayer))
        {
            contentsCount++;
            foreach (lightInteractionPackage lip in onEnterLights)
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
            foreach (movingPlatformInteractionPackage mpip in onEnterMovingPlatforms)
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
            foreach (conveyorInteractionPackage cip in onEnterConveyors)
            {
                if (cip != null)
                {
                    if (cip.conveyor != null)
                    {
                        cip.conveyor.processsInteraction(cip.interactionMode);
                    }
                }
            }
            foreach (teleporterInteractionPackage tpip in onEnterTeleporters)
            {
                if (tpip != null)
                {
                    if (tpip.teleporter != null)
                    {
                        tpip.teleporter.ProcessInteraction(tpip.interactionMode);
                    }
                }
            }
            foreach (doorInteractionPackage dip in onEnterDoors)
            {
                DoorknobBehavior dkb = dip.door.GetComponent<DoorknobBehavior>();
                if (dkb == null)
                    dkb = dip.door.GetComponentInChildren<DoorknobBehavior>();
                if (dkb != null)
                {
                    affectDoor(dkb, dip.interactionMode);
                }
            }
            foreach (laserInteractionPackage lip in onEnterLasers)
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
        
        //base.OnTriggerEnter();
    }

    private void OnTriggerExit(Collider other)
    {
        if ((other.gameObject.GetComponent<GAME1304PlayerController>() != null) || (!onlyTriggerOnPlayer))
        {
            contentsCount--;
            if (contentsCount <= 0)
            {
                foreach (lightInteractionPackage lip in onEmptyLights)
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
                foreach (movingPlatformInteractionPackage mpip in onEmptyMovingPlatforms)
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
                foreach (conveyorInteractionPackage cip in onEmptyConveyors)
                {
                    if (cip != null)
                    {
                        if (cip.conveyor != null)
                        {
                            cip.conveyor.processsInteraction(cip.interactionMode);
                        }
                    }
                }
                foreach (teleporterInteractionPackage tpip in onEmptyTeleporters)
                {
                    if (tpip != null)
                    {
                        if (tpip.teleporter != null)
                        {
                            tpip.teleporter.ProcessInteraction(tpip.interactionMode);
                        }
                    }
                }
                foreach (doorInteractionPackage dip in onEmptyDoors)
                {
                    DoorknobBehavior dkb = dip.door.GetComponent<DoorknobBehavior>();
                    if (dkb == null)
                        dkb = dip.door.GetComponentInChildren<DoorknobBehavior>();
                    if (dkb != null)
                    {
                        affectDoor(dkb, dip.interactionMode);
                    }
                }
                foreach (laserInteractionPackage lip in onEmptyLasers)
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
            else
            {
                foreach (lightInteractionPackage lip in onExitLights)
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
                foreach (movingPlatformInteractionPackage mpip in onExitMovingPlatforms)
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
                foreach (conveyorInteractionPackage cip in onExitConveyors)
                {
                    if (cip != null)
                    {
                        if (cip.conveyor != null)
                        {
                            cip.conveyor.processsInteraction(cip.interactionMode);
                        }
                    }
                }
                foreach (teleporterInteractionPackage tpip in onExitTeleporters)
                {
                    if (tpip != null)
                    {
                        if (tpip.teleporter != null)
                        {
                            tpip.teleporter.ProcessInteraction(tpip.interactionMode);
                        }
                    }
                }
                foreach (doorInteractionPackage dip in onExitDoors)
                {
                    DoorknobBehavior dkb = dip.door.GetComponent<DoorknobBehavior>();
                    if (dkb == null)
                        dkb = dip.door.GetComponentInChildren<DoorknobBehavior>();
                    if (dkb != null)
                    {
                        affectDoor(dkb, dip.interactionMode);
                    }
                }
                foreach (laserInteractionPackage lip in onExitLasers)
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
        }
        //base.OnTriggerEnter();
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointBehavior : MonoBehaviour 
{

    [Header("Event Listening")]
    public List<string> eventsToTriggerThis;

    [Header("Event Sending")]
    public List<EventPackage> EventsToSendOnUse;
    public List<EventPackage> EventsToSendOnRespawnHere;
    public bool consumeOnUse = false;

    public Material dormantMaterial;
    public Material activeMaterial;
    
    public Transform respawnTransform;

    private static GameObject activeCheckpoint;    
    private static List<GameObject> checkpoints;

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

    // Use this for initialization
    void Awake () 
	{
		foreach(string s in eventsToTriggerThis)
        {
            EventRegistry.AddEvent(s, useCheckpointOnEvent, gameObject);
        }
        GetComponent<Renderer>().material = dormantMaterial;
        if (checkpoints == null)
        {
            ResetCheckpoints();
        }
        checkpoints.Add(gameObject);
	}
	
    public static void ResetCheckpoints()
    {
        checkpoints = new List<GameObject>();
        activeCheckpoint = null;
    }
	// Update is called once per frame
	void Update () 
	{
		
	}
    void useCheckpointOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        useCheckpoint();
    }

    public Transform getRespawnTransform()
    {
        if (respawnTransform != null)
            return respawnTransform;
        else
            return transform;
    }
    public void useCheckpoint()
    {
        activeCheckpoint = gameObject;
        foreach (GameObject go in checkpoints)
        {
            go.GetComponent<CheckpointBehavior>().UpdateCheckpointMaterial();
        }
        foreach(EventPackage ep in EventsToSendOnUse)
        {
            if(ep.scope == eventScope.instigator)
            {
                ep.scope = eventScope.privateEvent;
                EventRegistry.SendEvent(ep, GameManager.player); //TODO: remove hard coded reference and figure something out
            }
            else
                EventRegistry.SendEvent(ep,gameObject);
        }
        if (consumeOnUse)
        {
            checkpoints.Add(gameObject); //should this be remove?
            Destroy(gameObject);
        }
    }

    public void RespawnHere()
    {
        //the player will take care of its position update and all that
        foreach (EventPackage ep in EventsToSendOnRespawnHere)
        {
            if (ep.scope == eventScope.instigator)
            {
                ep.scope = eventScope.privateEvent;
                EventRegistry.SendEvent(ep, GameManager.player); //TODO: remove hard coded reference and figure something out
            }
            else
                EventRegistry.SendEvent(ep, gameObject);
        }
        RespawnObjectControl();
    }

    private void RespawnObjectControl()
    {
        DoorknobBehavior dkb;
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

    public void UpdateCheckpointMaterial()
    {

        if (gameObject == activeCheckpoint)
        {
            GetComponent<Renderer>().material = activeMaterial;
        }
        else
        {
            GetComponent<Renderer>().material = dormantMaterial;
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        GAME1304PlayerController pc = collision.gameObject.GetComponent<GAME1304PlayerController>();
        
        if (pc != null)
        {
            pc.setCheckpoint(this);
            HUDManager.notificationQueue.Enqueue("Checkpoint reached");

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GAME1304PlayerController pc = other.gameObject.GetComponent<GAME1304PlayerController>();

        if (pc != null)
        {
            pc.setCheckpoint(this);
            HUDManager.notificationQueue.Enqueue("Checkpoint reached");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.Serialization;

public enum doorInteractionMode {doNothing,openDoor,closeDoor,unlockDoor,lockDoor,forceOpenDoor,forceCloseDoor,toggleOpenClose,toggleLocked};

[Serializable]
public class DoorUnlockButtonBehavior : InteractiveObject
{
    public List<GameObject> doors;
    public doorInteractionMode interactionMode = doorInteractionMode.unlockDoor;

    [Tooltip("If true, this button will override the interact label with what it does (open, unlock, etc)")]
    public bool overrideInteractLabel = true;
   
    public override void Start()
    {
        base.Start();
        if (overrideInteractLabel)
        {
            switch (interactionMode)
            {
                case doorInteractionMode.closeDoor:
                    interactLabel = "Close Door";
                    break;
                case doorInteractionMode.forceCloseDoor:
                    interactLabel = "Close Door";
                    break;
                case doorInteractionMode.forceOpenDoor:
                    interactLabel = "Open Door";
                    break;
                case doorInteractionMode.lockDoor:
                    interactLabel = "Lock Door";
                    break;
                case doorInteractionMode.openDoor:
                    interactLabel = "Open Door";
                    break;
                case doorInteractionMode.unlockDoor:
                    interactLabel = "Unlock Door";
                    break;
                case doorInteractionMode.toggleOpenClose:
                    interactLabel = "Toggle Door";
                    break;
                case doorInteractionMode.toggleLocked:
                    interactLabel = "Toggle Door Lock";
                    break;
                default:
                    break;
            }
        }
         
    }

    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        if (doors.Count > 0)
        {
            switch(interactionMode)
            {
                case doorInteractionMode.closeDoor:
                    Handles.color = Color.blue;
                    break;
                case doorInteractionMode.forceCloseDoor:
                    Handles.color = Color.magenta;
                    break;
                case doorInteractionMode.forceOpenDoor:
                case doorInteractionMode.toggleOpenClose:
                    Handles.color = Color.white;
                    break;
                case doorInteractionMode.lockDoor:
                    Handles.color = Color.red;
                    break;
                case doorInteractionMode.openDoor:
                case doorInteractionMode.toggleLocked:
                    Handles.color = Color.yellow;
                    break;
                case doorInteractionMode.unlockDoor:
                    Handles.color = Color.green;
                    break;
            }

            foreach (GameObject door in doors)
            {
                if(door != null)                
                    Handles.DrawDottedLine(transform.position, door.transform.position, 2.0f);
            }            
            
        }
#endif
    }

    public override void interact()
    {
        DoorknobBehavior dkb;
        base.interact();
        if (UseOnce && _used)
            return;
        if (!isEnabled)
            return;
        //TODO: remove redundant loop in future versions, deprecate old one
        
        foreach(GameObject door in doors)
        {
            dkb = door.GetComponent<DoorknobBehavior>();
            if(dkb==null)
                dkb = door.GetComponentInChildren<DoorknobBehavior>();
            if (dkb != null)
            {
                affectDoor(dkb,interactionMode);                                
            }
        }
    }
    private void affectDoor(DoorknobBehavior dkb,doorInteractionMode interactionMode)
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
            case doorInteractionMode.toggleLocked:
                dkb.ToggleThisLock("", null);
                break;
            case doorInteractionMode.toggleOpenClose:
                dkb.ToggleThisOpened("", null);
                break;
        }
    }
}

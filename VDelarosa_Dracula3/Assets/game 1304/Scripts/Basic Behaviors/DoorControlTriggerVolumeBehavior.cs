using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class DoorControlTriggerVolumeBehavior : MonoBehaviour
{
    public List<GameObject> doors;
    public bool onlyTriggerOnPlayer = true;
    int contentsCount = 0;
    List<GameObject> contents;
    [Header("On Enter Behavior")]
    public doorInteractionMode interactionModeOnEnter = doorInteractionMode.doNothing;
    [Header("On Exit Behavior")]
    public doorInteractionMode interactionModeOnExit = doorInteractionMode.doNothing;    
    [Header("On Empty Behavior")]
    [Tooltip("This will trigger when everything has left the volume. This can replace On Exit")]
    public doorInteractionMode interactionModeOnEmpty;

    private void Start()
    {
        contents = new List<GameObject>();
    }

    void OnTriggerEnter(Collider other)
    {
        DoorknobBehavior dkb;

        if ((other.gameObject.GetComponent<GAME1304PlayerController>() != null) || (!onlyTriggerOnPlayer))
        {
            contentsCount++;
            contents.Add(other.gameObject);
            if (interactionModeOnExit != doorInteractionMode.doNothing)
                InvokeRepeating("CheckContents", 0, 0.25f);
            foreach (GameObject door in doors)
            {
                dkb = door.GetComponent<DoorknobBehavior>();
                if (dkb == null)
                    dkb = door.GetComponentInChildren<DoorknobBehavior>();
                if (dkb != null)
                {
                    affectDoor(dkb, interactionModeOnEnter);
                }
            }
        }        
    }

    private void CheckContents()
    {
        for(int i = contents.Count-1;i>=0;i--)
        {
            if (contents[i] == null)
                contents.RemoveAt(i);
        }
        if (contents.Count == 0)
            TriggerEmptied();
    }
    private void OnTriggerExit(Collider other)
    {
        DoorknobBehavior dkb;
        if ((other.gameObject.GetComponent<GAME1304PlayerController>() != null) || (!onlyTriggerOnPlayer))
        {
            contentsCount--;
            contents.Remove(other.gameObject);
            
            if (contentsCount <= 0)
            {
                TriggerEmptied();
                
            }
            foreach (GameObject door in doors)
            {
                dkb = door.GetComponent<DoorknobBehavior>();
                if (dkb == null)
                    dkb = door.GetComponentInChildren<DoorknobBehavior>();
                if (dkb != null)
                {
                    affectDoor(dkb, interactionModeOnExit);
                }
            }
        }
    }

    private void TriggerEmptied()
    {
        CancelInvoke();
        DoorknobBehavior dkb;
        foreach (GameObject door in doors)
        {
            dkb = door.GetComponent<DoorknobBehavior>();
            if (dkb == null)
                dkb = door.GetComponentInChildren<DoorknobBehavior>();
            if (dkb != null)
            {
                affectDoor(dkb, interactionModeOnEmpty);
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
            case doorInteractionMode.doNothing:
                break;
        }
    }
}

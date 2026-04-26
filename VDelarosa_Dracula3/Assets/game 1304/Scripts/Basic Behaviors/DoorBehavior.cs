using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehavior : InteractiveObject
{
    public GameObject doorRoot;
    
    
    [Header("Event Listening")]    
    public string eventToLock;
    public string eventToUnlock;

    public string eventToOpen;
    public string eventToClose;

    [Header("Locking through script")]
    public bool isLockedArtificially = false;
    public bool canOpenWithScriptIfLocked = true;
    public string artificialLockedLabel = "Unlocked Elsewhere";

    void Start()
    {
        base.Start();
        if (eventToLock != "")
        {
            EventRegistry.AddEvent(eventToLock, lockThis, gameObject);
        }
        if (eventToUnlock != "")
        {
            EventRegistry.AddEvent(eventToUnlock, unlockThis, gameObject);
        }
        if (eventToOpen != "")
        {
            EventRegistry.AddEvent(eventToOpen, openThis, gameObject);
        }
        if (eventToClose != "")
        {
            EventRegistry.AddEvent(eventToClose, closeThis, gameObject);
        }
    }

    public override void interact()
    {
        base.interact();
          
        if(doorRoot!=null)
        {
            RotatingMoverBehavior rmb = doorRoot.GetComponent<RotatingMoverBehavior>();
            if(rmb!=null)
            {
                rmb.goToNext();
            }
        }
        //"unlock" the door
        if (InventoryItemNeeded != null)
            InventoryItemNeeded = null;
    }

    public void lockThis(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        isLockedArtificially = true;
    }

    public void unlockThis(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        isLockedArtificially = false;
    }

    public void openThis(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        if ((isLockedArtificially) && (!canOpenWithScriptIfLocked))
            return;
        if (doorRoot != null)
        {
            RotatingMoverBehavior rmb = doorRoot.GetComponent<RotatingMoverBehavior>();
            if (rmb != null)
            {
                rmb.goToB();
            }
        }
    }

    public void closeThis(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        if ((isLockedArtificially)&&(!canOpenWithScriptIfLocked))
             return;
        if (doorRoot != null)
        {
            RotatingMoverBehavior rmb = transform.root.GetComponentInChildren<RotatingMoverBehavior>();
            //RotatingMoverBehavior rmb = doorRoot.GetComponent<RotatingMoverBehavior>();
            if (rmb != null)
            {
                rmb.goToA();
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventListener_TakeInventoryItem : MonoBehaviour
{
    [Header("Event Listening")]
    public string eventToListenFor;

    public InventoryDef inventoryItem;
    public int numberToRemove = 1;

    // Use this for initialization
    void Start ()
    {
        EventRegistry.Init();
        if (eventToListenFor != "")
        {
            EventRegistry.AddEvent(eventToListenFor, takeItem, gameObject);
        }

    }

    public void takeItem(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != gameObject))
            return;
        ObjectInteractionBehavior oib;

        if (GameManager.player != null)
        {
            oib = GameManager.player.GetComponent<ObjectInteractionBehavior>();
            if (oib != null)
            {

                oib.inventory.removeItem(inventoryItem, numberToRemove);
            }

        }
    }
}

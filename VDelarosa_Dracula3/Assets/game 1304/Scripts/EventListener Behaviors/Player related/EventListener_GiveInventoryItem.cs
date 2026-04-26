using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventListener_GiveInventoryItem : MonoBehaviour
{
    [Header("Event Listening")]
    public string eventToListenFor;

    public InventoryDef inventoryItem;
    public int numberToGive = 1;

    // Use this for initialization
    void Start()
    {
        EventRegistry.Init();
        if (eventToListenFor != "")
        {
            EventRegistry.AddEvent(eventToListenFor, giveItem, gameObject);
        }
    }    

    public void giveItem(string eventName, GameObject obj)
    {        
        if ((obj != null) && (obj != this.gameObject))
            return;
        ObjectInteractionBehavior oib;

        if (GameManager.player != null)
        {
            oib = GameManager.player.GetComponent<ObjectInteractionBehavior>();
            if (oib != null)
            {

                for (int x = 0; x < numberToGive; x++)
                    oib.inventory.addItem(inventoryItem, numberToGive);
                    //oib.inventory.addItem(inventoryItemName,inventoryItemImage, numberToGive);
            }

        }
    }
}

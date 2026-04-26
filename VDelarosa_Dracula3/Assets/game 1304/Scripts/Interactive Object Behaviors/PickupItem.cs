using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickupItem : MonoBehaviour 
{

	//public string inventoryItemName;
    public int itemAmount = 1;
	//public Sprite inventoryItemImage;
    public InventoryDef inventoryItemDefinition;
    [SerializeField]
    public List<EventPackage> EventsSentOnPickup;

    [Tooltip("The item will be destroyed on pickup and won't go into your inventory.")]
	public bool consumeOnPickup = false;
    [Tooltip("Item will be picked up when you run into it rather than having to interact with it.")]
    public bool pickUpOnCollision = false;

    public static bool IsKey(PickupItem item)
    {
        return ((Key_PickupItem)item != null);
    }
}

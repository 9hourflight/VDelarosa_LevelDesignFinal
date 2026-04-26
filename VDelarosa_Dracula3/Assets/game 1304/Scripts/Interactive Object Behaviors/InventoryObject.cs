using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public enum EquipSlot { unequippable, head, torso, hand, legs, foot, back};
public enum InventoryInteractionContext { InWorld,InHand,InInventory};
public enum InventoryOperation { removeAll, multiply, add, subtract, divide };

[System.Serializable]
public class inventoryInteraction
{
    [SerializeField]
    public string itemName;
    
    public List<EventPackage> eventsToSend;
}



[System.Serializable]
public class InventoryManipulationPackage
{
    public string eventToListenFor;
    public InventoryOperation inventoryOperation;
    public int operationValue;
}

public class InventoryObject : MonoBehaviour
{    
    public string itemName;
    public int maxStackSize = 1;
    // Start is called before the first frame update
 
    public EquipSlot equipSlot = EquipSlot.unequippable;
 
    Vector2 size = new Vector2(1, 1);
 
    GameObject meshInHand;
 
    GameObject meshInWorld;
 
    public Sprite inventorySprite;
 
    public List<inventoryInteraction> inventoryInteractions;

    [Header("Event Listening")]
    public List<InventoryManipulationPackage> inventoryOperationEvents;

    [HideInInspector]
    public InventoryInteractionContext currentInteractionContext;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

//public enum comparisonOperator { lessThan,lessThanEqual,Equal,greaterThanEqual,greaterThan,notEqual};


[System.Serializable]
public class EventRelay_InventoryTest : MonoBehaviour
{
    public InventoryDef inventoryDef;
    public comparisonOperator comparisonOp;
    public int count;

    
    public List<string> eventsToListenFor;
    public List<EventPackage> eventsTosendIfTestSucceeds;
    public List<EventPackage> eventsToSendIfTestFails;



    void Start()
    {
        if (eventsToListenFor.Count > 0)
        {
            foreach (string s in eventsToListenFor)
            {
                if (s != "")
                    EventRegistry.AddEvent(s, testInventory, gameObject);
            }
        }
    }

    void testInventory(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != gameObject))
            return;
        switch (comparisonOp)
        {
            case comparisonOperator.Equal:
                executeEvents(GameManager.GetPlayerInventory().GetItemCount(inventoryDef) == count);                    
                break;
            case comparisonOperator.greaterThan:
                executeEvents(GameManager.GetPlayerInventory().GetItemCount(inventoryDef) > count);                    
                break;
            case comparisonOperator.greaterThanEqual:
                executeEvents(GameManager.GetPlayerInventory().GetItemCount(inventoryDef) >= count);                    
                break;
            case comparisonOperator.lessThan:
                executeEvents(GameManager.GetPlayerInventory().GetItemCount(inventoryDef) < count);                  
                break;
            case comparisonOperator.lessThanEqual:
                executeEvents(GameManager.GetPlayerInventory().GetItemCount(inventoryDef) <= count);                    
                break;
            case comparisonOperator.notEqual:
                executeEvents(GameManager.GetPlayerInventory().GetItemCount(inventoryDef) != count);                   
                break;
        }
        
    }

    void executeEvents(bool success)
    {
        if (success)
        {                       
            foreach (EventPackage ep in eventsTosendIfTestSucceeds)
                EventRegistry.SendEvent(ep, this.gameObject);
        }
        else
        {            
            foreach (EventPackage ep in eventsToSendIfTestFails)
                EventRegistry.SendEvent(ep, this.gameObject);
        }

    }


}

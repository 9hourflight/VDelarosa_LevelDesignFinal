using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class inventoryEntry
{
    public InventoryDef inventoryDefinition;
    public Sprite entrySprite;
    public string entryName;
    private string baseEntryName;
    public int count = 1;


    /*public void updateName()
    {
        //TODO: check for null text component
        if (count > 1)
            UIName.GetComponent<Text>().text = count.ToString() + " X " + entryName;
        else
            UIName.GetComponent<Text>().text = entryName;
    }*/
}

[System.Serializable]
public class Inventory
{
    private List<inventoryEntry> _inventoryEntries;
    public Inventory()
    {
        _inventoryEntries = new List<inventoryEntry>();
    }


    public int GetItemCount(InventoryDef iDef)
    {
        foreach (inventoryEntry iE in _inventoryEntries)
        {
            if (iE.inventoryDefinition == iDef)
            {
                return iE.count;
            }
        }
        return 0;
    }
    public int GetItemCount(string itemName)
    {
        foreach (inventoryEntry iE in _inventoryEntries)
        {
            if (iE.entryName == itemName)
            {
                return iE.count;
            }
        }
        return 0;
    }

    public bool hasItem(string itemName)
    {
        inventoryEntry tempIE;
        return hasItem(itemName, out tempIE);
    }

    public bool hasItem(string itemName, out inventoryEntry entry)
    {
        foreach (inventoryEntry iE in _inventoryEntries)
        {
            if (iE.entryName == itemName)
            {
                entry = iE;
                return true;
            }
        }
        entry = null;
        return false;
    }

    public bool hasItem(InventoryDef iDef, int count = 1)
    {
        //TODO: remove redundant code here with the other overloaded versions of this method
        if (GetItemCount(iDef) >= count)
            return true;
        else
            return false;
    }

    public bool hasItem(InventoryDef iDef, out inventoryEntry entry)
    {
        foreach (inventoryEntry iE in _inventoryEntries)
        {
            if (iE.inventoryDefinition == iDef)
            {
                entry = iE;
                return true;
            }
        }
        entry = null;
        return false;
    }

    public bool hasItem(string itemName, int count = 1)
    {
        //TODO: remove redundant code here with the other overloaded versions of this method
        if (GetItemCount(itemName) >= count)
            return true;
        else
            return false;
    }
    public void addItem(inventoryEntry ie)
    {
        _inventoryEntries.Add(ie);
    }

    public void removeItem(InventoryDef iDef, int count = 1)
    {
        inventoryEntry iEToRemove = null;
        foreach (inventoryEntry iE in _inventoryEntries)
        {
            if (iE.inventoryDefinition == iDef)
            {
                iE.count -= count;
                if (iE.count <= 0)
                {
                    iEToRemove = iE;
                    iE.count = 0;
                }
                else
                {
                    // iE.updateName();
                }
            }
        }
        if(iEToRemove != null)
            _inventoryEntries.Remove(iEToRemove);
    }
    public void removeItem(string itemName, int count = 1)
    {
        inventoryEntry iEToRemove = null;
        foreach (inventoryEntry iE in _inventoryEntries)
        {
            if (iE.entryName == itemName)
            {
                iE.count -= count;
                if (iE.count <= 0)
                {
                    // GameObject.Destroy(iE.UIName);
                    // GameObject.Destroy(iE.UISprite);
                    iEToRemove = iE;
                    iE.count = 0;
                }
                else
                {
                   // iE.updateName();
                }
            }
        }
        if (iEToRemove != null)
            _inventoryEntries.Remove(iEToRemove);
    }

    public void addItem(InventoryDef inventoryDefinition, int count=1)
    {
        inventoryEntry tempIE;

        if (hasItem(inventoryDefinition, out tempIE))
        {
            tempIE.count += 1;
            //tempIE.updateName();
        }
        else
        {
            tempIE = new inventoryEntry();
            tempIE.inventoryDefinition = inventoryDefinition;
            tempIE.count = count;
            _inventoryEntries.Add(tempIE);
        }
    }
    public void addItem(string itemName, Sprite itemSprite, int count = 1)
    {        
        inventoryEntry tempIE;
        if (hasItem(itemName, out tempIE))
        {
            tempIE.count += 1;
            //tempIE.updateName();
        }
        else
        {
            tempIE = new inventoryEntry();
            tempIE.entryName = itemName;
            tempIE.entrySprite = itemSprite;
            tempIE.count = count;
            _inventoryEntries.Add(tempIE);
        }        
    }    

    public List<inventoryEntry> getEntries()
    {
        return _inventoryEntries;
    }
  
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Unnamed Inventory Item", menuName = "GAME 1304/Inventory Object", order = 1)]
public class InventoryDef : ScriptableObject
{
    public GameObject objectPrefab;
    public string itemName;
    public string itemDescription;
    public Sprite inventoryImage;
    public int gridw, gridh;
    public float weight;
    public bool droppable = true;
}
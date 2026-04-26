using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryTabManager : UtilityTabBehavior
{
    [Space(3)]
    [Header("Inventory UI Stuff")]
    public GameObject InventoryTextUIPrefab;
    public GameObject InventoryImageUIPrefab;    
    private List<GameObject> inventoryChildren;
    public Canvas inventoryGridCanvas;
    public GameObject cursor;
    private Image cursorImage;
    public Text itemNameText;
    public Text itemDescriptionText;
    public Text keyRingText;    

    /*public  Text objectiveDescriptionText;
    public Text objectiveTitleText;
    public Text objectiveTasksText;
    public static string objectiveTitle;
    public static string objectiveDescription;

    public static List<objectiveTaskEntry> taskEntries;*/
    public static InventoryTabManager instance;
    private Inventory inventory;
    private int cursorIndex = 0;
    private ObjectInteractionBehavior playerOIB;

    private static bool isInitialized = false;
    void Awake()
    {        
        instance = this;
        cursorImage = cursor.GetComponent<Image>();
        if (GameManager.player != null)
        {
            if (GameManager.player.TryGetComponent<ObjectInteractionBehavior>(out playerOIB))
            {
                inventory = playerOIB.inventory;
            }
        }        
    }

    public override void display()
    {
        //TODO: update the cursor position and what the cursor is pointing to
        //we could end up here after the player removes something from their inventory and things get shuffled down
        //ultimately there should be allowance for "gaps" in the inventory when something is dropped, and then items can be added into those gaps when picked up

        if (inventory == null)
        {                        
            if (GameManager.player.TryGetComponent<ObjectInteractionBehavior>(out playerOIB))
            {
                inventory = playerOIB.inventory;
            }            
        }
        if (inventory == null)
        {
            cursorImage.enabled = false;
            return;
        }
            
        
        if (inventoryChildren != null)
            foreach (GameObject go in inventoryChildren)
                Destroy(go);
        inventoryChildren = new List<GameObject>();
        /*int index = 0;
        int xpos, ypos;*/
        foreach (inventoryEntry iE in inventory.getEntries())
        {
            /*xpos = index % inventoryGridWidth;
            ypos = index / inventoryGridWidth;*/
            
            GameObject UISprite = Instantiate(InventoryImageUIPrefab, inventoryGridCanvas.transform);
            GameObject UIName = Instantiate(InventoryTextUIPrefab, UISprite.transform);            
            inventoryChildren.Add(UISprite);
            inventoryChildren.Add(UIName);
            if (iE.count > 1)
                UIName.GetComponent<Text>().text = " X "+iE.count.ToString();
            else
                UIName.GetComponent<Text>().text = "";
            if (iE.inventoryDefinition != null)
            {
                UISprite.GetComponent<Image>().sprite = iE.inventoryDefinition.inventoryImage;
            }
            else
            {
                UISprite.GetComponent<Image>().sprite = iE.entrySprite;
            }

            //TODO: make the position not based on hard numbers
            /*UIName.transform.position = new Vector3(inventoryGridCanvas.pixelRect.xMin + (xpos * 100), inventoryGridCanvas.pixelRect.yMax - (ypos * 100), UIName.transform.position.z);
            if(UISprite.GetComponent<Image>() != null)
                UISprite.GetComponent<Image>().transform.position = new Vector3(inventoryGridCanvas.pixelRect.xMin+ (xpos*100), inventoryGridCanvas.pixelRect.yMax - (ypos*100) - 50, UISprite.transform.position.z);*/
          //  index++;
        }
        if (inventory.getEntries().Count == 0)
        {
            cursorImage.enabled = false;
            itemDescriptionText.text = "";
            itemNameText.text = "";
        }
        else
            cursorImage.enabled = true;

        //TODO: figure out why this doesn't always map to the number of items in the inventory list
        if (inventoryGridCanvas.transform.childCount > 0)
        {
            cursor.transform.position = inventoryGridCanvas.transform.GetChild(cursorIndex).transform.position; // inventoryChildren
            if (cursorIndex > inventory.getEntries().Count - 1)
                cursorIndex = inventory.getEntries().Count - 1;
            if (cursorIndex < 0)
                cursorIndex = 0;

            if (inventory.getEntries()[cursorIndex].inventoryDefinition != null)
            {
                itemNameText.text = inventory.getEntries()[cursorIndex].inventoryDefinition.name;
                itemDescriptionText.text = inventory.getEntries()[cursorIndex].inventoryDefinition.itemDescription;
            }
            else
            {
                itemNameText.text = inventory.getEntries()[cursorIndex].entryName;
            }
            
        }

        string tempString = "";
        if (playerOIB.keyRing != null)
        {
            foreach(string s in playerOIB.keyRing.Keys)
            {
                int keycount = playerOIB.keyRing[s];
                if (keycount > 0)
                {
                    tempString += s;
                    if (keycount > 1)
                    {
                        tempString += "x" + keycount;
                    }
                    tempString += "\n";
                }
            }
            
        }
        keyRingText.text = tempString;

        
    }

    public static void init()
    {
        if (isInitialized)
            return;
        isInitialized = true;        
        
    }

    // Update is called once per frame
    void Update()
    {
        int tempInt;
        bool cursorMoved = false;
        if(Input.GetKeyDown(KeyCode.W))
        {
            tempInt = cursorIndex - 10;
            if (tempInt >= 0)
            {
                cursorIndex = tempInt;
                cursorMoved = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            tempInt = cursorIndex + 10;
            if (tempInt < inventoryGridCanvas.transform.childCount)
            {
                cursorIndex = tempInt;
                cursorMoved = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            tempInt = cursorIndex - 1;
            if (tempInt >= 0)
            {
                if((tempInt / 10) == (cursorIndex/10))
                {
                    cursorIndex = tempInt;
                    cursorMoved = true;
                }
                
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            tempInt = cursorIndex + 1;
            if (tempInt < inventoryGridCanvas.transform.childCount)
            {
                if ((tempInt / 10) == (cursorIndex / 10))
                {
                    cursorIndex = tempInt;
                    cursorMoved = true;
                }
                
            }

        }
        if(cursorMoved)
        {
            cursor.transform.position = inventoryGridCanvas.transform.GetChild(cursorIndex).transform.position;
            if (inventory.getEntries()[cursorIndex].inventoryDefinition != null)
            {
                itemNameText.text = inventory.getEntries()[cursorIndex].inventoryDefinition.name;
                itemDescriptionText.text = inventory.getEntries()[cursorIndex].inventoryDefinition.itemDescription;
            }
            else
            {
                itemNameText.text = inventory.getEntries()[cursorIndex].entryName;
            }
        }
        //Drop the item
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (playerOIB != null)
            {
                playerOIB.DropItemFromInventory(inventory.getEntries()[cursorIndex].inventoryDefinition);
            }
            inventory.removeItem(inventory.getEntries()[cursorIndex].inventoryDefinition);
            
            //cursor.transform.position = inventoryGridCanvas.transform.GetChild(cursorIndex).transform.position;
            //itemNameText.text = inventory.getEntries()[cursorIndex].entryName;            
            display();
        }
    }

   
}

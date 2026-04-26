using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using System;

public enum DispensedBoxColor { red, blue, green, yellow, white, black, purple, orange};

[Serializable]
public class materialColorPair
{
    public Material matRef;
    public DispensedBoxColor boxColor;
}
[Serializable]
public class InteractiveObjectBoxDispenser : InteractiveObject
{
    public GameObject boxPrefab;
    public Transform spawnLocation;
    public DispensedBoxColor boxColor;    
    public List<materialColorPair> materialMapping;

    string interactEvent;

    public override void Start()
    {
        base.Start();
        interactEvent = "DestroyAllDispensedBoxes_";
        switch (boxColor)
        {
            case DispensedBoxColor.black:
                interactEvent += "Black";
                break;
            case DispensedBoxColor.red:
                interactEvent += "Red";
                break;
            case DispensedBoxColor.blue:
                interactEvent += "Blue";
                break;
            case DispensedBoxColor.green:
                interactEvent += "Green";
                break;
            case DispensedBoxColor.yellow:
                interactEvent += "Yellow";
                break;
            case DispensedBoxColor.white:
                interactEvent += "White";
                break;
            case DispensedBoxColor.purple:
                interactEvent += "Purple";
                break;
            case DispensedBoxColor.orange:
                interactEvent += "Orange";
                break;
        }
    }

    public override void interact()
    {
        GameObject boxObject;
        EventListener_DestroyObject doBehavior;
        Renderer r;
        base.interact();
        EventRegistry.SendEvent(interactEvent); 
        boxObject = Instantiate(boxPrefab, spawnLocation.position,spawnLocation.rotation);
        r = boxObject.GetComponent<Renderer>();
        if (r != null)
        {
            foreach (materialColorPair mcp in materialMapping)
            {
                if(mcp.boxColor == boxColor)
                {
                    r.material = mcp.matRef;
                }
            }
        }
        doBehavior = boxObject.GetComponent<EventListener_DestroyObject>();
        if(doBehavior!=null)
        {
            doBehavior.eventsToTriggerThis.Add(interactEvent);
        }
        
    }




}

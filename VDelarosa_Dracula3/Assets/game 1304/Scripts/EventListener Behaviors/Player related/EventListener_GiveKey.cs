using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventListener_GiveKey: MonoBehaviour
{
    [Header("Event Listening")]
    public string eventToListenFor;

    public string keyName;
    public int numberToGive = 1;

    ObjectInteractionBehavior playerOIB;
    // Use this for initialization
    void Start()
    {
        EventRegistry.Init();
        if (eventToListenFor != "")
        {
            EventRegistry.AddEvent(eventToListenFor, giveKey, gameObject);
        }
        playerOIB = GameManager.player.GetComponent<ObjectInteractionBehavior>();
    }    

    public void giveKey(string eventName, GameObject obj)
    {        
        if ((obj != null) && (obj != this.gameObject))
            return;
        

        if (GameManager.player != null)
        {            
            if (playerOIB != null)
            {
                if (playerOIB.keyRing.ContainsKey(keyName))
                {
                    playerOIB.keyRing[keyName] += numberToGive;
                }
                else
                {
                    playerOIB.keyRing.Add(keyName, numberToGive);
                }
            }

        }
    }
}

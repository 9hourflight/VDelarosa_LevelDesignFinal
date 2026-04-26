using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventListener_GiveWeapon : MonoBehaviour
{
    [Header("Event Listening")]
    public string eventToListenFor;

    public GameObject weaponPrefab;

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
            (GameManager.player.GetComponent<GAME1304PlayerController>()).giveWeapon(weaponPrefab);
        }
    }
}
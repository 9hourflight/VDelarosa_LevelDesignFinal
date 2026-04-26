using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventListener_AlterPlayerStats: MonoBehaviour 
{
    [Header("Events")]
    public List<playerStatChange> playerStatChangeEvents;
    private GAME1304PlayerController playerController;

    void Start()
    {
        Invoke("setPlayer", 0.25f);
        foreach (playerStatChange psc in playerStatChangeEvents)
        {
            EventRegistry.AddEvent(psc.eventToListenFor, alterStatsOnEvent, gameObject);
        }
    }

    void alterStatsOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        foreach (playerStatChange psc in playerStatChangeEvents)
        {
            if (eventName == psc.eventToListenFor)
            {
                playerController.changePlayerStat(psc);
            }
        }
    }

    void setPlayer()
    {
        playerController = GameManager.player.GetComponent<GAME1304PlayerController>();
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NPCSpawnProfile
{    
    public GameObject NPCPrefabToSpawn;
    public NavPointContainer patrolContainer;
    public PatrolSwitchBehaviorType psb = PatrolSwitchBehaviorType.pickFirst;
    public Transform spawnLocation;
}

public enum SpawnMethod { sequential, random};
public class NPCSpawner : MonoBehaviour
{
    [Header("Spawn Info")]
    
    public List<NPCSpawnProfile> spawnProfiles;            

    [Header("Event Listening")]
    public string eventToSpawn;
    [Tooltip("Remove all NPCs from this spawner before spawning new ones?")]
    public bool wipeOnSpawn = false;

    [Header("Event sending")]
    public List<EventPackage> EventsToFireOnNPCKilled;
    public List<EventPackage> EventsToFireOnAllNPCsKilled;

    List<NPCBehavior> NPCs;
    void Start()
    {
        if (eventToSpawn != "")
        {
            EventRegistry.AddEvent(eventToSpawn, SpawnOnEvent, gameObject);
        }
        NPCs = new List<NPCBehavior>();
    }

    void SpawnOnEvent(string eventName, GameObject obj)
    {
        NPCBehavior tempNPCB;
        GameObject tempNPC;
        if(wipeOnSpawn)
        {
            foreach(NPCBehavior npcb in NPCs)
            {
                Destroy(npcb.gameObject);
            }
            NPCs = new List<NPCBehavior>();
        }

        foreach(NPCSpawnProfile sp in spawnProfiles)
        {
            tempNPC = Instantiate(sp.NPCPrefabToSpawn,sp.spawnLocation.position,sp.spawnLocation.rotation);
            if(tempNPC.TryGetComponent<NPCBehavior>(out tempNPCB))
            {
                NPCs.Add(tempNPCB);
                tempNPCB.OnDeath.AddListener(NPCKilled);
                if(sp.patrolContainer!=null)
                {
                    tempNPCB.setPatrol(sp.patrolContainer,sp.psb);
                    //tempNPCB.PatrolContainer = sp.patrolContainer;
                    tempNPCB.setBehavior(BehaviorType.patrolling);
                }
            }
        }
    }

    void NPCKilled()
    {
        foreach(EventPackage ep in EventsToFireOnNPCKilled)
        {
            EventRegistry.SendEvent(ep,gameObject);
        }
        bool anyAlive = false;

        foreach(NPCBehavior npcb in NPCs)
        {
            if (npcb != null)
            {
                if (npcb.isAlive)
                    anyAlive = true;
            }
        }
        if(!anyAlive)
        {
            foreach (EventPackage ep in EventsToFireOnAllNPCsKilled)
            {
                EventRegistry.SendEvent(ep, gameObject);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public static class NPCManager
{
	public static List<NPCBehavior> AIList;
    public static List<GameObject> objectsOfInterest;
    private static bool isInitialized = false;
    public static List<FactionDef> factionDefinitions;
    public static List<GameObject> knownCorpses;
    public static Dictionary<Faction, Dictionary<Faction, Attitude>> factionRelationships;
    static bool debugVisualState = false;

    public static void init()
    {
        
        FactionDef tempFD;
        if (isInitialized)
            return;
        isInitialized = true;
        AIList = new List<NPCBehavior>();
        objectsOfInterest = new List<GameObject>();

        factionRelationships = new Dictionary<Faction, Dictionary<Faction, Attitude>>();
        //TODO: Ugh, hard coding. It will have to do for a class setting where I don't want people to have to remember an extra step of loading a game manager object and set their faction database though
        FactionDatabase factionDatabase = new FactionDatabase(); // (FactionDatabase)AssetDatabase.LoadAssetAtPath("Assets/game 1304/Scriptable Objects/FactionDefs.asset", typeof(FactionDatabase));
        
        factionDefinitions = new List<FactionDef>();
        if (factionDatabase.factionDefinitions != null)
        {
            foreach (FactionDef fd in factionDatabase.factionDefinitions)
            {
                factionDefinitions.Add(fd);
                factionRelationships.Add(fd.faction, new Dictionary<Faction, Attitude>());
                foreach (factionRelationship fr in fd.factionRelationships)
                {
                    if (factionRelationships.ContainsKey(fd.faction))
                        factionRelationships[fd.faction].Add(fr.targetFaction, fr.attitude);
                }
            }
        }
        knownCorpses = new List<GameObject>();
    }

	public static void Reset()
    {
        isInitialized = false;
    }
	
    public static void updatePlayerRef()
    {
        init();
        foreach (NPCBehavior eb in AIList)
            eb.updatePlayerRef();
    }

    public static void registerOOI(GameObject ooi)
    {
        init();
        if (objectsOfInterest != null)
            objectsOfInterest.Add(ooi);
    }

    public static void registerAI(NPCBehavior enemy)
    {
        init();
        if (!AIList.Contains(enemy))
        {
            AIList.Add(enemy);
            enemy.SetDebugVisuals(debugVisualState);
        }
    }

    public static void wipeAllAISuspicion()
    {
        init();
        foreach (NPCBehavior enemy in AIList)
        {
            enemy.wipeSuspicion();
        }
    }

    internal static void ToggleDebugVisuals()
    {
        debugVisualState = !debugVisualState;
        foreach(NPCBehavior enemy in AIList)
            enemy.SetDebugVisuals(debugVisualState);
    }
}

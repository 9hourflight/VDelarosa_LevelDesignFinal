using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "FactionDef", menuName = "GAME 1304/NPC Faction Definitions", order = 1)]
public class FactionDatabase : ScriptableObject
{    
    public List<FactionDef> factionDefinitions;
}

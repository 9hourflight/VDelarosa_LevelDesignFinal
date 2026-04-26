using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[System.Serializable]
public enum Attitude { friendly, neutral, hostile, fearful};

public enum Faction { player, enemies, friendlies, bystanders, FactionA, FactionB, FactionC, FactionD, FactionE, FactionF }

[System.Serializable]
public class factionRelationship
{
    public Faction targetFaction;
    public Attitude attitude;
}

[System.Serializable]
public class FactionDef
{
    public Faction faction;
    public string factionName = "";
    public List<factionRelationship> factionRelationships;    
}

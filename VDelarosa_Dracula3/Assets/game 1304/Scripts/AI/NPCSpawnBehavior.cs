using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawnBehavior : MonoBehaviour 
{
	public string spawnEvent;
	public List<string> EventsToFireOnSpawn;
	public bool spawnOnStart = true;
	NPCBehavior npcBehavior;

	// Use this for initialization
	void Start () 
	{
		TryGetComponent<NPCBehavior>(out npcBehavior);
		if (spawnOnStart == false)
		{
			//invoking this so the NPC can register event listeners and such before those behaviors are disabled
			Invoke("DeactivateNPC", 0);

		}
		else
			Invoke("SpawnEvents", 0);
	}
	
	void DeactivateNPC()
	{
		Collider[] colliders = GetComponentsInChildren<Collider>();
		if (colliders.Length > 0)
		{
			foreach (Collider c in colliders)
			{
				c.enabled = false;
			}
		}
		Renderer[] renderers = GetComponentsInChildren<Renderer>();
		if (renderers.Length > 0)
		{
			foreach (Renderer r in renderers)
			{
				r.enabled = false;
			}
		}
		if (TryGetComponent<Collider>(out Collider col))
			col.enabled = false;
		if (npcBehavior != null)
		{
			npcBehavior.setBehavior(BehaviorType.idle);
			npcBehavior.enabled = false;
		}
		if (TryGetComponent<Renderer>(out Renderer ren))
			ren.enabled = false;

		if (spawnEvent != "")
			EventRegistry.AddEvent(spawnEvent, Spawn, gameObject);
	}
	// Update is called once per frame
	void Update () 
	{
		
	}
	void SpawnEvents()
    {
		foreach (string s in EventsToFireOnSpawn)
		{
			if (s != "")
				EventRegistry.SendEvent(s);
		}
	}
	void Spawn(string eventname, GameObject obj)
	{
        if ((obj != null) && (obj != this.gameObject))
            return;
        HelperFunctions.showObjectAndChildren(this.gameObject);
		HelperFunctions.enableCollisionObjectAndChildren(this.gameObject);

		//gameObject.GetComponent<NPCBehavior>().enabled = true;
		npcBehavior.enabled = true;
		npcBehavior.setBehavior(npcBehavior.startingBehavior);
		SpawnEvents();
	}
}

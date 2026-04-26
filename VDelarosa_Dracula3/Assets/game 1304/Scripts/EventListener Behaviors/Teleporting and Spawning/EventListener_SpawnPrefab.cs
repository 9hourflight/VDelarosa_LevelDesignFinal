using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventListener_SpawnPrefab : MonoBehaviour 
{

	public string eventToListenFor;
	public GameObject prefabToSpawn;
	public GameObject objectToSpawnAt;
	public Vector3 locationToSpawn;
	public Vector3 rotationToSpawn;

	void Start () 
	{
		EventRegistry.AddEvent(eventToListenFor, spawnObject, gameObject);
	}
	
	void spawnObject(string eventName, GameObject obj)
	{
        if ((obj != null) && (obj != gameObject))
            return;
        if (objectToSpawnAt != null)
			Instantiate(prefabToSpawn, objectToSpawnAt.transform.position,objectToSpawnAt.transform.rotation);	
		else
			Instantiate(prefabToSpawn, locationToSpawn, Quaternion.Euler(rotationToSpawn));	
	}

	void Update () 
	{
		
	}
}

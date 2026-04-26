using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventListener_DistributeObjects : MonoBehaviour 
{

	public string eventToListenFor;
	public List<GameObject> objectsToDistribute;
	public List<GameObject> destinationMarkerLocations;    

	void Start () 
	{
		if(eventToListenFor != "")
		{
			EventRegistry.AddEvent(eventToListenFor, distributeObjects, gameObject);
		}
	}
	
	void distributeObjects(string eventName, GameObject obj)
	{
        if ((obj != null) && (obj != this.gameObject))
            return;
        int i;
		shuffleDestinationList();
		for (i=0;i<objectsToDistribute.Count;i++) 
		{
			//if(i>destinationMarkerLocations.Count
			GameObject o = objectsToDistribute[i];
			//int destIndex = 
			o.transform.position = getDestination(i).transform.position;
			o.transform.rotation = getDestination(i).transform.rotation;
		}
	}

	GameObject getDestination(int index)
	{
		if(index < destinationMarkerLocations.Count)
			return destinationMarkerLocations[index];
		else
			return destinationMarkerLocations[index % destinationMarkerLocations.Count];
	}

	void shuffleDestinationList()
	{
		GameObject temp;
		int randomIndex;
		for(int i = 0; i < destinationMarkerLocations.Count; i++)
		{
			temp = destinationMarkerLocations[i];
			randomIndex = Random.Range(i, destinationMarkerLocations.Count);
			destinationMarkerLocations[i] = destinationMarkerLocations[randomIndex];
			destinationMarkerLocations[randomIndex] = temp;
		}
	}



}

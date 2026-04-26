using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NPCBehavior))]
public class EventListener_MoveToPoint : MonoBehaviour 
{
	public string eventToListenFor;
	public List<string> eventsToSendOnCompleting;
	public GameObject destination;
	public bool rotateToDestinationFacing = false;
	//public bool destinationIsMoving = false;
	//TODO: Implement the above
    private NPCBehavior eb;
	private bool isMoving = false;

	void Start () 
	{
        eb = GetComponent<NPCBehavior>();		
		if(eventToListenFor != "")					
			EventRegistry.AddEvent(eventToListenFor,moveToPoint, gameObject);
		
	}
	
	void moveToPoint(string eventName, GameObject obj)
	{
        if ((obj != null) && (obj != gameObject))
            return;
        if (eb != null)
		{
			eb.setAgentDestination(destination.transform.position);			
			isMoving = true;
		}
	}

	void Update () 
	{
		if (!eb.getAgentPathPending() && eb.getAgentRemainingDistance() < 0.5f && isMoving)
		{
			isMoving = false;
			if (rotateToDestinationFacing)
			{
				
				Invoke("updateRotation", 0.5f);
			}

			if(eventsToSendOnCompleting.Count > 0)
			{
				foreach(string s in eventsToSendOnCompleting)
					EventRegistry.SendEvent(s);
			}
		}
	}
	void updateRotation()
    {
		eb.transform.rotation = destination.transform.rotation;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.AI;

public class TeleporterVolume : MonoBehaviour 
{
	//TODO: make this inherit from trigger volume and get rid of a lot of the cruft
	[Header("Teleportation Parameters")]
	public TeleporterVolume destinationTeleporter;
	public bool oneWay = false;
	//public Transform inboundTeleportPosition;
	public ParticleSystem teleporterFX;
	public Light teleporterLight;

    [Header("Event Sending")]
    public List<EventPackage> EventsToSendOnEnter;
    public List<EventPackage> EventsToSendOnExit;

    [Header("Event Listening")]
    public string eventToEnableThis;
	public string eventToDisableThis;
	public bool startEnabled = true;
	//public bool PlayerOnly = true;
	private bool isEnabled;
    public bool onlyTriggerOnPlayer = false;
    public List<GameObject> onlyTriggerOnTheseObjects;
	public List<GameObject> ignoreTheseObjects;
	
	

    void Start () 
	{
		
		setEnabled(startEnabled);
		if(eventToEnableThis != "")
			EventRegistry.AddEvent(eventToEnableThis, enableThisOnEvent, gameObject);
		if(eventToDisableThis != "")
			EventRegistry.AddEvent(eventToDisableThis, disableThisOnEvent, gameObject);
		if(oneWay)
        {
			if (destinationTeleporter != null)
				destinationTeleporter.disableThis();
        }
	}

	void Update () 
	{
		
	}

    public void setEnabled(bool newEnabled)
    {
		isEnabled = newEnabled;
		if(isEnabled)
			enableThis();
		else
			disableThis();
		
	}
	public void enableThis()
    {
        isEnabled = true;
		if (teleporterFX != null)
			teleporterFX.Play();
		if (teleporterLight != null)
			teleporterLight.enabled = true;

	}
	public void enableThisOnEvent(string eventName, GameObject obj)
	{
        if ((obj != null) && (obj != this.gameObject))
            return;
        enableThis();
	}

    private void disableThis()
    {
		isEnabled = false;
		if (teleporterFX != null)
			teleporterFX.Stop();
		if (teleporterLight != null)
			teleporterLight.enabled = false;
	}
	public void disableThisOnEvent(string eventName, GameObject obj)
	{
        if ((obj != null) && (obj != this.gameObject))
            return;
		disableThis();
	}

	private bool objectChecksOut(GameObject go)
	{
		if(ignoreTheseObjects.Count>0)
        {
			foreach(GameObject g in ignoreTheseObjects)
            {
				if (go == g)
					return false;
            }
        }
        if(onlyTriggerOnPlayer)
        {
            if (go == GameManager.player)
                return true;
            else
                return false;
        }
		if(onlyTriggerOnTheseObjects.Count > 0)
		{
			foreach(GameObject g in onlyTriggerOnTheseObjects)
			{
				if(go == g)
					return true;
			}
			return false;
		}
		else
			return true;
	}

	void OnTriggerEnter(Collider other) 
	{
		if(isEnabled)
		{
			if(objectChecksOut(other.gameObject)&&(!other.isTrigger))
			{
				
				if(EventsToSendOnEnter.Count > 0)
				{
                    foreach (EventPackage ep in EventsToSendOnEnter)
                    {
                        if ((ep.scope == eventScope.instigator) || (ep.scope == eventScope.visualScriptingInstigator))
                            EventRegistry.SendEvent(ep, other.gameObject);
                        else
                            EventRegistry.SendEvent(ep, this.gameObject);
                    }                    
                }
				Teleport(other.gameObject);
			}
		}
	}

	public void ProcessInteraction(teleporterInteractionModes mode)
    {
		switch(mode)
        {
			case teleporterInteractionModes.doNothing:
				break;
			case teleporterInteractionModes.toggleOnOff:
				if(isEnabled)
                {
					disableThis();
					if (destinationTeleporter != null)
						destinationTeleporter.disableThis();
				}
				else
                {
					enableThis();
					if ((destinationTeleporter != null) && (!oneWay))
						destinationTeleporter.enableThis();
				}
				break;
			case teleporterInteractionModes.turnOff:
				disableThis();
				if (destinationTeleporter != null)
					destinationTeleporter.disableThis();
				break;
			case teleporterInteractionModes.turnOn:
				enableThis();
				if ((destinationTeleporter != null)&&(!oneWay))
					destinationTeleporter.enableThis();
				break;
        }
    }
	void Teleport(GameObject teleTarget)
    {
		if (destinationTeleporter == null)
			return;
		GAME1304PlayerController pc;
		Vector3 offset = transform.position - teleTarget.transform.position;
		Debug.Log("offset: " + offset);
		Transform destination = destinationTeleporter.transform;
		//destination.position -= offset;
		NavMeshAgent targetAgent;

		destinationTeleporter.setEnabled(false);
		if(teleTarget.TryGetComponent<GAME1304PlayerController>(out pc))
        {
			pc.teleport(destination);
        }
		else
        {
			targetAgent = teleTarget.GetComponent<NavMeshAgent>();
			if (targetAgent != null)
			{
				NavMeshHit hit;
				NavMesh.SamplePosition(destination.position + (Vector3.up * 2), out hit, 5f, NavMesh.AllAreas);
				if (Vector3.Distance(destination.position, hit.position) < 10)
					destination.position = hit.position;
				if (targetAgent.isOnOffMeshLink)
				{
					targetAgent.CompleteOffMeshLink();
				}
				targetAgent.Warp(destination.position);
			}
			else
			{
				teleTarget.transform.position = destination.position;
				teleTarget.transform.rotation = destination.rotation;
			}
		}
		Invoke("ReenableDestination",0.5f);

    }

	void ReenableDestination()
    {
		if(!oneWay)
			destinationTeleporter.enableThis();
    }
	void OnTriggerExit(Collider other) 
	{
		if(isEnabled)
		{
			if(objectChecksOut(other.gameObject)&&(!other.isTrigger))
			{
				if(EventsToSendOnExit.Count > 0)
				{
                    foreach (EventPackage ep in EventsToSendOnExit)
                    {
                        if ((ep.scope == eventScope.instigator) || (ep.scope == eventScope.visualScriptingInstigator))
                            EventRegistry.SendEvent(ep, other.gameObject);
                        else
                            EventRegistry.SendEvent(ep, this.gameObject);
                    }                        
                }
			}
		}
	}

}

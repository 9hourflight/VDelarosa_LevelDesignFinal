using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsCarryObject : MonoBehaviour 
{
	public string objectName;
	public bool overrideMassWhenCarried = false;
	public float carryMassOverride = 0f;
	public bool overrideThrowForce = false;
	public float throwForceMultiplier = 0f;
	float oldMass;
	public bool snapToRotation;
	public Vector3 snapRotation;
	public int baseDamage = 0;
    public signalTypes damageType = signalTypes.physics;
	public float velocityDamageThreshold = 1.0f;
    //public damageTypes damageType;
    public List<EventPackage> eventsToSendOnPickup;
    public List<EventPackage> eventsToSendOnDrop;
    public List<EventPackage> eventsToSendOnThrow;
	Rigidbody thisRB;
	void Start () 
	{
		thisRB = GetComponent<Rigidbody>();
		oldMass = thisRB.mass;
	}
	

	void Update () 
	{
		
	}

	public void pickupObject()
	{
		if (overrideMassWhenCarried)
		{
			
			thisRB.mass = carryMassOverride;
		}
        foreach (EventPackage ep in eventsToSendOnPickup)
            EventRegistry.SendEvent(ep, this.gameObject);
    }

	public void revertMass()
    {
		thisRB.mass = oldMass;
    }

	public void dropObject()
	{		
        foreach (EventPackage ep in eventsToSendOnDrop)
            EventRegistry.SendEvent(ep, this.gameObject);
		if (overrideMassWhenCarried)
			Invoke("revertMass", 2f);
    }

	public void throwObject()
	{		
        foreach (EventPackage ep in eventsToSendOnThrow)
            EventRegistry.SendEvent(ep, this.gameObject);
		if (overrideMassWhenCarried)
			Invoke("revertMass", 2f);
	}

	void OnCollisionEnter(Collision other)
	{
		NPCBehavior otherEnemy = other.gameObject.GetComponent<NPCBehavior>();
		
		if(otherEnemy != null)
		{
			if(thisRB.velocity.magnitude >= velocityDamageThreshold)
			{
				otherEnemy.Damage(baseDamage, damageType);
			}
		}
		if (overrideMassWhenCarried)
			thisRB.mass = oldMass;
	}
}

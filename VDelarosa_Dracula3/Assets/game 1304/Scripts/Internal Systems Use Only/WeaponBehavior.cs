using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBehavior : MonoBehaviour 
{	
	public float cooldown = 0.5f;
    protected bool isCooling;    
    protected float cooldownTimer;
    // Use this for initialization
    [Header("Event Sending")]
    public List<EventPackage> EventsToSendOnFire;

    protected virtual void Update()
    {
        if (isCooling)
            cooldownTimer -= Time.deltaTime;
        if(cooldownTimer<=0)
        {
            isCooling = false;
        }
    }
    

	public virtual void Fire()
	{
        foreach (EventPackage ep in EventsToSendOnFire)
        {
            if ((ep.scope == eventScope.instigator) || (ep.scope == eventScope.visualScriptingInstigator))
                EventRegistry.SendEvent(ep, GameManager.player);
            else
                EventRegistry.SendEvent(ep, this.gameObject);
        }
    }
}

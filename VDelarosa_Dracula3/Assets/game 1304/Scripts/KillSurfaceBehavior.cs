using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillSurfaceBehavior : MonoBehaviour 
{
    public bool damageOverTime = false;
    public float damageInterval = 1f;
    private float damageTimer = 0f;
    public int damageAmount = 100;
	public List<string> eventsOnDamage;
	public bool affectsPlayer = true;
	public bool affectsEnemies = false;
	public bool useAsTrigger = false;
    public signalTypes damageType = signalTypes.genericHazard;
    [Header("Event Listening")]
    public string eventToEnableThis;
    public string eventToDisableThis;
    private bool isEnabled;
    public bool startEnabled = true;
    List<GameObject> objectsInTrigger;

    void OnTriggerEnter(Collider other) 
	{
        if ((!other.isTrigger) && (useAsTrigger) && isEnabled)
        {
            objectsInTrigger.Add(other.gameObject);
            doDamage(other.gameObject);
        }
	}

    private void OnTriggerExit(Collider other)
    {
        if ((!other.isTrigger) && (useAsTrigger) && isEnabled)
        {
            objectsInTrigger.Remove(other.gameObject);            
        }
    }    

    void OnCollisionEnter(Collision collision)
	{
        if (damageOverTime)
            return;
        if ((!collision.collider.isTrigger) && (isEnabled))
        {
            doDamage(collision.gameObject);
            damageTimer = 0;
        }
        
	}

    private void OnCollisionStay(Collision collision)
    {
        if ((!collision.collider.isTrigger) && (isEnabled))
        {
            if (damageOverTime)
            {
                if (damageTimer > damageInterval)
                {
                    doDamage(collision.gameObject);
                    damageTimer = 0;
                }
            }
        }                                
    }

    private void Update()
    {
        damageTimer += Time.deltaTime;

        if ((isEnabled) && (useAsTrigger)&& damageOverTime)
        {            
            if (damageTimer > damageInterval)
            {
                foreach(GameObject go in objectsInTrigger)
                    doDamage(go);
                damageTimer = 0;
            }
            
        }
    }

    void doDamage(GameObject go)
	{
		bool damageDone = false;
		GAME1304PlayerController playerInfo = go.GetComponent<GAME1304PlayerController>();
		if ((playerInfo != null)&&(affectsPlayer))
		{
			playerInfo.takeDamage(damageAmount, damageType);
			damageDone = true;

		}

		NPCBehavior enemyInfo = go.GetComponent<NPCBehavior>();
		if ((enemyInfo  != null)&&(affectsEnemies))
		{
			enemyInfo.Damage(damageAmount, damageType);
			damageDone = true;
		}

        SignalReceiver sr = go.GetComponent<SignalReceiver>();
        if (sr != null) 
        {
            sr.processSignal(damageType, damageAmount);
            damageDone = true;
        }

        if (damageDone)
		{
            if (eventsOnDamage != null)
            {
                foreach (string s in eventsOnDamage)
                    EventRegistry.SendEvent(s);
            }
		}
	}

    public void enableThis(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        setEnabled(true);
        
    }

    public void setEnabled(bool newEnabled)
    {
        isEnabled = newEnabled;
    }
    public void disableThis(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        setEnabled(false);
        
    }

    void Start () 
	{
        isEnabled = startEnabled;
        if (eventToEnableThis != "")
            EventRegistry.AddEvent(eventToEnableThis, enableThis, gameObject);
        if (eventToDisableThis != "")
            EventRegistry.AddEvent(eventToDisableThis, disableThis, gameObject);
        objectsInTrigger = new List<GameObject>();
    }		
		
}

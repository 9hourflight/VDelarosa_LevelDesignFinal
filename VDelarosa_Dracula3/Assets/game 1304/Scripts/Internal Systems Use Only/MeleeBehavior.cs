using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBehavior : WeaponBehavior 
{
    public int damage;
    public signalTypes damageType = signalTypes.melee;
    private Quaternion baseRotation;
    private Quaternion swingRotation;
    //TODO: add damage types?	
    // Use this for initialization
    private float lerpValue;
	void Start () 
	{
        baseRotation = transform.rotation;
        //swingRotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z) + new Vector3(0,0,45));
        swingRotation = Quaternion.Euler(new Vector3(0, 0, 45));
    }

    private void Update()
    {
        if(isCooling)
        {
            transform.rotation = Quaternion.Euler(new Vector3(transform.parent.rotation.eulerAngles.x, transform.parent.rotation.eulerAngles.y, transform.parent.rotation.eulerAngles.z) + new Vector3(45, 0, 0));
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0)
                isCooling = false;
        }
        else
        {
            transform.rotation = transform.parent.rotation; // baseRotation;
        }
    }
    // Update is called once per frame
    void FixedUpdate () 
	{
        /*
        lerpValue += Time.deltaTime / rotationDuration;
        distanceToDestination = Quaternion.Angle(rb.transform.rotation, _rotationB);
        if (distanceToDestination <= 1.0f)
        {
            rb.MoveRotation(_rotationB);
            
            currentState = moverState.Waiting;
            _currentWaitTime = 0;
            nextState = moverState.MovingToA;
            waitTime = pauseDurationAtB;
            lerpValue = 0;
            if (eventsToFireAtB.Count > 0)
            {
                foreach (string s in eventsToFireAtB)
                {
                    EventRegistry.callEvent(s);
                }
            }
        }*/
    }

	public override void Fire()
	{
        RaycastHit hit;
        NPCBehavior eb;
        SignalReceiver sr;
        BreakableObject bo;

        RaycastHit[] raycastHits;
        if (!isCooling)
        {
            base.Fire();
            isCooling = true;
            cooldownTimer = cooldown;            
            //Physics.SphereCast(transform.root.position, 1.5f, transform.forward, out hit, 1.0f);
            raycastHits = Physics.SphereCastAll(transform.parent.position, 1.0f, transform.forward, 1.0f);
            foreach (RaycastHit rch in raycastHits)
            {
                if (rch.collider != null)
                {
                    if (!rch.collider.isTrigger)
                    {
                        eb = rch.collider.gameObject.GetComponent<NPCBehavior>();
                        if (eb != null)
                            eb.Damage(damage, damageType);

                        sr = rch.collider.gameObject.GetComponent<SignalReceiver>();
                        if (sr != null)
                            sr.processSignal(damageType, damage);

                        bo = rch.collider.gameObject.GetComponent<BreakableObject>();
                        if (bo != null)
                            bo.Damage(damage, damageType,rch.collider.transform.position-transform.parent.position);
                    }
                }
            }
        }
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionBehavior : MonoBehaviour
{
    public signalTypes damageType = signalTypes.fire;
    public int damageAmount = 50;
    public float explosionRadius = 10;
    public float explosionForce = 250;

    public string explosionEvent;

    void Start()
    {
        EventRegistry.Init();
        if (explosionEvent != "")
        {
            EventRegistry.AddEvent(explosionEvent, ExplodeOnEvent, gameObject);
        }
    }

    void ExplodeOnEvent(string eventName, GameObject obj)
    {
        //TODO: abstract this into signal receiver and/or health component
        SignalReceiver explosionReceiver;
        BreakableObject bo;
        NPCBehavior npc;
        GAME1304PlayerController pc;
        Rigidbody rb;

        if ((obj != null) && (obj != gameObject))
            return;
        foreach(Collider c in Physics.OverlapSphere(transform.position, explosionRadius))
        {
            if (c.gameObject != gameObject)
            {
                if (c.TryGetComponent<SignalReceiver>(out explosionReceiver))
                {
                    explosionReceiver.processSignal(damageType, damageAmount);
                }
                if (c.TryGetComponent<BreakableObject>(out bo))
                {
                    bo.Damage(damageAmount, damageType, (bo.gameObject.transform.position - transform.position).normalized);
                }
                if (c.TryGetComponent<NPCBehavior>(out npc))
                {
                    npc.Damage(damageAmount, damageType);
                }
                if (c.TryGetComponent<GAME1304PlayerController>(out pc))
                {
                    pc.takeDamage(damageAmount, damageType);
                }
                if (c.TryGetComponent<Rigidbody>(out rb))
                {
                    rb.AddExplosionForce(explosionForce, transform.position, explosionRadius,5);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

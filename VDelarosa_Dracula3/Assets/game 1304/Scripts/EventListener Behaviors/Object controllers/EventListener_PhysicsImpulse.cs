using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EventListener_PhysicsImpulse : MonoBehaviour
{
    public float force;
    [Tooltip("Relative offset from the object that the impulse should come from. Leave 0 if you want to use a direction vector instead.")]
    public Vector3 relativeLocationOfForce;
    [Tooltip("Euler direction of the force. Leave 0 if you want to use an offset vector instead.")]
    public Vector3 directionOfForce;
    [Tooltip("Duration of force. If 0, this will be a one time impulse. If higher this will be force over time.")]
    public float duration = 0f;
    
    
    [Header("Event Listening")]
    public string impulsePhysicsEvent;
    

    private Rigidbody _rb;
    private bool isImpulsing = false;
    private Vector3 forceVector;

    void Start()
    {
        EventRegistry.Init();
        if (impulsePhysicsEvent != "")
        {
            EventRegistry.AddEvent(impulsePhysicsEvent, impulsePhys, gameObject);
        }

        
        _rb = GetComponent<Rigidbody>();
        //TODO: add error if there isn't an RB
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isImpulsing)
        {
            DoForce(ForceMode.Force);
        }
    }

    public void impulsePhys(string eventName, GameObject obj)
    {
        
        if ((obj != null) && (obj != gameObject))
            return;
        if (_rb == null)
            return;
        if (duration > 0)
        {
            isImpulsing = true;
            Invoke("deactivateImpulse",duration);
        }
        else
        {
            DoForce(ForceMode.Impulse);

        }
        
    }
    private void deactivateImpulse()
    {
        isImpulsing = false;
    }

    void DoForce(ForceMode mode)
    {
        if(relativeLocationOfForce == Vector3.zero)
        { 
            forceVector = directionOfForce.normalized;
        }        
        else
        {
            forceVector = - (relativeLocationOfForce).normalized;
        }
        _rb.AddForce(forceVector * force, mode);
    }
}

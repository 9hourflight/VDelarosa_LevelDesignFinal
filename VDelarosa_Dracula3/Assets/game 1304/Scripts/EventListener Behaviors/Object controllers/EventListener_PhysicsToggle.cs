using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventListener_PhysicsToggle : MonoBehaviour
{
    public bool startPhysical = true;
    public bool startWithGravity = true;
    private bool isPhysical = false;
    private bool hasGravity = false;
    
    [Header("Event Listening")]
    public string enablePhysicsEventName;
    public string disablePhysicsEventName;
    public string togglePhysicsEventName;

    public string enableGravityEventName;
    public string disableGravityEventName;
    public string toggleGravityEventName;

    private Rigidbody _rb;
    private 

    void Start()
    {
        EventRegistry.Init();
        if (enablePhysicsEventName != "")
        {
            EventRegistry.AddEvent(enablePhysicsEventName, updatePhysState, gameObject);
        }

        if (disablePhysicsEventName != "")
        {
            EventRegistry.AddEvent(disablePhysicsEventName, updatePhysState, gameObject);
        }

        if (togglePhysicsEventName != "")
        {
            EventRegistry.AddEvent(togglePhysicsEventName, updatePhysState, gameObject);
        }

        if (enableGravityEventName != "")
        {
            EventRegistry.AddEvent(enableGravityEventName, updateGravState, gameObject);
        }

        if (disableGravityEventName != "")
        {
            EventRegistry.AddEvent(disableGravityEventName, updateGravState, gameObject);
        }

        if (toggleGravityEventName != "")
        {
            EventRegistry.AddEvent(toggleGravityEventName, updateGravState, gameObject);
        }

        _rb = GetComponent<Rigidbody>();


        if (startPhysical)
        {
            setPhysics(true);
        }
        else
            setPhysics(false);

        if (startWithGravity)
        {
            setGravity(true);
        }
        else
            setGravity(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void updatePhysState(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != gameObject))
            return;
        if (_rb != null)
        {
            if (eventName == enablePhysicsEventName)
                setPhysics(true);
            if (eventName == disablePhysicsEventName)
                setPhysics(false);
            if (eventName == togglePhysicsEventName)
                setPhysics(_rb.isKinematic);
        }
    }

    private void setPhysics(bool state)
    {
        _rb.isKinematic = !state;
    }

    private void setGravity(bool state)
    {
        _rb.useGravity = state;
    }

    public void updateGravState(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != gameObject))
            return;
        if (_rb != null)
        {
            if (eventName == enableGravityEventName)
                setGravity(true);
            if (eventName == disableGravityEventName)
                setGravity(false);
            if (eventName == toggleGravityEventName)
                setGravity(!_rb.useGravity); 
        }
    }

}

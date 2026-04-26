///This behavior listens for an event and then fires a projectile in the forward facing of the object it is attached to

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventListener_ProjectileLauncher : MonoBehaviour
{
    [Header("Event Listening")]
    public List<string> eventsToEnableThis;
    public List<string> eventsToDisableThis;
    public List<string> eventsToFireOnce;
    [Tooltip("If constant fire is triggered, it will continue to fire at the value set by fireRate until it is told to stop through the event system")]
    public List<string> eventsToStartConstantFiring;
    public List<string> eventsToStopConstantFiring;
    public List<string> eventsToToggleConstantFiring;
    [Space(5)]
    [Header("Firing parameters")]
    [Tooltip("The interval, in seconds, for the projectiles to fire if constant fire has been started")]
    public float fireRate = 1.0f;
    [Tooltip("This will only fire if it is enabled. Value can be set through the event system")]
    public bool isEnabled = true;
    [Tooltip("If true, the launcher will start in constant fire mode and will only stop if an event tells it to")]
    public bool startsFiringConstantly = false;
    private bool isConstantlyFiring = false;

    public GameObject projectilePrefabToLaunch;
    
    void Start()
    {
        foreach (string s in eventsToEnableThis)
        {            
            EventRegistry.AddEvent(s, enableOnEvent, gameObject);            
        }

        foreach (string s in eventsToDisableThis)
        {
            EventRegistry.AddEvent(s, disableOnEvent, gameObject);
        }

        foreach (string s in eventsToStartConstantFiring)
        {
            EventRegistry.AddEvent(s, startConstantOnEvent, gameObject);
        }

        foreach (string s in eventsToStopConstantFiring)
        {
            EventRegistry.AddEvent(s, stopConstantOnEvent, gameObject);
        }

        foreach (string s in eventsToToggleConstantFiring)
        {
            EventRegistry.AddEvent(s, toggleConstantOnEvent, gameObject);
        }

        foreach (string s in eventsToFireOnce)
        {
            EventRegistry.AddEvent(s, fireProjectileOnEvent, gameObject);
        }
        
        if (startsFiringConstantly)
            startConstantOnEvent("",null);
    }

    void enableOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        isEnabled = true;
    }

    void disableOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        isEnabled = false;
    }
    
    void fireProjectileOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        fireProjectile();
    }

    void startConstantOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        if (!isConstantlyFiring)
        {
            InvokeRepeating("fireProjectile", 0, fireRate);
            isConstantlyFiring = true;
        }
    }

    void stopConstantOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        CancelInvoke();
        isConstantlyFiring = false;
    }

    void toggleConstantOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        if (isConstantlyFiring)
            stopConstantOnEvent(eventName, obj);
        else
            startConstantOnEvent(eventName, obj);
    }

    void fireProjectile()
    {
        GameObject bulletObj = GameObject.Instantiate(projectilePrefabToLaunch, transform.position,transform.rotation);
        bulletObj.transform.parent = null;
        if (bulletObj.GetComponent<BulletBehavior>() != null)
            bulletObj.GetComponent<BulletBehavior>().init(transform.forward);
    }

    void Update()
    {
        
    }
}

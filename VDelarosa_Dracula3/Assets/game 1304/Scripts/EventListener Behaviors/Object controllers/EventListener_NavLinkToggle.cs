using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshLink))]
public class EventListener_NavLinkToggle : MonoBehaviour
{
    NavMeshLink nml;
    public bool linkStartEnabled = true;

    [Header("Event Listening")]
    public string enableLinkEvent;
    public string disableLinkEvent;
    public string toggleLinkEvent;
    void Start()
    {
        nml = GetComponent<NavMeshLink>();        
        nml.enabled = linkStartEnabled;       
        
        EventRegistry.AddEvent(enableLinkEvent, enableOnEvent, gameObject);        
    }

    private void enableOnEvent(string eventName, GameObject obj)
    {
        nml.enabled = true;
    }

    private void disableOnEvent(string eventName, GameObject obj)
    {
        nml.enabled = false;
    }

    private void toggleeOnEvent(string eventName, GameObject obj)
    {
        nml.enabled = !nml.enabled;
    }

    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using System;

[Serializable]
public class InteractiveObject : MonoBehaviour 
{
	public string interactLabel;
    public string interactVerb;
    [Tooltip("If TRUE, this object can only be used once before becoming unusable.")]
    public bool UseOnce = false;
    public bool startEnabled = true;
    [Header("Event Sending")]
    public List<EventPackage> eventsToSend;    
    [Tooltip("Delay in seconds before interaction fires.")]
    public float delay = 0f;

    [Header("Feedback")]
    public Material activeMaterial;
    public Material usingMaterial;
    public Material inactiveMaterial;
    public Material highlightMaterial;
    public Vector3 useOffset;
    public AudioClip useSound;
    private bool materialChanging = false;

    [Header("Event Listening")]
    public string enableThisEvent;
	public string disableThisEvent;

    UnityEvent usedEvent;
    
	public virtual bool isEnabled
    {
        get {return _isEnabled;}
        set
        {
            _isEnabled = value;
            if (_isEnabled)
            {
                if (activeMaterial != null)
                    GetComponent<Renderer>().material = activeMaterial;
                //else
                //  activeMaterial = GetComponent<Renderer>().material;
            }
            else
            {
                //if (!materialChanging)
                {
                    if (inactiveMaterial != null)
                        GetComponent<Renderer>().material = inactiveMaterial;
                }
                // else
                //    inactiveMaterial = GetComponent<Renderer>().material;
            }
        }
    }
	protected bool _isEnabled;
	[Header("Inventory Interaction")]
	//public string InventoryItemNeeded;
    public InventoryDef InventoryItemNeeded;
	public int quantityRequired = 1;
    public bool hideUIForItemNeeded = false;
    public string itemNeededOverrideLabel = "";
	[Tooltip("If TRUE, the inventory item will be removed from the player's inventory when they use this object.")]
	public bool ConsumeInventoryItem;
	protected  bool _used;
	public bool isUsed {get{ return _used;} }
    private Vector3 oldPosition;

    public virtual void Start () 
	{
		_used = false;
		_isEnabled = startEnabled;
        oldPosition = transform.position;
        usedEvent = new UnityEvent();
        EventRegistry.AddEvent(enableThisEvent, enableThisOnEvent, gameObject);
        EventRegistry.AddEvent(disableThisEvent, disableThisOnEvent, gameObject);
        
        if (isEnabled)
        {
            if (activeMaterial != null)
                GetComponent<Renderer>().material = activeMaterial;
            else
                activeMaterial = GetComponent<Renderer>().material;
            //else
            //  activeMaterial = GetComponent<Renderer>().material;
        }
        else
        {
            if (activeMaterial != null)
                GetComponent<Renderer>().material = inactiveMaterial;
           // else
            //    inactiveMaterial = GetComponent<Renderer>().material;
        }
    }

	public void onInteract()
	{		
		if(UseOnce && _used)
			return;
		if(!isEnabled)
			return;
		_used = true;
        Invoke("interact", delay);        
	}

    public virtual void interact()
    {
        AudioSource source;
        if(useSound != null)
        {
            if(TryGetComponent<AudioSource>(out source))
            {
                source.PlayOneShot(useSound);
            }
        }
        if(usingMaterial != null)
        {
            GetComponent<Renderer>().material = usingMaterial;
            materialChanging = true;
        }
        //transform.SetPositionAndRotation(transform.position + useOffset,transform.rotation);
        if (useOffset.magnitude > 0)
        {
            transform.localPosition += transform.up * useOffset.y; //  transform.position + useOffset;
            
        }
        Invoke("returnToNormal", 0.25f);

        foreach (EventPackage ep in eventsToSend)
        {
            if((ep.scope == eventScope.instigator)||(ep.scope == eventScope.visualScriptingInstigator))
                EventRegistry.SendEvent(ep, GameManager.player);
            else
                EventRegistry.SendEvent(ep, this.gameObject);
        }
        
        usedEvent.Invoke();
    }

    void returnToNormal()
    {
        //transform.SetPositionAndRotation(oldPosition, transform.rotation);
        transform.position = oldPosition;
        materialChanging = false;
        if (UseOnce && _used)
            isEnabled = false;
        if ((UseOnce && _used)||(!isEnabled))
        {
            if(inactiveMaterial != null)
            {
                GetComponent<Renderer>().material = inactiveMaterial;
            }
        }
        else
        {
            if(activeMaterial != null)
            {
                GetComponent<Renderer>().material = activeMaterial;
            }
        }
    }

	void enableThisOnEvent(string eventName, GameObject obj)
	{
        if ((obj != null) && (obj != this.gameObject))
            return;
        isEnabled = true;
        _used = false;
	}

	void disableThisOnEvent(string eventName, GameObject obj)
	{
        if ((obj != null) && (obj != this.gameObject))
            return;
        isEnabled = false;
	}

	
}

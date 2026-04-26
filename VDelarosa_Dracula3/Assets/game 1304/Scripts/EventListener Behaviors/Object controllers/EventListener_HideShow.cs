using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventListener_HideShow : MonoBehaviour 
{
	public bool startHidden;
	private bool isHidden;
	public bool includeCollision;
    [Header("Event Listening")]
    public string hideEventName;
	public string showEventName;
	public string toggleEventName;


	private Renderer _renderer;
	private Collider _collider;
	private Flare _flare;
	// Use this for initialization
	void Start () 
	{
		EventRegistry.Init();
		if (hideEventName != "")
		{
			EventRegistry.AddEvent (hideEventName, hideOnEvent, gameObject);
		}

		if (showEventName != "")
		{
			EventRegistry.AddEvent (showEventName, showOnEvent, gameObject);
		}

		if (toggleEventName != "")
		{
			EventRegistry.AddEvent (toggleEventName, toggleOnEvent, gameObject);
		}

		_renderer = GetComponent<Renderer>();
		_collider = GetComponent<Collider>();
		_flare = GetComponent<Flare>();

		if (startHidden)
		{
			hideOnEvent ("",null);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

    public void hide()
    {
        isHidden = true;
        HelperFunctions.hideObjectAndChildren(this.gameObject);
        /*if (_renderer != null)
            _renderer.enabled = false;*/
        if (includeCollision)
            HelperFunctions.disableCollisionObjectAndChildren(this.gameObject);
    }

	private void hideOnEvent(string eventName, GameObject obj)
	{
        if ((obj != null) && (obj != gameObject))
            return;
        hide();                
	}

    public void show()
    {
        isHidden = false;
        HelperFunctions.showObjectAndChildren(this.gameObject);
        /*if (_renderer != null)
			_renderer.enabled = true;*/
        if (includeCollision)
            HelperFunctions.enableCollisionObjectAndChildren(this.gameObject);
    }

	public void showOnEvent(string eventName, GameObject obj)
	{
        if ((obj != null) && (obj != gameObject))
            return;
        show();        
	}


    public void toggleHidden()
    {
        if (isHidden)
            show();
        else
            hide();
    }

	private void toggleOnEvent(string eventName, GameObject obj)
	{
        if ((obj != null) && (obj != gameObject))
            return;
        toggleHidden();        
	}
}

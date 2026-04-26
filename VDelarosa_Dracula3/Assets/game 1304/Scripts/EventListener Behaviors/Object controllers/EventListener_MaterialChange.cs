using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MaterialEntry
{
	public string eventName;
	public Material material;
}

public class EventListener_MaterialChange : MonoBehaviour 
{	
	[Tooltip("Events to listen for that will change the color of the light.")]
	public  List<MaterialEntry> materialChangeEvents;
	
	// Use this for initialization
	private Renderer _renderer;

	void Start () 
	{
        _renderer = GetComponent<Renderer>();


		foreach(MaterialEntry me in materialChangeEvents)
		{
			EventRegistry.AddEvent(me.eventName, MaterialChange, gameObject);
		}

		
			
	}
	
	void MaterialChange(string eventName, GameObject obj)
	{
        if ((obj != null) && (obj != gameObject))
            return;
        foreach (MaterialEntry me in materialChangeEvents)
		{
            if (me.eventName == eventName)
                _renderer.material = me.material;
		}
		
	}
}
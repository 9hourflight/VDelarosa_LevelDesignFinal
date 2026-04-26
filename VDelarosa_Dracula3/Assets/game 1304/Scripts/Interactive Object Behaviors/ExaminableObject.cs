using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExaminableObject : MonoBehaviour 
{
	public string examineItemName;
	public Sprite examineImage;
    [Header("Event Sending")]
    public List<EventPackage> EventsToSendOnExamine;

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}

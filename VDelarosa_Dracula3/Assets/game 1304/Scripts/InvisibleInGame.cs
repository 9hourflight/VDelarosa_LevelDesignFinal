using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]

public class InvisibleInGame : MonoBehaviour 
{
	private Renderer rend;
	// Use this for initialization
	void Start () 
	{
		rend = GetComponent<Renderer>();
		rend.enabled = false;
	}
	
}

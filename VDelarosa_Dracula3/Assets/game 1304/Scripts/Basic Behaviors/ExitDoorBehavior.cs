using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoorBehavior : MonoBehaviour 
{
	private bool gameOver = false;
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	void OnGUI() 
	{
		GUI.skin.label.fontSize = 50;
		var textDimensions = GUI.skin.label.CalcSize(new GUIContent("text"));
		if (gameOver)
			
			GUI.Label(new Rect(Screen.width /2 - textDimensions.x/2, Screen.height/2 - textDimensions.y/2, Screen.width/2 + textDimensions.x/2, Screen.height/2 + textDimensions.y/2), "LEVEL COMPLETE");
	}

	void OnTriggerEnter(Collider other) 
	{
		GameObject go = other.gameObject;
		if (go.tag == "Player")
		{
			gameOver = true;
		}
	}

}

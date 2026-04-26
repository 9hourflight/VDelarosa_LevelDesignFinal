using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureCollectorBehavior : MonoBehaviour 
{
    public string treasureToken = "";
	// Use this for initialization
	void Start () 
	{
		TokenRegistry.setToken(treasureToken, 0);
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	//void OnControllerColliderHit(ControllerColliderHit hit)
	void OnCollisionEnter (Collision col)
	{
		TreasureBehavior tb = col.gameObject.gameObject.GetComponent<TreasureBehavior>();
		if (tb != null)
		{
			TokenRegistry.modifyToken(treasureToken, tb.amount, operationType.add);
			tb.collectTreasure();
            
		}
	}

}

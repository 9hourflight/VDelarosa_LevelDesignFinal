using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureBehavior : MonoBehaviour {

	public GameObject TreasureParticle;
	public int amount = 1;
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void collectTreasure()
	{
		if (TreasureParticle != null)
		{
			ParticleSystem ps = TreasureParticle.GetComponent<ParticleSystem>();
			ps.transform.parent = null;
			ps.Play();
		}
			Destroy(gameObject);			
		
	}
}

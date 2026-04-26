using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnAtSpawnPoint : MonoBehaviour 
{
	public GameObject SpawnPointObject;
	private Vector3 respawnLocation;
	private Quaternion respawnRotation;
	public GameObject playerCamera;
	// Use this for initialization

	void Start () 
	{		
		respawnLocation = SpawnPointObject.transform.position;
		respawnRotation = SpawnPointObject.transform.rotation;
		//playerCamera =  transform.Find("FirstPersonCamera").gameObject;// gameObject.GetComponentInChildren<Camera>();

		respawn();
	}

	public void respawn()
	{
		
		transform.SetPositionAndRotation(respawnLocation,respawnRotation);

		//transform.root.position = respawnLocation;
		//if (playerCamera != null)
		//	playerCamera.transform.localRotation = respawnRotation; // SetPositionAndRotation(respawnLocation,respawnRotation); // .rotation = respawnRotation;		
	}

	void OnControllerColliderHit(ControllerColliderHit hit)
	//void OnCollisionEnter (Collision col)
	{
		KillSurfaceBehavior ksb = hit.gameObject.gameObject.GetComponent<KillSurfaceBehavior>();
		if (ksb != null)
		{
			respawn();
		}

		CheckpointBehavior cpb = hit.gameObject.GetComponent<CheckpointBehavior>();
		if (cpb != null)
		{
			respawnLocation = hit.gameObject.transform.position;
			respawnRotation = hit.gameObject.transform.rotation;
			DestroyObject(hit.gameObject);
		}

	}
	// Update is called once per frame
	void Update () 
	{
		
	}
}

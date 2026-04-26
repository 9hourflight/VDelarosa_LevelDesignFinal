using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class HelperFunctions 
{
    public static void sendSignal(GameObject go, int signalAmount, signalTypes signalType)
    {
        
        GAME1304PlayerController playerInfo = go.GetComponent<GAME1304PlayerController>();
        if (playerInfo != null)         
            playerInfo.takeDamage(signalAmount, signalType);
            
        NPCBehavior enemyInfo = go.GetComponent<NPCBehavior>();
        if (enemyInfo != null)         
            enemyInfo.Damage(signalAmount, signalType);

        SignalReceiver sr = go.GetComponent<SignalReceiver>();
        if (sr != null)
            sr.processSignal(signalType,signalAmount);
        
    }

    public static void hideObjectAndChildren(GameObject go)
	{        
		Component h;
        NavMeshObstacle obs;
		Renderer r;
		ParticleSystem ps;
		Light l;
		if(go == null)
			return;		
		int i;
		if (go.name.ToLower() == "torchbase")
			i = 0;
        //go.SetActive(false);		
        if (go.TryGetComponent<Renderer>(out r))
        {
            r.enabled = false;
        }
		/*h = go.GetComponent("Halo");
		if(h != null)
        
        if (go.TryGetComponent(typeof("Halo"),out h) )
        {
			h.GetType().GetProperty("enabled").SetValue(h, false, null);
		}*/

        if(go.TryGetComponent<NavMeshObstacle>(out obs))
        { 
        /*obs = go.GetComponent<NavMeshObstacle>();
        if (obs != null)
        {*/
            obs.enabled = false;
        }

        /*ps = go.GetComponent<ParticleSystem>();
		if(ps != null)*/
        if (go.TryGetComponent<ParticleSystem>(out ps))
        {
			ps.Stop();
		}			


		/*l = go.GetComponent<Light>();
		if(l != null)*/
        if (go.TryGetComponent<Light>(out l))
        {
			l.enabled = false;
		}

		foreach(Transform child in go.GetComponentInChildren<Transform>())
		{
			hideObjectAndChildren(child.gameObject);
		}			
	}

	public static void showObjectAndChildren(GameObject go)
	{
        
        Component h;
        NavMeshObstacle obs;
		Renderer r;
		ParticleSystem ps;
		Light l;

		if(go == null)
			return;		
		if((go.GetComponent<Renderer>() != null)&&(go.GetComponent<InvisibleInGame>()==null))			
			go.GetComponent<Renderer>().enabled = true;

        h = go.GetComponent("Halo");
		if(h != null)        
		{
			h.GetType().GetProperty("enabled").SetValue(h, true, null);
		}

        /*obs = go.GetComponent<NavMeshObstacle>();
        if (obs != null)*/
        if(go.TryGetComponent<NavMeshObstacle>(out obs))
        {
            obs.enabled = true;
        }

        /*ps = go.GetComponent<ParticleSystem>();
		if(ps != null)*/
        if(go.TryGetComponent<ParticleSystem>(out ps))
		{
			ps.Play();
		}

		/*l = go.GetComponent<Light>();
		if(l != null)*/
        if(go.TryGetComponent<Light>(out l))
		{
			l.enabled = true;
		}

		foreach(Transform child in go.GetComponentInChildren<Transform>())
		{
			showObjectAndChildren(child.gameObject);
		}


	}

	public static void disableCollisionObjectAndChildren(GameObject go)
	{
		if(go == null)
			return;		

		if(go.GetComponent<Collider>() != null)
			go.GetComponent<Collider>().enabled = false;

		foreach(Collider c in go.GetComponentsInChildren<Collider>())
		{						
			c.enabled = false;
		}			
	}

	public static void enableCollisionObjectAndChildren(GameObject go)
	{
		if(go == null)
			return;		

		if(go.GetComponent<Collider>() != null)
			go.GetComponent<Collider>().enabled = true;

		foreach(Collider c in go.GetComponentsInChildren<Collider>())
		{						
			c.enabled = true;
		}			
	}

}

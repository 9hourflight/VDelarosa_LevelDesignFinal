using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventListener_ParticleStart : MonoBehaviour 
{

	private ParticleSystem _particle;
	public string startParticleEvent;
	public string stopParticleEvent;
	public bool startOn=false;

	void Start () 
	{
		EventRegistry.Init();
		if (startParticleEvent != "")
		{
			EventRegistry.AddEvent(startParticleEvent, startParticle, gameObject);
		}
		if (stopParticleEvent != "")
		{
			EventRegistry.AddEvent(stopParticleEvent, stopParticle, gameObject);
		}
		_particle = GetComponent<ParticleSystem>();
		if(_particle != null)
		{
			if(startOn)
				_particle.Play();
			else
				_particle.Stop();
		}
	}

	void startParticle(string eventName, GameObject obj)
	{
        if ((obj != null) && (obj != gameObject))
            return;
        if (_particle != null)
			_particle.Play();
	}

	void stopParticle(string eventName, GameObject obj)
	{
        if ((obj != null) && (obj != gameObject))
            return;
        if (_particle != null)
			_particle.Stop();
	}

	void Update () 
	{
		
	}
}

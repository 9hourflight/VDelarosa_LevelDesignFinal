using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EventListener_PlaySound : MonoBehaviour 
{
	private AudioSource _audio;
	public string startSoundEvent;
	public string stopSoundEvent;
    public float randomPitchVariance = 0;
	public bool playLooped = false;
	[Tooltip("If you specify a clip here, it will play instead of the source's default clip.")]
	public AudioClip overrideAudioClipToPlay;
	// Use this for initialization
	void Start () 
	{
		EventRegistry.Init();
		if (startSoundEvent != "")
		{
			EventRegistry.AddEvent(startSoundEvent, startSound, gameObject);
		}
		if (stopSoundEvent != "")
		{
			EventRegistry.AddEvent(stopSoundEvent, stopSound, gameObject);
		}
		_audio = GetComponent<AudioSource>();

	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	private void startSound(string eventName, GameObject obj)
	{
		CancelInvoke();
        if ((obj != null) && (obj != gameObject))
            return;
        if (_audio != null)
        {
			//TODO - allow for pitch variance if looping
			RandomizePitch();
			
			//if (_audio.clip != null)
			{
				if (playLooped)
				{
					if (overrideAudioClipToPlay != null)
						_audio.clip = overrideAudioClipToPlay;
					_audio.loop = true;
					_audio.Play();
					if (randomPitchVariance > 0)
						Invoke("RandomizePitch", _audio.clip.length);
				}
				else
				{
					if(overrideAudioClipToPlay != null)
						_audio.PlayOneShot(overrideAudioClipToPlay);
					else
						_audio.PlayOneShot(_audio.clip);
				}
				
			}
        }
	}

	private void RandomizePitch()
    {
		if (randomPitchVariance > 0)
			_audio.pitch = 1 + ((Random.Range(0, (int)(randomPitchVariance * 100)) / 100f) * 2) - (randomPitchVariance);
		Invoke("RandomizePitch", _audio.clip.length);
	}
	private void stopSound(string eventName, GameObject obj)
	{
		CancelInvoke();
		if ((obj != null) && (obj != gameObject))
            return;
        if (_audio != null)
			_audio.Stop();
	}
}

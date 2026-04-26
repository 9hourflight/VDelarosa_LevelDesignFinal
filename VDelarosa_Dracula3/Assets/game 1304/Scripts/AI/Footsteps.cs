using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum SurfaceType { none, stone, grass, glass};

[Serializable]
public class SurfaceFootstepPairing
{
    public SurfaceType surface;
    public AudioClip sound;
}

public class Footsteps : MonoBehaviour
{
    public List<SurfaceFootstepPairing> footstepTypes;
    public float strideSpacing = 0.5f;
    public float pitchVariance = 0.1f;
    public float volumeOverride = 1f;

    public AudioSource audioSource;
    private float distanceCount = 0f;
    private AudioClip footstepClip;

    private bool isMuted = false;

    private void Start()
    {
        //eventually make this a component reference, add a child object with a source at the feet
        //audioSource = GetComponent<AudioSource>();
        if(footstepTypes.Count > 0)
        {
            if(footstepTypes[0].sound != null)
            {
                footstepClip = footstepTypes[0].sound;
            }
        }
    }

    public void moveForward(float distance, SurfaceType surface)
    {
        distanceCount += distance;
        if (distanceCount>= strideSpacing)
        {
            if (!isMuted)
            {
                audioSource.pitch = 1 + UnityEngine.Random.Range(-pitchVariance, pitchVariance);
                audioSource.PlayOneShot(footstepClip,volumeOverride);
            }
            
            distanceCount = 0;
        }

    }
    public void setMuted(bool newMuted)
    {
        isMuted = newMuted;
    }
}

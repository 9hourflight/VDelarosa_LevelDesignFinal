using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class EventListener_AnimController : MonoBehaviour
{
    private Animation animation;
    private Animator animator;
    public bool startOn = false;
    //public bool isLooping = false;

    [Header("Event Listening")]
    public List<string> eventsToStart;
    public List<string> eventsToPause;
    public List<string> eventsToResume;
    public List<string> eventsToRewind;

    [Header("Event Sending")]
    public List<EventPackage> EventsToSendOnFinish;

    void Start()
    {
        animation = GetComponent<Animation>();
        animator = GetComponent<Animator>();
        
        //animation.playAutomatically = startOn;

        if (!startOn)
            animator.StopPlayback(); // .Play("FeaturesTutorialCameraAnim");
        else
            animator.Play("FeaturesTutorialCameraAnim");

        //TODO: figure out looping

        foreach (string s in eventsToStart)
            EventRegistry.AddEvent(s, startOnEvent, gameObject);
        foreach (string s in eventsToPause)
            EventRegistry.AddEvent(s, pauseOnEvent, gameObject);
        foreach (string s in eventsToResume)
            EventRegistry.AddEvent(s, resumeOnEvent, gameObject);
        foreach (string s in eventsToRewind)
            EventRegistry.AddEvent(s, rewindOnEvent, gameObject);
    }

    void startOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != gameObject))
            return;
        //animation.Rewind(animation.clip.name);
        animator.StartPlayback();
        animator.Play("FeaturesTutorialCameraAnim");
    }

    void pauseOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != gameObject))
            return;
        animator.StopPlayback();
        //animation.Stop(animation.clip.name);
    }

    void resumeOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != gameObject))
            return;
        animator.StartPlayback();
        animator.Play("FeaturesTutorialCameraAnim");
    }

    void rewindOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != gameObject))
            return;
        //animator.
        animation.Rewind(animation.clip.name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

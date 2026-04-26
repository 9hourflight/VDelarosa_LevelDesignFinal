using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum fadeDirection { fadeDown, fadeUp};
public enum conflictResolution { drop, interrupt, queue};

[System.Serializable]
public class FadeEvent
{
    public string eventToListenFor;
    public fadeDirection direction;
    public Color fadeColor;    
    public float duration;
    public List<EventPackage> eventsToSendOnCompletion;
    public conflictResolution resolutionOnFadeConflict = conflictResolution.queue;
}

[RequireComponent(typeof(Camera))]
public class EventListener_CameraControls : MonoBehaviour
{
    [Header("Event Listening")]
    public List<FadeEvent> fadeEvents;
    public List<string> eventsToMakeThisCameraMain;
    [Tooltip("The camera will stay active for this long before reverting to the previous camera. A vlue of 0 means indefinite duration.")]
    public float DurationBeforeReverting = 0f;
    private bool isFading;    
    Camera cam;
    Camera oldCamera;
    private Color startColor, endColor;
    private float fadeDuration, currentFadeTime, currentFadeRatio;
    private Color clearColor;
    private Texture2D _texture;
    private Queue<FadeEvent> queuedFadeEvents;
    FadeEvent currentFadeEvent;
    //TODO: add conflict resolution to fade system

    void Start()
    {
        cam = GetComponent<Camera>();
        clearColor = new Color(0, 0, 0, 0);
        foreach(FadeEvent fe in fadeEvents )
        {
            EventRegistry.AddEvent(fe.eventToListenFor, fadeOnEvent, gameObject);
        }
        foreach(string s in eventsToMakeThisCameraMain)
        {
            EventRegistry.AddEvent(s, takeOverOnEvent, gameObject);
        }
        queuedFadeEvents = new Queue<FadeEvent>();
    }

    private void takeOverOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        if (Camera.main != null)
        {
            oldCamera = Camera.main;
            Camera.main.enabled = false;
        }
        foreach (Camera ca in FindObjectsOfType<Camera>())
        {
            ca.enabled = false;
        }
        cam.enabled = true;
        
        if (DurationBeforeReverting > 0)
            Invoke("RevertCam", DurationBeforeReverting);
    }

    void RevertCam()
    {
        cam.enabled = false;
        oldCamera.enabled = true;
    }
    private void startFade(FadeEvent fe)
    {
        currentFadeEvent = fe;
        if (fe.direction == fadeDirection.fadeDown)
        {
            startColor = clearColor;
            endColor = fe.fadeColor;
        }
        else
        {
            startColor = fe.fadeColor;
            endColor = clearColor;
        }
        isFading = true;
        currentFadeTime = 0;
        fadeDuration = fe.duration;
    }

    
    private void fadeOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        foreach (FadeEvent fe in fadeEvents)
        {
            if (eventName == fe.eventToListenFor)
            {
                if (!isFading)
                    startFade(fe);
                else
                {
                    switch (fe.resolutionOnFadeConflict)
                    {
                        case conflictResolution.drop:
                            break;
                        case conflictResolution.interrupt:
                            startFade(fe);
                            break;
                        case conflictResolution.queue:                                                
                            queuedFadeEvents.Enqueue(fe);
                            break;
                    }
                }
            }
        }
        
            
    }


    private void Update()
    {
        if (isFading)
        {
            currentFadeTime += Time.deltaTime;
            currentFadeRatio = Mathf.InverseLerp(0, fadeDuration, currentFadeTime);
            if (currentFadeTime > fadeDuration)
            {
                isFading = false;
                foreach(EventPackage ep in currentFadeEvent.eventsToSendOnCompletion )
                {
                    EventRegistry.SendEvent(ep, this.gameObject);
                }
            }
        }
        else
        {
            if (queuedFadeEvents.Count > 0)
            {
                startFade(queuedFadeEvents.Dequeue());
            }
        }
    }

    private void OnGUI()
    {        
        if (_texture == null) _texture = new Texture2D(1, 1);
        _texture.SetPixel(0, 0, Color.Lerp(startColor,endColor,currentFadeRatio)); // new Color(0, 0, 0, 255));
        _texture.Apply();           
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _texture);        
    }
}

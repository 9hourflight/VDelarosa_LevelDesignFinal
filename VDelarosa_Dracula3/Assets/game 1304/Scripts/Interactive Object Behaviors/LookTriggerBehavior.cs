using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class LookTriggerBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Look At Properties")]
    public float percentFromCenter = 0.5f;
    public float minimumDuration = 0f;
    public float triggerDistance = 10f;
    public bool triggerThroughWalls = false;

    [Header("Event Sending")]
    public List<EventPackage> EventsToSendOnLookAt;
    public List<EventPackage> EventsToSendOnLookAway;

    [Header("Event Listening")]
    public string eventToEnableThis;
    public string eventToDisableThis;

    [Header("Trigger Properties")]
    public int maxTriggerCount = 0;
    private int currentTriggerCount;
    public bool startEnabled = true;
    //public bool PlayerOnly = true;
    private bool isEnabled;

    private bool isBeingLookedAt = false;
    private bool playerIsInRange = false;
    private float lookTimer = 0f;

    private SphereCollider radiusSphere;
    private Camera playercamera;    

    void Start()
    {
        isEnabled = startEnabled;
        if (eventToEnableThis != "")
            EventRegistry.AddEvent(eventToEnableThis, enableThisOnEvent, gameObject);
        if (eventToDisableThis != "")
            EventRegistry.AddEvent(eventToDisableThis, disableThisOnEvent, gameObject);
        radiusSphere = gameObject.AddComponent<SphereCollider>();
        radiusSphere.radius = triggerDistance;
        radiusSphere.isTrigger = true;
        playercamera = GameManager.player.gameObject.GetComponentInChildren<Camera>();        
    }

    void OnTriggerEnter(Collider other)
    {
      if (isEnabled)
        {
            if (other.gameObject == GameManager.player)
            {
                playerIsInRange = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (isEnabled)
        {
            if (other.gameObject == GameManager.player)
            {
                playerIsInRange = false;
                lookTimer = 0f;
                if (isBeingLookedAt)
                {
                    isBeingLookedAt = false;
                    triggerLookAway();
                }
            }
        }
    }

    void Update()
    {
        RaycastHit hitInfo;
        if (playerIsInRange)
        {
            Vector3 screenPoint = playercamera.WorldToViewportPoint(gameObject.transform.position);
            bool onScreen = screenPoint.z > 0 && (screenPoint.x > (0.5f- percentFromCenter/2f)) && (screenPoint.x < (0.5f+percentFromCenter/2f)) && (screenPoint.y > (0.5f - percentFromCenter/2f)) && (screenPoint.y < (0.5f + percentFromCenter/2f));
            if(triggerThroughWalls == false)
            {
                if (Physics.Linecast(gameObject.transform.position, playercamera.transform.position,out hitInfo, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                {
                    if((hitInfo.collider.gameObject != gameObject)&&(hitInfo.collider.gameObject != GameManager.player))
                        onScreen = false;
                }
            }
            //if (playercamera.)
            //faking it until I get the proper look angle checks
            if (onScreen)
            {
                isBeingLookedAt = true;
                lookTimer += Time.deltaTime;
                if (lookTimer >= minimumDuration)
                {
                    triggerLookAt();
                }
            }
            else
            {
                if(isBeingLookedAt)
                {
                    triggerLookAway();
                    isBeingLookedAt = false;
                    lookTimer = 0;
                }
                
            }

        }
        else
            lookTimer = 0f;
    }

    public void enableThis()
    {
        isEnabled = true;
    }
    private void enableThisOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        enableThis();
    }

    public void disableThis()
    {
        isEnabled = false;
    }
    private void disableThisOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        disableThis();
    }

    private void triggerLookAt()
    {
      
        foreach (EventPackage ep in EventsToSendOnLookAt)
        {
            if ((ep.scope == eventScope.instigator) || (ep.scope == eventScope.visualScriptingInstigator))
                EventRegistry.SendEvent(ep, GameManager.player.gameObject);
            else
                EventRegistry.SendEvent(ep, this.gameObject);
        }
            
        lookTimer = 0f;
    }

    private void triggerLookAway()
    {        
        foreach (EventPackage ep in EventsToSendOnLookAway)
        {
            if ((ep.scope == eventScope.instigator) || (ep.scope == eventScope.visualScriptingInstigator))
                EventRegistry.SendEvent(ep, GameManager.player.gameObject);
            else
                EventRegistry.SendEvent(ep, this.gameObject);
        }            
        lookTimer = 0f;
    }

}

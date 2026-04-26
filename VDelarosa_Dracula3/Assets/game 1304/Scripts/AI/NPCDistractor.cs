using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(SphereCollider))]
public class NPCDistractor : MonoBehaviour
{
    
    [Tooltip("The minimum amount of time for this to be functional again after an NPC is distracted by it")]
    public float minCoolDown = 20f;
    [Tooltip("The maximum amount of time for this to be functional again after an NPC is distracted by it")]
    public float maxCoolDown = 60f;
    public bool resetCooldownOnEnableDisable = true;
    private float currentCooldown = 0f;
    private float newCooldown;
    public float attractionRadius = 10f;
    [Tooltip("If true, there will have to be an unimpeded line between this and the NPC for it to trigger")]
    public bool requireLineOfSight = false;
    private bool isCurrentlyDistracting = false;
    private NPCBehavior currentlyDistractedNPC;
    public Transform NPCPlacementTarget;
    public Transform NPCLookTarget;
    private Quaternion facingRotation;
    public float minDuration = 1f;
    public float maxDuration = 3f;
    [HideInInspector]
    public float currentDuration;
    private float newDuration;
    public bool startEnabled = true;
    private bool isEnabled;
    //add allowed factions list
    //add allowed alertness states
    [Tooltip("While distracted at this distractor, the NPCs sight range and acuity will be multiplied by this factor")]
    public float visionMultiplier = 0.25f;
    //public float hearingMultiplier = 0.25f;


    [Header("Event Listening")]
    public List<string> eventsToEnableThis;
    public List<string> eventsToDisableThis;
    public List<string> eventsToToggleThis;

    [Header("Event Sending")]
    public List<string> eventsToSendOnEnteringDistraction;
    public List<string> eventsToSendOnExitingDistraction;

    //public List<Transform> lookTargets;
    [Header("Debug")]
    public bool hideDebugDraw = false;
    public bool onlyShowDebugWhenSelected = false;
    [Tooltip("The NPC will get within this distance of the distractor's target marker before stopping. ")]
    public float minDistance = 0f;

    void Start()
    {
        Vector3 tempVec;
        isEnabled = startEnabled;
        GetComponent<SphereCollider>().radius = attractionRadius;
        GetComponent<SphereCollider>().isTrigger = true;
        foreach (string s in eventsToDisableThis)
            EventRegistry.AddEvent(s, disableOnEvent, gameObject);
        foreach (string s in eventsToEnableThis)
            EventRegistry.AddEvent(s, enableOnEvent, gameObject);
        foreach (string s in eventsToToggleThis)
            EventRegistry.AddEvent(s, toggleOnEvent, gameObject);
        if (NPCLookTarget != null && NPCPlacementTarget != null)
        {
            tempVec = NPCLookTarget.transform.position - NPCPlacementTarget.transform.position;
            tempVec.y = 0;
            facingRotation = Quaternion.LookRotation(tempVec, Vector3.up);
        }
        else
            facingRotation = Quaternion.identity;
    }

    void enableOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        isEnabled = true;
        if (resetCooldownOnEnableDisable)
            resetCooldown();
        else
        {
            newCooldown = 0;
            currentCooldown = newCooldown;
        }

    }

    void disableOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        isEnabled = false;
    }

    void toggleOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        if (isEnabled)
            disableOnEvent(eventName, obj);
        else
            enableOnEvent(eventName,obj);
    }

    void resetCooldown()
    {        
        newCooldown = UnityEngine.Random.Range(minCoolDown, maxCoolDown);
        currentCooldown = newCooldown;
    }

    private void OnTriggerStay(Collider other)
    {
        NPCBehavior eb;
        if (other.isTrigger)
            return;

        if (isEnabled && currentCooldown <= 0 && !isCurrentlyDistracting)
        {
            eb = other.gameObject.GetComponent<NPCBehavior>();
            if (eb != null)
            {
                if (eb.canBeDistracted())
                {
                    eb.startDistraction(this);
                    currentlyDistractedNPC = eb;
                    isCurrentlyDistracting = true;
                    newDuration = UnityEngine.Random.Range(minDuration, maxDuration);
                    currentDuration = newDuration;
                }
            }
        }
    }

    public void finishDistraction()
    {
        isCurrentlyDistracting = false;
        resetCooldown();
    }

    void Update()
    {
        if (!isCurrentlyDistracting)
        {
            if (currentCooldown > 0)
                currentCooldown -= Time.deltaTime;
            if (currentCooldown < 0)
                currentCooldown = 0;
        }
    }

    private void OnDrawGizmos()
    {
        if (!hideDebugDraw && !onlyShowDebugWhenSelected)
            drawGizmos();
    }

    public Quaternion getFacing()
    {
        return facingRotation;
    }
    public void drawGizmos()
    {
#if UNITY_EDITOR
        if (!isEnabled)
            return;
        //TODO: Add better NULL checks for all navpoints
        if(NPCPlacementTarget != null)
        {
            Handles.color = new Color(1f, 1f, 0.25f, 1f);
            Handles.DrawWireDisc(NPCPlacementTarget.transform.position, Vector3.up, minDistance);
            //Handles.DrawSolidDisc(NPCTargetLocation.transform.position, Vector3.up, 0.5f);
            if (NPCLookTarget != null)
            {
                Handles.color = new Color(0f, 1f, 0.5f, 0.5f);                
                Handles.ArrowHandleCap(0, NPCPlacementTarget.transform.position , facingRotation, 0.25f, EventType.Repaint);
            }
        }

        //Handles.color = new Color(0.125f, 1f, 0.125f, 1.0f);
        //Handles.draw .SphereCap(0, transform.position, Quaternion.identity, attractionRadius);
        Gizmos.color = Color.Lerp(Color.green, Color.red, Mathf.InverseLerp(0, newCooldown, currentCooldown));
        //Gizmos.color = new Color(0.125f, 1f, 0.125f, 1.0f);
        Gizmos.DrawWireSphere(transform.position, attractionRadius/2f);

        //TODO: add gizmo data for current state of distractor        
#endif
    }
}

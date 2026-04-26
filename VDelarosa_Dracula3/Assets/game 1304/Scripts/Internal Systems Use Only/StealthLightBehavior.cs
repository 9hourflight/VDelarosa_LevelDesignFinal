using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class StealthLightBehavior : MonoBehaviour
{
    private bool playerIsInLightRadius = false;
    private SphereCollider sphereCollider;
    private Light light;
    private LayerMask playerMask;
    private float playerDistance;
    private float _visibilityContribution;
    private GameObject currentBlocker;
    public float visibilityContribution
    {
        get { return _visibilityContribution; }
    }
    void Start()
    {
        Init();
    }

    public void Init()
    {
        playerMask = LayerMask.GetMask("PlayerTrigger");
        light = GetComponent<Light>();
        //add trigger if it doesn't exist
        //set radius of trigger
        if(!TryGetComponent<SphereCollider>(out sphereCollider))
        {
            sphereCollider = gameObject.AddComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            
        }
        sphereCollider.radius = light.range;
    }

    
    void Update()
    {
        _visibilityContribution = 0;
        RaycastHit hitInfo;
        if(playerIsInLightRadius)
        {
            //calculate player's visibility based on distance to light and raycasting
            //basic version - raycast to player
            //advanced version - multiple casts and average
            //current assumption is only one entity with a light receiver at a time (the player)
            //TODO: future consideration to allow multiple entities to be "lit" by a light
            if (Physics.Linecast(transform.position, GameManager.player.transform.position, out hitInfo,~0, QueryTriggerInteraction.Ignore)) //, playerMask))
            {
                if (hitInfo.collider != null)
                {
                    if (hitInfo.collider.transform.gameObject.GetComponent<StealthLightReceiver>())
                    {
                        playerDistance = Vector3.Distance(transform.position, GameManager.player.transform.position);
                        //TODO: add inverse square if this effect isn't "good enough"
                        _visibilityContribution = Mathf.Lerp(light.intensity, 0, Mathf.InverseLerp(0, light.range, playerDistance));
                    }
                    else
                    {
                        currentBlocker = hitInfo.collider.gameObject;
                        Debug.Log(gameObject.name + " - Light hit something not the player" + hitInfo.collider.gameObject.name);
                    }
                }
            }            
        }                    
    }
    public string getBlockerName()
    {
        if (currentBlocker != null)
            return (currentBlocker.name);
        else
            return "null";
    }
    private void OnTriggerEnter(Collider other)
    {
        
        StealthLightReceiver otherReceiver;
        if(other.TryGetComponent<StealthLightReceiver>(out otherReceiver))
        {
            playerIsInLightRadius = true;
            otherReceiver.AddLight(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        StealthLightReceiver otherReceiver;
        if (other.TryGetComponent<StealthLightReceiver>(out otherReceiver))
        {
            playerIsInLightRadius = false;
            otherReceiver.RemoveLight(this);
        }
    }
}

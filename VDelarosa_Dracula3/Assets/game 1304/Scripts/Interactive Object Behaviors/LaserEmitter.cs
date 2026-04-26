using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum laserInteractionModes { doNothing, turnOn, turnOff, toggle};

public class LaserEmitter : MonoBehaviour
{
    public GameObject laserTip;
    public float maxLength;
    public bool startOn = true;
    public bool updateRealtime = false;
    public GameObject laserObject;
    Vector3 laserOrigin;

    [Header("Event Listening")]       
    public string turnOnEvent;
    public string turnOffEvent;
    public string toggleEvent;

    private Collider laserCollision;
    private Renderer laserRenderer;
    private bool _isEnabled;
    private bool isEnabled
    {
        set
        {
            _isEnabled = value;
            if (value)
            {
                laserRenderer.enabled = true;
                laserCollision.enabled = true;                
            }
            else
            {
                laserRenderer.enabled = false;
                laserCollision.enabled = false;

            }
        }
        get
        {
            return _isEnabled;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        RaycastHit hitinfo;
        Vector3 destination;
        float laserLength;

        laserRenderer = laserObject.GetComponent<Renderer>();
        laserCollision = laserObject.GetComponent<Collider>();

        if(!updateRealtime)
        {
            laserOrigin = laserTip.transform.position;
            //if (Physics.Raycast (laserOrigin,  laserTip.transform.forward, out hitinfo, maxLength))
            if (Physics.Raycast(laserOrigin,laserTip.transform.forward, out hitinfo, maxLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                destination = hitinfo.point;
                laserLength = ((Vector3)(destination-laserOrigin)).magnitude;
                
            }
            else
            {
                destination = laserOrigin + Vector3.forward*maxLength;
                laserLength = maxLength;                
            }

            laserObject.transform.position = laserOrigin + ((destination - laserOrigin) / 2);
            laserObject.transform.localScale = new Vector3(0.125f, laserLength, 0.125f);
        }

        isEnabled = startOn;


        

        if (turnOnEvent != "")
        {
            EventRegistry.AddEvent(turnOnEvent, turnOnOnEvent, gameObject);
        }
        if (turnOffEvent != "")
        {
            EventRegistry.AddEvent(turnOnEvent, turnOffOnEvent, gameObject);
        }
        if (toggleEvent != "")
        {
            EventRegistry.AddEvent(turnOnEvent, toggleOnEvent, gameObject);
        }

    }

    public void processInteraction(laserInteractionModes mode)
    {
        switch (mode)
        {
            case laserInteractionModes.toggle:
                toggleOnEvent("", null);
                break;
            case laserInteractionModes.turnOn:
                turnOnOnEvent("",null);
                break;
            case laserInteractionModes.turnOff:
                turnOffOnEvent("", null);
                break;
        }
    }

    void turnOnOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        isEnabled = true;
    }

    void turnOffOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        isEnabled = false;
    }

    void toggleOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        isEnabled = !isEnabled;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hitinfo;
        Vector3 destination;
        float laserLength;
        if ((updateRealtime)&&(isEnabled))
        {
            laserOrigin = laserTip.transform.position;
            if (Physics.Raycast(laserOrigin, laserTip.transform.forward, out hitinfo, maxLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                destination = hitinfo.point;
                laserLength = ((Vector3)(destination - laserOrigin)).magnitude;
                laserObject.transform.position = laserOrigin + ((destination - laserOrigin) / 2);
                laserObject.transform.localScale = new Vector3(0.125f, laserLength, 0.125f);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LadderBehavior : MonoBehaviour
{
    public Transform topDismount;    
    public string interactLabel = "Ladder";
    public bool isEnabled = true;
    [Header("Event Listening")]
    public List<string> eventsToEnableThis;
    public List<string> eventsToDisableThis;
    new private Collider collider;
    private float _spineTop;

    private float _spineBottom;

    // Start is called before the first frame update
    void Start()
    {
        foreach (string s in eventsToDisableThis)
            EventRegistry.AddEvent(s, disableMe, gameObject);
        foreach (string s in eventsToEnableThis)
            EventRegistry.AddEvent(s, enableMe, gameObject);

        collider = GetComponent<Collider>();

        //calculate top and bottom of climbable "spine"
        _spineTop = collider.bounds.max.y;
        _spineBottom = collider.bounds.min.y;
    }    

    void disableMe(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        enabled = false;
    }

    void enableMe(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        enabled = true;
    }

    public float getTop()
    {
        return _spineTop;
    }

    public float getBottom()
    {
        return _spineBottom;
    }

    public Vector3 getOffset()
    {
        return -transform.forward * GameManager.player.GetComponent<GAME1304PlayerController>().playerRadius * 1.25f + collider.bounds.center;
        //return new Vector2(-transform.forward)
    }
}

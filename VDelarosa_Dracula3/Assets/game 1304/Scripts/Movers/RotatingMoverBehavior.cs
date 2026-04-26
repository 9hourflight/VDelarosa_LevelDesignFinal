using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class RotatingMoverBehavior : MonoBehaviour
{
    public bool startOn = true;
    private bool _isActive;
    public Vector3 rotationOffset;
    //public Vector3 rotationA;
    private Quaternion _rotationA;
    //public Vector3 rotationB;
    private Quaternion _rotationB;
    [Tooltip("Velocity to move in units per second")]
    public float rotationDuration;
    [Tooltip("Amount of time in seconds to wait at Point A")]
    public float pauseDurationAtA;
    [Tooltip("Amount of time in seconds to wait at Point B")]
    public float pauseDurationAtB;
    [Tooltip("Set the time that the mover has already waited at A. Should not exceed A's wait time")]
    public float startTimeOffset;

    private moverState currentState;
    private moverState nextState;

    private float distanceToDestination;
    private Vector3 velAtoB;
    private Vector3 velBtoA;
    private Rigidbody rb;
    private float waitTime;
    private float _currentWaitTime;
    private float lerpValue;


    [Header("Events")]
    public bool isEventDriven = false;
    public List<string> eventsToFireAtA;
    public List<string> eventsToFireLeavingA;
    public List<string> eventsToFireAtB;
    public List<string> eventsToFireLeavingB;
    public string pauseEvent;
    public string resumeEvent;
    public string toggleActiveEvent;
    public string goToAEvent;
    public string goToBEvent;


    void Start()
    {
        //_rotationA = Quaternion.Euler(rotationA);
        _rotationA = transform.rotation;
        _rotationB = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z) + rotationOffset);
        //transform.rotation = Quaternion.Euler(rotationA);	

        rb = gameObject.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.GetComponentInChildren<Rigidbody>();
        }
        rb.isKinematic = true;
        currentState = moverState.Waiting;
        nextState = moverState.MovingToB;
        waitTime = Time.time + pauseDurationAtA - startTimeOffset;
        _isActive = startOn;
        lerpValue = 0;

        //set up events
        EventRegistry.Init();
        if (pauseEvent != "")
        {
            EventRegistry.AddEvent(pauseEvent, pauseOnEvent, gameObject);
        }
        if (resumeEvent != "")
        {
            EventRegistry.AddEvent(resumeEvent, resumeOnEvent, gameObject);
        }
        if (toggleActiveEvent != "")
        {
            EventRegistry.AddEvent(toggleActiveEvent, toggleActiveOnEvent, gameObject);
        }
        if (goToAEvent != "")
        {
            EventRegistry.AddEvent(goToAEvent, goToAOnEvent, gameObject);
        }
        if (goToBEvent != "")
        {
            EventRegistry.AddEvent(goToBEvent, goToBOnEvent, gameObject);
        }
    }


    // Update is called once per frame
    void Update()
    {

    }

    void pauseOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        _isActive = false;
    }

    void resumeOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        _isActive = true;
    }

    void toggleActiveOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        _isActive = !_isActive;
    }

    void goToAOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        currentState = moverState.MovingToA;
        _isActive = true;
    }

    void goToBOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        currentState = moverState.MovingToB;
        _isActive = true;
    }

    public void goToA()
    {
        goToAOnEvent("",null);
    }

    public void goToB()
    {
        goToBOnEvent("",null);
    }

    public void goToNext()
    {
        if (currentState == moverState.Waiting)
        {
            currentState = nextState;
            _isActive = true;
        }
    }

    public void processInteractionInput(moverInteractionModes interactionMode)
    {
        switch (interactionMode)
        {
            case moverInteractionModes.doNothing:
                break;
            case moverInteractionModes.goToA:
                goToA();
                break;
            case moverInteractionModes.goToB:
                goToB();
                break;
            case moverInteractionModes.goToNextStop:
                goToNext();
                break;
            case moverInteractionModes.toggleOnOff:
                _isActive = !_isActive;
                break;
            case moverInteractionModes.turnOff:
                _isActive = false;
                break;
            case moverInteractionModes.turnOn:
                _isActive = true;
                break;
        }
    }
    void FixedUpdate()
    {        
        //Time.time
        if (!_isActive)
            return;
        switch (currentState)
        {
            case moverState.MovingToB:
                {
                    lerpValue += Time.deltaTime / rotationDuration;
                    distanceToDestination = Quaternion.Angle(rb.transform.rotation, _rotationB);
                    if (distanceToDestination <= 1.0f)
                    {
                        //version A
                        //transform.rotation = _rotationB;
                        
                        //version B
                        rb.MoveRotation(_rotationB);
                        
                        /*for (int i = 0; i < rb.transform.childCount; i++)
                        {
                            //  (rb.transform.GetChild(i)).transform.rotation =  _rotationB;

                        }*/
                        waitTime = pauseDurationAtB;
                        _currentWaitTime = 0;
                        /*if (waitTime == 0)
                        {
                            currentState = moverState.MovingToA;
                            nextState = moverState.Waiting;
                        }
                        else
                        {
                            currentState = moverState.Waiting;
                            nextState = moverState.MovingToA;
                        }*/

                        currentState = moverState.Waiting;
                        nextState = moverState.MovingToA;

                        lerpValue = 0;
                        if (eventsToFireAtB.Count > 0)
                        {
                            foreach (string s in eventsToFireAtB)
                            {
                                EventRegistry.SendEvent(s);
                            }
                        }
                    }
                    else
                    {
                        //version A
                        //transform.rotation = Quaternion.Lerp(_rotationA, _rotationB, lerpValue);

                        //version B
                        rb.MoveRotation(Quaternion.Lerp(_rotationA, _rotationB, lerpValue));
                        for (int i = 0; i < rb.transform.childCount; i++)
                        {
                            // (rb.transform.GetChild(i)).transform.rotation = Quaternion.Lerp(_rotationA, _rotationB, lerpValue);

                        }                        
                    }
                }
                break;
            case moverState.Waiting:
                {
                    rb.velocity = Vector3.zero;
                    if (_isActive && !isEventDriven)
                    {

                        _currentWaitTime += Time.deltaTime;

                        if (_currentWaitTime >= waitTime)
                        {
                            currentState = nextState;
                            lerpValue = 0;

                            if (currentState == moverState.MovingToA)
                            {
                                if (eventsToFireLeavingB.Count > 0)
                                {
                                    foreach (string s in eventsToFireLeavingB)
                                    {
                                        EventRegistry.SendEvent(s);
                                    }
                                }
                            }
                            if (currentState == moverState.MovingToB)
                            {
                                if (eventsToFireLeavingA.Count > 0)
                                {
                                    foreach (string s in eventsToFireLeavingA)
                                    {
                                        EventRegistry.SendEvent(s);
                                    }
                                }
                            }
                        }

                    }
                }
                break;
            case moverState.MovingToA:
                {
                    lerpValue += Time.deltaTime / rotationDuration;
                    distanceToDestination = Quaternion.Angle(rb.transform.rotation, _rotationA);
                    if (distanceToDestination <= 1.0f)
                    {
                        //version A
                        //transform.rotation = _rotationA;
                        
                        //version B
                        rb.MoveRotation(_rotationA);


                        /*for (int i = 0; i < rb.transform.childCount; i++)
                        {
                            // (rb.transform.GetChild(i)).transform.rotation = _rotationA;
                        }*/
                        currentState = moverState.Waiting;
                        nextState = moverState.MovingToB;
                        _currentWaitTime = 0;

                        
                        waitTime = pauseDurationAtA;
                        lerpValue = 0;
                        if (eventsToFireAtA.Count > 0)
                        {
                            foreach (string s in eventsToFireAtA)
                            {
                                EventRegistry.SendEvent(s);
                            }
                        }
                    }
                    else
                    {
                        //version A
                        //transform.rotation = Quaternion.Lerp(_rotationB, _rotationA, lerpValue);

                        //version B
                        rb.MoveRotation(Quaternion.Lerp(_rotationB, _rotationA, lerpValue));


                        for (int i = 0; i < rb.transform.childCount; i++)
                        {
                            // (rb.transform.GetChild(i)).transform.rotation = Quaternion.Lerp(_rotationB, _rotationA, lerpValue);
                        }
                        //rb.velocity = velAtoB * moveSpeed;
                    }
                }
                break;

        }

    }
}

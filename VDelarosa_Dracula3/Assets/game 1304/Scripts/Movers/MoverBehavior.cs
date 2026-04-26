using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
public enum moverState {MovingToB,Waiting,MovingToA};

[RequireComponent(typeof(Rigidbody))]
[Serializable]
public class MoverBehavior : MonoBehaviour
{

    [Header("Basic Settings")]
    public bool startOn = true;
	private bool _isActive;
    [HideInInspector]
	public Vector3 positionA;
    [HideInInspector]
    public Vector3 positionB;
    public Vector3 movementOffset;
	[Tooltip("Velocity to move in units per second from A to B")]
	public float travelDurationAtoB = 1;
    public AnimationCurve AtoBCurve;
    [Tooltip("Velocity to move in units per second from B to A")]
    public float travelDurationBtoA = 1;
    public AnimationCurve BtoACurve;
    [Tooltip("Amount of time in seconds to wait at Point A")]
	public float pauseDurationAtA = 1;
	[Tooltip("Amount of time in seconds to wait at Point B")]
	public float pauseDurationAtB = 1;
	[Tooltip("Set the time that the mover has already waited at A. Should not exceed A's wait time")]
	public float startTimeOffset = 0;
    /*[Tooltip("If true, this mover will pop back to position A after reaching B and waiting for its pause duration.")]
    public bool isLooping = false;*/

   // [Header("Sounds")]
    //public AudioClip StartSound;
    public AudioClip StopSound;
    public AudioClip MovingSound;
    //public AudioClip IdlingSound;
    private AudioSource _audioSource;


    private moverState currentState;
	private moverState nextState;

	private float distanceToDestination;
	private Vector3 velAtoB;
	private Vector3 velBtoA;
	private Rigidbody rb;
	private float waitTime;
	private float _currentWaitTime;
	private float lerpValue;
    private bool eventControlled = false;

	[Header("Event Sending")]
    public List<string> eventsToSendAtA;
    public List<string> eventsToSendLeavingA;
    public List<string> eventsToSendAtB;
    public List<string> eventsToSendLeavingB;

    [Header("Event Listening")]
    [Tooltip("If TRUE, this mover will only move when events affect it. Otherwise events will still affect it, but it will resume normal movement after each event.")]    
    public string pauseEvent;
	public string resumeEvent;
	public string toggleActiveEvent;
	public string goToAEvent;
	public string goToBEvent;
    public string resetToAEvent;
    public string resetToBEvent;
    public string toggleLocationEvent;


    void Start () 
	{        
        _audioSource = GetComponent<AudioSource>();

        positionA = transform.position;
        positionB = transform.position + movementOffset;
		//transform.position = positionA;	
		velAtoB = Vector3.Normalize(positionB - positionA);// * moveSpeed;
		velBtoA = Vector3.Normalize(positionA - positionB);// * moveSpeed;
		rb = gameObject.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.GetComponentInChildren<Rigidbody>();
        }
        rb.isKinematic = true;
        //rb.AddForce(velAtoB);
        //rb.MovePosition(rb.position+velAtoB);
        rb.velocity = velAtoB;
		currentState = moverState.Waiting;
		nextState = moverState.MovingToB;
        _isActive = startOn;
        lerpValue = 0;
        if (startTimeOffset > 0)
        {
            //Set the offset position for the mover
            startTimeOffset = startTimeOffset % (pauseDurationAtA + pauseDurationAtB + (travelDurationAtoB + travelDurationBtoA));
            if (startTimeOffset <= pauseDurationAtA)
                waitTime = Time.time + pauseDurationAtA - startTimeOffset;

            if ((startTimeOffset > pauseDurationAtA) && (startTimeOffset <= (pauseDurationAtA + travelDurationAtoB)))
            {
                waitTime = 0;
                lerpValue = (startTimeOffset - pauseDurationAtA) / travelDurationAtoB;
                currentState = moverState.MovingToB;
                nextState = moverState.Waiting;
            }
            if ((startTimeOffset > (pauseDurationAtA + travelDurationAtoB)) && (startTimeOffset <= (pauseDurationAtA + travelDurationAtoB + pauseDurationAtB)))
            {
                waitTime = startTimeOffset - pauseDurationAtA + travelDurationAtoB;
                currentState = moverState.Waiting;
                nextState = moverState.MovingToA;
            }
            if (startTimeOffset > (pauseDurationAtA + travelDurationAtoB + pauseDurationAtB))
            {
                waitTime = 0;
                lerpValue = (startTimeOffset - (pauseDurationAtA + travelDurationAtoB + pauseDurationAtB)) / travelDurationBtoA;
                currentState = moverState.MovingToA;
                nextState = moverState.Waiting;
            }
        }
        //set up events
        //EventRegistry.Init();
		if(pauseEvent != "")
		{
			EventRegistry.AddEvent(pauseEvent, pauseOnEvent, gameObject);
		}
		if(resumeEvent != "")
		{
			EventRegistry.AddEvent(resumeEvent, resumeOnEvent, gameObject);
		}
		if(toggleActiveEvent != "")
		{
			EventRegistry.AddEvent(toggleActiveEvent, toggleActiveOnEvent, gameObject);
		}
		if(goToAEvent != "")
		{
			EventRegistry.AddEvent(goToAEvent, goToAOnEvent, gameObject);
		}
		if(goToBEvent != "")
		{
			EventRegistry.AddEvent(goToBEvent, goToBOnEvent, gameObject);
		}
        if (resetToAEvent != "")
        {
            EventRegistry.AddEvent(resetToAEvent, ResetToAOnEvent, gameObject);
        }
        if (resetToBEvent != "")
        {
            EventRegistry.AddEvent(resetToBEvent, ResetToBOnEvent, gameObject);
        }
        if (toggleLocationEvent != "")
        {
            EventRegistry.AddEvent(toggleLocationEvent, toggleLocationOnEvent, gameObject);
        }
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
        eventControlled = false;
    }

	void toggleActiveOnEvent(string eventName, GameObject obj)
	{
        if ((obj != null) && (obj != this.gameObject))
            return;
        _isActive = !_isActive;
        eventControlled = false;
    }

    void toggleLocationOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        switch (currentState)
        {
            case moverState.MovingToA:
                currentState = moverState.MovingToB;
                lerpValue = 1 - lerpValue;
                break;
            case moverState.MovingToB:
                currentState = moverState.MovingToA;
                lerpValue = 1 - lerpValue;
                break;
            case moverState.Waiting:
                if (nextState == moverState.MovingToB)
                {
                    currentState = moverState.MovingToB;
                    _currentWaitTime = 0;
                }
                if (nextState == moverState.MovingToA)
                {
                    currentState = moverState.MovingToA;
                    _currentWaitTime = 0;
                }
                break;
        }
        eventControlled = true;
        _isActive = true;
    }

    public void processInteractionInput(moverInteractionModes mode)
    {
        switch (mode)                    
        {
            case moverInteractionModes.goToA:
                goToAOnEvent("",null);
                break;
            case moverInteractionModes.goToB:
                goToBOnEvent("",null);
                break;
            case moverInteractionModes.resetToA:
                ResetToAOnEvent("", null);
                break;
            case moverInteractionModes.resetToB:
                ResetToBOnEvent("", null);
                break;
            case moverInteractionModes.goToNextStop:
                toggleLocationOnEvent("",null);
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
    
    void goToAOnEvent(string eventName, GameObject obj)
	{
        if ((obj != null) && (obj != this.gameObject))
            return;

        GoToA();
	}

    public void GoToA()
    {
        eventControlled = true;
        if (currentState == moverState.MovingToB)
            lerpValue = 1 - lerpValue;
        currentState = moverState.MovingToA;
        _isActive = true;
    }

    void ResetToAOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;

        ResetToA();
    }

    public void ResetToA()
    {
        eventControlled = true;
        transform.position = positionA;
        currentState = moverState.MovingToB;
        _isActive = true;
        lerpValue = 0;
        _currentWaitTime = 0;
    }

    void goToBOnEvent(string eventName, GameObject obj)
	{
        if ((obj != null) && (obj != this.gameObject))
            return;
        GoToB();
	}

   public void GoToB()
    {
        eventControlled = true;
        if (currentState == moverState.MovingToA)
            lerpValue = 1 - lerpValue;
        currentState = moverState.MovingToB;
        _isActive = true;
    }

    void ResetToBOnEvent(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;

        ResetToB();
    }

    public void ResetToB()
    {
        eventControlled = true;
        transform.position = positionB;
        currentState = moverState.MovingToA;
        _isActive = true;
        lerpValue = 0;
        _currentWaitTime = 0;
    }
    void FixedUpdate()
	{
		if(!_isActive)
			return;
		//Time.time
		//TODO: use a LERP instead of whatever the hell this stuff is
		switch (currentState)
		{
			case moverState.MovingToB:
			{
                if (travelDurationAtoB > 0)
                {
                    lerpValue += Time.fixedDeltaTime / travelDurationAtoB;
                    distanceToDestination = Vector3.Distance(transform.position, positionB);
                }
                else
                {
                    transform.position = positionB;
                        //rb.MovePosition(positionB);
                    distanceToDestination = 0f;
                }
				if(distanceToDestination <= Time.fixedDeltaTime / travelDurationAtoB)
				{
                    if ((_audioSource != null) && (StopSound != null))
                    {
                        _audioSource.loop = false;
                        _audioSource.clip = StopSound;
                        _audioSource.Play();
                    }
                    if (eventsToSendAtB.Count > 0)
                    {
                        foreach (string s in eventsToSendAtB)
                        {
                            EventRegistry.SendEvent(s);
                        }
                    }
                    lerpValue = 0;
                    _currentWaitTime = 0;
                    if (eventControlled)
                    {
                        _isActive = false;
                        currentState = moverState.Waiting;
                        nextState = moverState.MovingToA;
                    }
                    else
                    {
                        currentState = moverState.Waiting;
                        nextState = moverState.MovingToA;
                        waitTime = pauseDurationAtB;
                    }
					
				}
				else
				{                    
                    if(AtoBCurve.length > 0)
                        rb.MovePosition(Vector3.Lerp(positionA, positionB, AtoBCurve.Evaluate(lerpValue)));
                    else
                        rb.MovePosition(Vector3.Lerp(positionA, positionB, lerpValue));
                        
                }
			}
			break;
			case moverState.Waiting:
			{
				//rb.velocity = Vector3.zero;

				if(_isActive)
				{

					_currentWaitTime += Time.fixedDeltaTime;
					if(_currentWaitTime >= waitTime)
					{
						currentState = nextState;
                        if ((_audioSource != null)&&(MovingSound != null))
                            {
                                _audioSource.loop = true;
                                _audioSource.clip = MovingSound;
                                _audioSource.Play();

                            }
						if(currentState == moverState.MovingToA)
						{                           
							if(eventsToSendLeavingB.Count > 0)
							{
								foreach(string s in eventsToSendLeavingB)
								{
									EventRegistry.SendEvent(s);
								}
							}
						}
						if(currentState == moverState.MovingToB)
						{
							if(eventsToSendLeavingA.Count > 0)
							{
								foreach(string s in eventsToSendLeavingA)
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
                if (travelDurationBtoA > 0)
                {
                    lerpValue += Time.fixedDeltaTime / travelDurationBtoA;
                    distanceToDestination = Vector3.Distance(transform.position, positionA);
                }
                else
                {
                    transform.position = positionA;
                    //    rb.MovePosition(positionA);
                    distanceToDestination = 0f;
                }                    
				if(distanceToDestination <= Time.fixedDeltaTime / travelDurationBtoA)
				{
                    if ((_audioSource != null) && (StopSound != null))
                    {
                        _audioSource.loop = false;
                        _audioSource.clip = StopSound;
                        _audioSource.Play();
                    }
                        if (eventControlled)
                        {
                            _isActive = false;
                            currentState = moverState.Waiting;
                            nextState = moverState.MovingToB;
                        }
                        else
                        {
                            currentState = moverState.Waiting;
                            nextState = moverState.MovingToB;
                            waitTime = pauseDurationAtA;
                        }
                        
					if(eventsToSendAtA.Count > 0)
					{
						foreach(string s in eventsToSendAtA)
						{
							EventRegistry.SendEvent(s);
						}
					}
					lerpValue = 0;
					_currentWaitTime = 0;
				}
				else
				{					
                    if(BtoACurve.length > 0)
                        rb.MovePosition(Vector3.Lerp(positionB, positionA, BtoACurve.Evaluate(lerpValue)));
                    else
                        rb.MovePosition(Vector3.Lerp(positionB, positionA, lerpValue));                        
                }
			}
			break;

		}						

	}


    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        List<Vector3> boundingBoxVerts;
        Collider col;
        Handles.color = Color.blue;
        Handles.DrawDottedLine(transform.position, transform.position + movementOffset, 2.0f);

        col = GetComponent<Collider>();

        if (col == null)
            return;
        boundingBoxVerts = new List<Vector3>();
        boundingBoxVerts.Add(new Vector3(col.bounds.min.x, col.bounds.max.y, col.bounds.min.z)  + movementOffset);
        boundingBoxVerts.Add(new Vector3(col.bounds.max.x, col.bounds.max.y, col.bounds.min.z)  + movementOffset);
        boundingBoxVerts.Add(new Vector3(col.bounds.min.x, col.bounds.max.y, col.bounds.max.z)  + movementOffset);
        boundingBoxVerts.Add(new Vector3(col.bounds.max.x, col.bounds.max.y, col.bounds.max.z)  + movementOffset);
        boundingBoxVerts.Add(new Vector3(col.bounds.min.x, col.bounds.min.y, col.bounds.min.z)  + movementOffset);
        boundingBoxVerts.Add(new Vector3(col.bounds.max.x, col.bounds.min.y, col.bounds.min.z)  + movementOffset);
        boundingBoxVerts.Add(new Vector3(col.bounds.min.x, col.bounds.min.y, col.bounds.max.z)  + movementOffset);
        boundingBoxVerts.Add(new Vector3(col.bounds.max.x, col.bounds.min.y, col.bounds.max.z)  + movementOffset);

        
        
        if (col != null)
        {
            //top
            Handles.DrawLine(boundingBoxVerts[0], boundingBoxVerts[1]);
            Handles.DrawLine(boundingBoxVerts[1], boundingBoxVerts[3]);
            Handles.DrawLine(boundingBoxVerts[3], boundingBoxVerts[2]);
            Handles.DrawLine(boundingBoxVerts[2], boundingBoxVerts[0]);

            //bottom
            Handles.DrawLine(boundingBoxVerts[4], boundingBoxVerts[5]);
            Handles.DrawLine(boundingBoxVerts[5], boundingBoxVerts[7]);
            Handles.DrawLine(boundingBoxVerts[7], boundingBoxVerts[6]);
            Handles.DrawLine(boundingBoxVerts[6], boundingBoxVerts[4]);

            //sides
            Handles.DrawLine(boundingBoxVerts[0], boundingBoxVerts[4]);
            Handles.DrawLine(boundingBoxVerts[2], boundingBoxVerts[6]);
            Handles.DrawLine(boundingBoxVerts[3], boundingBoxVerts[7]);
            Handles.DrawLine(boundingBoxVerts[1], boundingBoxVerts[5]);
        }
        //navContainerObject.drawPatrolGizmos();
#endif
    }
}

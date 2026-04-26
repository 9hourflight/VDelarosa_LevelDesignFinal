using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FlyingLocomotionBehavior : MonoBehaviour
{
    private float movementSpeed;
    private Vector3 destination;
    private bool hasDestination = false;
    private bool isStopped = false;
    private Vector3 headingVector;
    private Rigidbody rb;
    private float distanceThreshold = 0.5f;
	// Use this for initialization
	void Start ()
    {
        rb = GetComponent<Rigidbody>();
	}
	
    public void setIsStopped(bool stopped)
    {
        isStopped = stopped;
    }

    public void setDestination(Vector3 newdestination)
    {
        destination = newdestination;
        hasDestination = true;
    }

    public void setSpeed(float speed)
    {
        movementSpeed = speed;
    }

    public float getRemainingDistance()
    {
        if (hasDestination)
            return (Vector3.Distance(transform.position, destination));
        else
            return 0;
    }
	
    public bool getPathPending()
    {
        return hasDestination;
    }

/*	void Update ()
    {
		if((!isStopped)&&(hasDestination))
        {

        }
	}*/

    void FixedUpdate()
    {
        if ((isStopped)||(!hasDestination))
            return;

        headingVector = Vector3.Normalize(destination - transform.position);
        rb.velocity = headingVector * movementSpeed; // (headingVector * (movementSpeed * Time.deltaTime));
        //rb.MovePosition(transform.position + (headingVector * (movementSpeed * Time.deltaTime)));
        rb.rotation = Quaternion.LookRotation(headingVector);
        if (getRemainingDistance() < distanceThreshold)
        {
            rb.velocity = Vector3.zero;
            hasDestination = false;
        }
    }
}

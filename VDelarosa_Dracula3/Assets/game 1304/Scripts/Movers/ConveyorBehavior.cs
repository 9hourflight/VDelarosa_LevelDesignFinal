using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ConveyorBehavior : MonoBehaviour
{
    public float speed = 5f;
    public bool startOn = true;
    Rigidbody rb;
    Renderer r;    
    BoxCollider col;
    float length;
    bool isRunning;
    bool isReversed;
    float actualSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        r = GetComponent<Renderer>();
        col = GetComponent<BoxCollider>();
        length = col.size.z * transform.lossyScale.z;
        r.material.SetTextureScale("_MainTex", new Vector2(1, length / 2.5f));
        isRunning = startOn;
        actualSpeed = speed;
    }

    public void processsInteraction(conveyorInteractionModes interactionMode)
    {
        switch (interactionMode)
        {
            case conveyorInteractionModes.turnOn:
                isRunning = true;
                break;
            case conveyorInteractionModes.turnOff:
                isRunning = false;
                break;
            case conveyorInteractionModes.toggleOnOff:
                isRunning = !isRunning;
                break;
            case conveyorInteractionModes.reverse:
                reverseDir();
                break;
        }
    }
    public void reverseDir()
    {
        isReversed = !isReversed;
        if (isReversed)
        {
            r.material.SetTextureScale("_MainTex", new Vector2(1, -length / 2.5f));
            actualSpeed = -speed;
        }
        else
        {
            r.material.SetTextureScale("_MainTex", new Vector2(1, length / 2.5f));
            actualSpeed = speed;
        }

    }

    private void Update()
    {
        Vector2 offset = new Vector2(0,Time.time * actualSpeed/(length / r.material.GetTextureScale("_MainTex").y));        
        if (isRunning)
        {
            if (r == null)
                return;
            r.material.SetTextureOffset(Shader.PropertyToID("_MainTex"), offset);
        }

    }

    void FixedUpdate()
    {
         if (isRunning)
         {
             Vector3 pos = rb.transform.position;
             
             rb.position -= transform.forward * actualSpeed * Time.fixedDeltaTime;
             
             rb.MovePosition(pos);
         }
    }

    public float getActualSpeed()
    {
        if (isRunning)
            return actualSpeed;
        else
            return 0;
    }    
}

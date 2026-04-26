using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ConveyorTriggerVolumeBehavior : MonoBehaviour
{

    public bool onlyTriggerOnPlayer = true;
    int contentsCount = 0;
    List<GameObject> contents;
    [FormerlySerializedAs("treadmills")]
    public List<ConveyorBehavior> conveyors;
    [Header("On Enter Behavior")]
    public conveyorInteractionModes interactionModeEnter;
    [Header("On Exit Behavior")]
    public conveyorInteractionModes interactionModeExit;

    private void Start()
    {
        contents = new List<GameObject>();
    }
    void OnTriggerEnter(Collider other)
    {

        if ((other.gameObject.GetComponent<GAME1304PlayerController>() != null) || (!onlyTriggerOnPlayer))
        {
            contentsCount++;
            contents.Add(other.gameObject);
            if (interactionModeExit != conveyorInteractionModes.doNothing)
                InvokeRepeating("CheckContents", 0, 0.25f);
            foreach (ConveyorBehavior tb in conveyors)
            {
                if (tb != null)
                {

                    tb.processsInteraction(interactionModeEnter);
                }
            }
        }        
    }

    private void CheckContents()
    {
        for (int i = contents.Count - 1; i >= 0; i--)
        {
            if (contents[i] == null)
                contents.RemoveAt(i);
        }
        if (contents.Count == 0)
            TriggerEmptied();
    }

    private void OnTriggerExit(Collider other)
    {
        DoorknobBehavior dkb;
        if ((other.gameObject.GetComponent<GAME1304PlayerController>() != null) || (!onlyTriggerOnPlayer))
        {
            contentsCount--;
            contents.Remove(other.gameObject);

            if (contentsCount <= 0)
            {
                TriggerEmptied();

            }
        }
    }

    private void TriggerEmptied()
    {
        CancelInvoke();        
        foreach (ConveyorBehavior tb in conveyors)
        {
            if (tb != null)
            {
                tb.processsInteraction(interactionModeExit);
            }
        }
    }
    
}

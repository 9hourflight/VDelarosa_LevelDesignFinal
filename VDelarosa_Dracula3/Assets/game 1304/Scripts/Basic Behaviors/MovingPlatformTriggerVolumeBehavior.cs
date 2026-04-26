using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MovingPlatformTriggerVolumeBehavior : MonoBehaviour
{
    public bool onlyTriggerOnPlayer = true;
    int contentsCount = 0;    
    List<GameObject> contents;
    public List<GameObject> movingPlatforms;
    [Header("On Enter Behavior")]
    [FormerlySerializedAs("interactionMode")]
    public moverInteractionModes interactionModeOnEnter;
    [Header("On Exit Behavior")]
    public moverInteractionModes interactionModeOnExit;

    private void Start()
    {
        contents = new List<GameObject>();
    }

    void OnTriggerEnter(Collider other)
    {

        if ((other.gameObject.GetComponent<GAME1304PlayerController>() != null)||(!onlyTriggerOnPlayer))
        {
            contentsCount++;
            contents.Add(other.gameObject);
            if (interactionModeOnExit != moverInteractionModes.doNothing)
                InvokeRepeating("CheckContents", 0, 0.25f);
            foreach (GameObject mp in movingPlatforms)
            {
                if (mp != null)
                {
                    MoverBehavior mb = mp.GetComponent<MoverBehavior>();
                    if (mb != null)
                    {
                        mb.processInteractionInput(interactionModeOnEnter);
                    }
                }
            }
        }
        //base.OnTriggerEnter();
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
        if ((other.gameObject.GetComponent<GAME1304PlayerController>() != null) || (!onlyTriggerOnPlayer))
        {
            contentsCount--;            
            contents.Remove(other.gameObject);

            if (contentsCount <= 0)
            {
                TriggerEmptied();

            }                            
        }
        //base.OnTriggerEnter();
    }

    private void TriggerEmptied()
    {
        CancelInvoke();
        foreach (GameObject mp in movingPlatforms)
        {
            if (mp != null)
            {
                MoverBehavior mb = mp.GetComponent<MoverBehavior>();
                if (mb != null)
                {
                    mb.processInteractionInput(interactionModeOnExit);
                }
            }
        }
    }

}

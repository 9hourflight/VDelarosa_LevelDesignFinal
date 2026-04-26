using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterTriggerVolumeBehavior : MonoBehaviour
{

    public bool onlyTriggerOnPlayer = true;
    int contentsCount = 0;
    public List<TeleporterVolume> teleporterVolumes;
    
    [Header("On Enter Behavior")]
    public teleporterInteractionModes interactionModeEnter;
    [Header("On Exit Behavior")]
    public teleporterInteractionModes interactionModeExit;
    
    void OnTriggerEnter(Collider other)
    {

        if ((other.gameObject.GetComponent<GAME1304PlayerController>() != null) || (!onlyTriggerOnPlayer))
        {
            contentsCount++;
            foreach (TeleporterVolume tv in teleporterVolumes)
            {
                if (tv != null)
                {
                    tv.ProcessInteraction(interactionModeEnter);
                }
            }
        }        
    }

    private void OnTriggerExit(Collider other)
    {
        if ((other.gameObject.GetComponent<GAME1304PlayerController>() != null) || (!onlyTriggerOnPlayer))
        {
            contentsCount--;
            if (contentsCount <= 0)
            {
                foreach (TeleporterVolume tv in teleporterVolumes)
                {
                    if (tv != null)
                    {
                        tv.ProcessInteraction(interactionModeExit);
                    }
                }
            }
        }
    }
}

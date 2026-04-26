using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EventListener_TeleportToPoint : MonoBehaviour
{
    public List<string> eventsToListenFor;
    [Tooltip("Leave empty if you want the event scope to dictate the object to teleport.")]
    public GameObject gameObjectToTeleport;
    public GameObject gameObjectDestination;
    public float randomRadius = 0f;
    public bool constrainRandomToHorizontal = true;
    public bool matchFacingOfDestinationObject = true;

    void Start()
    {
        if (eventsToListenFor.Count > 0)
        {
            foreach (string s in eventsToListenFor)
            {
                if (s != "")
                    EventRegistry.AddEvent(s, teleport, gameObject);
            }
        }
    }

    
    void teleport(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != gameObject))
            return;
        if ((gameObjectToTeleport == null)||(gameObjectDestination==null))
            return;

        Vector3 destinationPosition;
        Vector3 destinationOffset;
        Vector2 horizontalOffset;
        NPCBehavior npcb;
        destinationOffset = Vector3.zero;

        if (randomRadius > 0)
        {
            if (constrainRandomToHorizontal)
            {
                horizontalOffset = Random.insideUnitCircle * randomRadius;
                destinationOffset = new Vector3(horizontalOffset.x, 0, horizontalOffset.y);
            }
            else
            {
                destinationOffset = Random.insideUnitSphere * randomRadius;
            }
        }
        destinationPosition = gameObjectDestination.transform.position + destinationOffset;

        if (gameObjectToTeleport.TryGetComponent<NPCBehavior>(out npcb)) // .GetComponent<>() != null)
        {
            npcb.TeleportToTransform(gameObjectDestination.transform, destinationOffset, true, matchFacingOfDestinationObject);
            //npcb.TeleportToLocation(destinationPosition, true);            
        }
        else
        {
            gameObjectToTeleport.transform.position = destinationPosition;
            if (matchFacingOfDestinationObject)
                gameObjectToTeleport.transform.rotation = gameObjectDestination.transform.rotation;
        }
        
	}
}

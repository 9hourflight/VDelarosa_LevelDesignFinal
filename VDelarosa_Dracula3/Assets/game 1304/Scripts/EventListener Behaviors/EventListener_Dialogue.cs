using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventListener_Dialogue : MonoBehaviour 
{
	public bool startEnabled = true;
	bool isEnabled;
	public List<string> eventsToEnableThis;
	public List<string> eventsToDisableThis;
	public string eventToListenFor;
	public DialogueScriptableObject dialogue;
	//public DialogueContainer dialogue;
	public List<CharacterProfile> speakingCharacters;

	void Start () 
	{
		if(eventToListenFor != "")
		{
			EventRegistry.AddEvent(eventToListenFor, startDialogue, gameObject);
		}
		foreach (string s in eventsToEnableThis)
        {
			EventRegistry.AddEvent(s, enableDialogue, gameObject);
		}
		foreach (string s in eventsToDisableThis)
        {
			EventRegistry.AddEvent(s, disableDialogue, gameObject);
		}
		if (dialogue != null)
		{
			dialogue.containingObject = gameObject;
			dialogue.SetHostObject(gameObject);
		}
		if (dialogue != null)
		{
			if(dialogue.speakingCharacters != null)
				dialogue.speakingCharacters.Clear();
			//TODO: throw exception here

			foreach (CharacterProfile npccp in speakingCharacters)
			{
				dialogue.AddSpeakingCharacter(npccp);
			}
		}
		isEnabled = startEnabled;
	}

	void enableDialogue(string eventName, GameObject obj)
	{
		if ((obj != null) && (obj != this.gameObject))
			return;
		isEnabled = true;
	}

	void disableDialogue(string eventName, GameObject obj)
	{
		if ((obj != null) && (obj != this.gameObject))
			return;
		isEnabled = false;
	}

	void startDialogue (string eventName, GameObject obj) 
	{
		if (!isEnabled)
			return;
        if ((obj != null) && (obj != this.gameObject))
            return;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            return;
        ObjectInteractionBehavior oib = player.GetComponent<ObjectInteractionBehavior>();
        if (oib == null)
            return;
		this.dialogue.SetHostObject(gameObject);
        oib.startDialogue(this.dialogue);        
    }
}
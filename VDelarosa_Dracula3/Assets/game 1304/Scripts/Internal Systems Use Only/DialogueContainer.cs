using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class dialogueLineCouplet
{
    [Tooltip("This is the short line that the player says in the list of replies.")]
    public string replyLine;
    //public Color replyColor = Color.white;
    //public DialogueEntry replyEntry;
    [Tooltip("If you fill in an address here, it'll point to the line in the dialogue tree with that address instead of using a brand new entry. Use this to add loops to your tree.")]
    public string replyAddress;
    [Tooltip("A token to check in order for this line to appear. Leave the token name empty and it'll always show up.")]
    public tokenCondition condition;
    public bool thisReplyEndsTheDialogue = false;
}

[System.Serializable]
public class characterReply
{
    public string replyLine;
    //public Color replyColor = Color.white;
    public AudioClip soundClip;
    [Tooltip("A token to check in order for this line to appear. Leave the token name empty and it'll always show up.")]
    public tokenCondition condition;
    [Tooltip("Events that will fire when this line is displayed.")]
    public List<EventPackage> eventsToSend;

}

[System.Serializable]
public class DialogueEntry
{
    public string entryAddress;
    public string entryPlayerLine;
    public Color textColor = Color.white;
    public List<characterReply> possibleCharacterReplies;
    public AudioClip soundClip;
    public List<dialogueLineCouplet> playerReplies;
    public List<EventPackage> eventsToSend;

        
    public int getAdjustedReplyCount()
    {
        int count = 0;
        for (int x = 0; x <playerReplies.Count;x++)
        {
            if (playerReplies[x].condition.tokenName == "")
                count++;
            else
            {
                if (TokenRegistry.testToken(playerReplies[x].condition))
                    count++;
            }
        }
        return count;
    }

}

[System.Serializable]
public class DialogueContainer 
{

	public bool canExit;
    public string dialogueTitle;
    public string playerName = "You";
    public string characterName = "Them";
    public List<EventPackage> eventsToSendOnExit;
    
    public Color titleFontColor = Color.white;    
    public Color backgroundColor = Color.white;
    public bool isFullScreen = false;
    [HideInInspector]
    public GameObject containingObject;
    [Tooltip("The first dialogue entry the player will see.")]
    public DialogueEntry initialDialogueEntry;
    [Tooltip("All of the other dialogue entries, referenced by their address.")]
    public List<DialogueEntry> dialogueEntries;



    public DialogueEntry getEntryByAddress(string searchAddress)
    {
        if (initialDialogueEntry.entryAddress == searchAddress)
            return initialDialogueEntry;

        foreach (DialogueEntry de in dialogueEntries)
        {
            if (de.entryAddress == searchAddress) //there is an entry to drill into and it's not trying to simply reference another branch of the tree by address reference (which could lead to infinite recursion, stack overflow, etc)
            {
                return de;
            }                                
        }
        return null;
    }

 
}

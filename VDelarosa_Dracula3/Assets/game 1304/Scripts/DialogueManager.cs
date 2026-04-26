using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    Canvas thisCanvas;
    public RectTransform parentRect;
    GUIStyle dialogueStyle;
    public Text speaker1Text;
    public Text speaker1Name;
    public Text playerName;
    public Text playerText;
    public GameObject replyButtonPrefab;
    public GameObject nameplatePrefab;
    public GameObject bodyTextPrefab;

    List<GameObject> DialogueObjects;
    List<GameObject> ReplyButtons;
    GameObject exitButton;
    List<DialogueNode_Reply> replies;
    string playerReplyFromPrevious = "";
    string sPlayerName = "Player";
    string nextNodeName = "";

    public static string activeEntryAddress;
    public void Init()
    {
        activeEntryAddress = "";
        nextNodeName = "";
        ReplyButtons = new List<GameObject>();
        DialogueObjects = new List<GameObject>();
        thisCanvas = gameObject.GetComponent<Canvas>();
        dialogueStyle = new GUIStyle();
        sPlayerName = GameManager.player.GetComponent<CharacterProfile>().name;
        if (HUDManager.currentDialogue.isBlocking)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
        
        parentRect.GetComponent<Image>().enabled = HUDManager.currentDialogue.isBlocking;              

        replies = new List<DialogueNode_Reply>();
        if (HUDManager.currentDialogue.GetAllNodes().Count() > 0)
            DisplayDialogueNode(HUDManager.currentDialogue.GetAllNodes().ElementAtOrDefault(0));
        
    }

    private void DisplayDialogueNode(string nodeName)
    {
        DisplayDialogueNode(HUDManager.currentDialogue.GetEntryByAddress(nodeName));
    }
    private void DisplayDialogueNode(DialogueNodeBase node)
    {
        GameObject tempObj;
        Text tempText;
        Button tempButton;
        bool noChildEntry = true;
        activeEntryAddress = "";
        string SubtitleString = "";
        float nonBlockingDelay = 0;
        string CurrentNPCName = "";
        CharacterProfile currentNPCProfile = null;
        //branch based on if this is a player node vs an NPC node
        //NPC node, show NPC lines. 
        //if previous entry is player reply, show full reply text
        //if child entry is player entry, show player reply options as buttons
        //if child entry is NPC entry, show next button that leads to displaying that node next
        //if no child entries, show exit button if it doesn't exist
        //future - if child entry is Random Entry, select from the random and act accordingly

        //player reply node can just display the buttons so the Display(playernode) can just be appended to a Display(NPCNode) and it all concatenates

        if (node == null)
        {
            //HUDManager.notificationQueue.Enqueue("display node null");
            return;
        }
        /*else
            HUDManager.notificationQueue.Enqueue("display node worked");*/
        //we're displaying a list of NPC lines
        if (node as DialogueNode_NPCEntry != null)
        {
            activeEntryAddress = node.entryAddress;
            CurrentNPCName = "Replying Character :";
            if ((node as DialogueNode_NPCEntry).indexOfSpeakingCharacter<HUDManager.currentDialogue.speakingCharacters.Count)
            {
                if (HUDManager.currentDialogue.speakingCharacters[(node as DialogueNode_NPCEntry).indexOfSpeakingCharacter] != null)
                {
                    currentNPCProfile = HUDManager.currentDialogue.speakingCharacters[(node as DialogueNode_NPCEntry).indexOfSpeakingCharacter];
                    CurrentNPCName = currentNPCProfile.characterName + ":";
                }
            }

            //if it's a blocking dialogue, show the player's full reply from the previous player entry selection
            //otherwise, select the NPC that's talking and have them start moving their mouth
            if (HUDManager.currentDialogue.isBlocking)
            {
                if (playerReplyFromPrevious != "")
                {
                    tempObj = Instantiate(nameplatePrefab, parentRect);
                    DialogueObjects.Add(tempObj);
                    tempObj.GetComponent<Text>().text = sPlayerName + ":";
                    tempObj = Instantiate(bodyTextPrefab, parentRect);
                    DialogueObjects.Add(tempObj);
                    tempObj.GetComponent<Text>().text = playerReplyFromPrevious;
                }

                //show the character's name
                tempObj = Instantiate(nameplatePrefab, parentRect);
                DialogueObjects.Add(tempObj);

                tempObj.GetComponent<Text>().text = CurrentNPCName; // "Replying Character :";


            }
            else
                HighlightTalkingNPC(currentNPCProfile);

            //if it's blocking, list out all of the NPC lines in the HUD
            //otherwise queue up the lines in the subtitle manager
            //and (TODO) play the audio
            if (HUDManager.currentDialogue.isBlocking)
            {
                //show the character's lines
                tempObj = Instantiate(bodyTextPrefab, parentRect);
                DialogueObjects.Add(tempObj);
                //grab the lines from the children. We don't start from the current node anymore
                tempObj.GetComponent<Text>().text = "";
                foreach (string s in node.GetChildren())
                {
                    tempObj.GetComponent<Text>().text += GetTextFromDialogueLines(HUDManager.currentDialogue.GetEntryByAddress(s));
                }
                //HUDManager.notificationQueue.Enqueue(tempObj.GetComponent<Text>().text);

            }
            else
            {
                nonBlockingDelay = 0;
                foreach (string s in node.GetChildren())
                {
                    SubtitleString = GetTextFromDialogueLines(HUDManager.currentDialogue.GetEntryByAddress(s));
                    if (SubtitleString != "")
                    {
                        if((HUDManager.currentDialogue.GetEntryByAddress(s) as DialogueNode_Line)!=null)
                        {
                            if((HUDManager.currentDialogue.GetEntryByAddress(s) as DialogueNode_Line).audioLine != null)
                            {

                            }
                        }
                        nonBlockingDelay += SubtitleManager.DisplaySubtitle(CurrentNPCName + SubtitleString, 0);
                    }
                }
            }
            //from here check the child. if it's a player reply show the node
            //if it's an NPC reply, show NEXT with a button that shows that node
            //if it's neither, then show the exit button if one isn't already there

            foreach (string s in node.GetChildren())
            {                
                //if this is blocking and the child node is another NPC entry
                //we just have a Next button so the player can proceed
                if (HUDManager.currentDialogue.GetEntryByAddress(s) as DialogueNode_NPCEntry)
                {
                    noChildEntry = false;
                    if (HUDManager.currentDialogue.isBlocking)
                    {
                        //display next button
                        //next button clears the current choice menu 
                        //then displays the next NPC entry
                        tempObj = Instantiate(replyButtonPrefab, parentRect);
                        tempText = tempObj.GetComponentInChildren<Text>();
                        tempButton = tempObj.GetComponentInChildren<Button>();
                        if (tempText != null)
                        {
                            tempText.text = "Next";
                        }
                        if (tempButton != null)
                        {
                            tempButton.onClick.AddListener(delegate { ShowNextNPCEntry(HUDManager.currentDialogue.GetEntryByAddress(s) as DialogueNode_NPCEntry); });
                        }
                        DialogueObjects.Add(tempObj);
                        
                    }
                    else
                    {
                        //if we're not blocking, proceed to the next NPC node
                        //TODO: find a better solution so we don't blow the call stack if these things loop indefinitely  
                        nextNodeName = s;
                        Invoke("DisplayNextNode", nonBlockingDelay);
                        //DisplayDialogueNode(HUDManager.currentDialogue.GetEntryByAddress(s));
                    }
                    
                }
                
                if (HUDManager.currentDialogue.GetEntryByAddress(s) as DialogueNode_PlayerEntry)
                {
                    //TODO: find a better solution so we don't blow the call stack if these things loop indefinitely
                    //DisplayDialogueNode(HUDManager.currentDialogue.GetEntryByAddress(s));
                    nextNodeName = s;
                    Invoke("DisplayNextNode", 0);
                    noChildEntry = false;
                }
            }
            if(noChildEntry && !HUDManager.currentDialogue.isBlocking)
            {
                Invoke("ExitDialogue", nonBlockingDelay);
            }
            if ((((HUDManager.currentDialogue.canExit)&& noChildEntry) || (noChildEntry)) && (exitButton == null) && (HUDManager.currentDialogue.isBlocking))
            {
                MakeExitButton();

            }

        }

        //we're displaying a list of player replies
        if (node as DialogueNode_PlayerEntry != null)
        {
            activeEntryAddress = node.entryAddress;
            if (HUDManager.currentDialogue.isBlocking)
            {
                //show the player name for replies
                tempObj = Instantiate(nameplatePrefab, parentRect);
                DialogueObjects.Add(tempObj);
                tempObj.GetComponent<Text>().text = sPlayerName + ":";

                replies = GetReplies(node).ToList();

                int buttonIndex = 0;
                foreach (DialogueNode_Reply reply in replies)
                {
                    if (reply != null)
                    {
                        noChildEntry = false;
                        tempObj = Instantiate(replyButtonPrefab, parentRect);
                        tempText = tempObj.GetComponentInChildren<Text>();
                        tempButton = tempObj.GetComponentInChildren<Button>();
                        if (tempText != null)
                        {
                            tempText.text = reply.reply;
                        }
                        if (tempButton != null)
                        {
                            int i = buttonIndex;
                            tempButton.onClick.AddListener(delegate { ReplyButtonClicked(i); });
                        }
                        ReplyButtons.Add(tempObj);

                    }
                    buttonIndex++;
                }
            
                //TODO:Move this out of the player entry section since you might have a dead-end NPC entry
                if (((HUDManager.currentDialogue.canExit) || (noChildEntry)) && (exitButton == null))
                {
                    MakeExitButton();
                    
                }
            }
        }

        //do these at the end so token settings don't screw with lines that display based on those tokens
        ProcessEvents(node);
        SetTokens(node);
    }

    private void MakeExitButton()
    {
        GameObject tempObj;
        Button tempButton;
        Text tempText;

        tempObj = Instantiate(replyButtonPrefab, parentRect);
        tempText = tempObj.GetComponentInChildren<Text>();
        tempButton = tempObj.GetComponentInChildren<Button>();
        if (tempText != null)
        {
            tempText.text = "(F) Exit";
        }
        if (tempButton != null)
        {
            tempButton.onClick.AddListener(ExitDialogue);
        }
        exitButton = tempObj;
    }
    private void HighlightTalkingNPC(CharacterProfile currentCP)
    {
        NPCBehavior npcb;

        if (HUDManager.currentDialogue.speakingCharacters == null)
            return;
        //iterate through all characters connected to this dialogue and set their talking mode to "off" or whatever
        foreach (CharacterProfile cp in HUDManager.currentDialogue.speakingCharacters)
        {
            if (cp != null)
            {
                if (currentCP == cp)
                {
                    if (cp.gameObject.TryGetComponent<NPCBehavior>(out npcb))
                        npcb.SetMouthState(NPCMouthState.talking);
                }
                else
                {
                    if (cp.gameObject.TryGetComponent<NPCBehavior>(out npcb))
                        npcb.SetMouthState(NPCMouthState.neutral);
                }
            }
        }


    }

    private void DisplayNextNode()
    {
        DisplayDialogueNode(nextNodeName);
    }

    private IEnumerable<DialogueNode_Reply> GetReplies(DialogueNodeBase node)
    {
        if (node == null)
            yield return null;
        if (node as DialogueNode_Reply != null)
            yield return (DialogueNode_Reply)node;
        if ((node as DialogueNode_Line != null) || (node as DialogueNode_NPCEntry))
        {
            yield return null;
        }
        if (node as DialogueNode_Logic != null)
        {
            if (EvaluateLogicNode(node as DialogueNode_Logic))
            {
                foreach (string nodeName in node.GetChildren())
                {
                    foreach (DialogueNode_Reply dnr in GetReplies(HUDManager.currentDialogue.GetEntryByAddress(nodeName)))
                    {
                        if (dnr != null)
                            yield return dnr;
                    }
                }
            }
            else
                yield return null;
        }
        if  (node as DialogueNode_PlayerEntry != null)
        {
            foreach (string nodeName in node.GetChildren())
            {
                foreach (DialogueNode_Reply dnr in GetReplies(HUDManager.currentDialogue.GetEntryByAddress(nodeName)))
                {
                    if (dnr != null)
                        yield return dnr;
                }
            }
        }
        yield return null;
    }

    private string GetTextFromDialogueLines(DialogueNodeBase node)
    {
        string outString = "";
        if (node == null)
            return "";
        if (node as DialogueNode_Reply != null)
            return "";
        if (node as DialogueNode_Line != null)
        {
            ProcessEvents(node);
            SetTokens(node);
            return ((DialogueNode_Line)node).lineText + "\n";
        }
        if (node as DialogueNode_Logic != null)
        {
            if (!EvaluateLogicNode(node as DialogueNode_Logic))
                return "";
        }
        if (node as DialogueNode_Logic != null)
        {

            foreach (string nodeName in node.GetChildren())
            {

                outString += GetTextFromDialogueLines(HUDManager.currentDialogue.GetEntryByAddress(nodeName));
            }
        }
        return outString;
    }

    private bool EvaluateLogicNode(DialogueNode_Logic logicNode)
    {
        if (logicNode as DialogueNode_LogicToken != null)
            return (EvaluateLogicNode_Token(logicNode as DialogueNode_LogicToken));

        if (logicNode as DialogueNode_LogicQuest != null)
            return (EvaluateLogicNode_Quest(logicNode as DialogueNode_LogicQuest));

        if (logicNode as DialogueNode_LogicInventory != null)
            return (EvaluateLogicNode_Inventory(logicNode as DialogueNode_LogicInventory));

        return true;
    }

    private bool EvaluateLogicNode_Token(DialogueNode_LogicToken tokenLogicNode)
    {
        switch (tokenLogicNode.comparison)
        {
            case comparisonOperator.Equal:
                return TokenRegistry.getToken(tokenLogicNode.tokenName) == tokenLogicNode.valueToCompare;
            case comparisonOperator.greaterThan:
                return TokenRegistry.getToken(tokenLogicNode.tokenName) > tokenLogicNode.valueToCompare;
            case comparisonOperator.greaterThanEqual:
                return TokenRegistry.getToken(tokenLogicNode.tokenName) >= tokenLogicNode.valueToCompare;
            case comparisonOperator.lessThan:
                return TokenRegistry.getToken(tokenLogicNode.tokenName) < tokenLogicNode.valueToCompare;
            case comparisonOperator.lessThanEqual:
                return TokenRegistry.getToken(tokenLogicNode.tokenName) <= tokenLogicNode.valueToCompare;
            case comparisonOperator.notEqual:
                return TokenRegistry.getToken(tokenLogicNode.tokenName) != tokenLogicNode.valueToCompare;
        }
        return false;
    }

    private bool EvaluateLogicNode_Inventory(DialogueNode_LogicInventory inventoryLogicNode)
    {
        int count;
        if (inventoryLogicNode.inventoryItem != null)
        {
            count = ((GameManager.player.GetComponent<ObjectInteractionBehavior>()).inventory.GetItemCount(inventoryLogicNode.inventoryItem));
        }
        else
        {
            count = ((GameManager.player.GetComponent<ObjectInteractionBehavior>()).inventory.GetItemCount(inventoryLogicNode.inventoryItemName));
        }

        switch (inventoryLogicNode.comparison)
        {
            case comparisonOperator.Equal:
                return count == inventoryLogicNode.quantity;
            case comparisonOperator.greaterThan:
                return count > inventoryLogicNode.quantity;
            case comparisonOperator.greaterThanEqual:
                return count >= inventoryLogicNode.quantity;
            case comparisonOperator.lessThan:
                return count < inventoryLogicNode.quantity;
            case comparisonOperator.lessThanEqual:
                return count <= inventoryLogicNode.quantity;
            case comparisonOperator.notEqual:
                return count != inventoryLogicNode.quantity;
        }
        return false;
    }

    private bool EvaluateLogicNode_Quest(DialogueNode_LogicQuest questLogicNode)
    {
        return true;
    }

    void ProcessEvents(DialogueNodeBase node)
    {
        if (node as DialogueNode_Line != null)
        {
            foreach (EventPackage ep in ((DialogueNode_Line)node).eventsToSend)
                EventRegistry.SendEvent(ep, HUDManager.currentDialogue.hostObject);
        }
        if (node as DialogueNode_Reply != null)
        {
            foreach (EventPackage ep in ((DialogueNode_Reply)node).eventsToSend)
                EventRegistry.SendEvent(ep, HUDManager.currentDialogue.hostObject);
        }
        if (node as DialogueNode_NPCEntry != null)
        {
            foreach (EventPackage ep in ((DialogueNode_NPCEntry)node).eventsToSend)
                EventRegistry.SendEvent(ep, HUDManager.currentDialogue.hostObject);
        }
    }

    private void SetTokens(DialogueNodeBase node)
    {
        if (node as DialogueNode_NPCEntry != null)
        {
            foreach (TokenValuePair tvp in (node as DialogueNode_NPCEntry).tokensToSet)
            {
                TokenRegistry.setToken(tvp.tokenName, tvp.value);
            }
        }
        if (node as DialogueNode_Line != null)
        {
            foreach (TokenValuePair tvp in (node as DialogueNode_Line).tokensToSet)
            {
                TokenRegistry.setToken(tvp.tokenName, tvp.value);
            }
        }
        if (node as DialogueNode_Reply != null)
        {
            foreach (TokenValuePair tvp in (node as DialogueNode_Reply).tokensToSet)
            {
                TokenRegistry.setToken(tvp.tokenName, tvp.value);
            }
           /* foreach (EventPackage ep in (node as DialogueNode_Reply).eventsToSend)
            {
                EventRegistry.SendEvent(ep,null);
            }*/
        }
        if (node as DialogueNode_PlayerEntry != null)
        {
            foreach (TokenValuePair tvp in (node as DialogueNode_PlayerEntry).tokensToSet)
            {
                TokenRegistry.setToken(tvp.tokenName, tvp.value);
            }
        }

    }
    void ReplyButtonClicked(int replyIndex)
    {
        DialogueNode_Reply reply = replies[replyIndex];
        ProcessEvents(reply);
        SetTokens(reply);
        if (reply.exitsDialogue)
            ExitDialogue();
        else
        {
            if (reply.GetChildren().Count > 0)
            {
                ClearDialogue();
                //WARNING: assumption made here from other implementation that the child will be an Entry type node
                if (reply.useFullReply)
                    playerReplyFromPrevious = reply.fullReply;
                else
                    playerReplyFromPrevious = "";
                DisplayDialogueNode(HUDManager.currentDialogue.GetEntryByAddress(reply.GetChildren().ElementAt(0)) as DialogueNode_NPCEntry);
            }
            /*else
                DisplayDialogueNode(null);
            //TODO: before you investigate rewiring this, figure out what the hell you were thinking when you made it in the first place
            */
        }
    }

    void ShowNextNPCEntry(DialogueNode_NPCEntry node)
    {
        ClearDialogue();
        DisplayDialogueNode(node);
    }
    void ClearDialogue()
    {
        
        playerReplyFromPrevious = "";
        foreach (GameObject dO in DialogueObjects)
            Destroy(dO);
        foreach (GameObject button in ReplyButtons)
            Destroy(button);
        ReplyButtons.Clear();
        if (exitButton != null)
        {
            Destroy(exitButton);
            exitButton = null;
        }

    }
    void ExitDialogue()
    {
        activeEntryAddress = "";
        Invoke("SendEventsAndSetTokens", 0.1f);
        ClearDialogue();
        HighlightTalkingNPC(null);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        GameManager.unPause();
        thisCanvas.gameObject.SetActive(false);
        thisCanvas.enabled = true;
    }

    void SendEventsAndSetTokens()
    {
        foreach (EventPackage ep in HUDManager.currentDialogue.eventsToSendOnExit)
        {
            EventRegistry.SendEvent(ep, HUDManager.currentDialogue.hostObject);
        }
        foreach (TokenOperation to in HUDManager.currentDialogue.tokensToModifyOnExit)
        {
            TokenRegistry.modifyToken(to);
        }
    }
}

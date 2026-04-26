using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum DialogueEntryMode { playerEntry, NPCEntry, none};
public enum EntryComponentType { line, reply, none };

[CreateAssetMenu(fileName = "Unnamed Dialogue", menuName = "GAME 1304/Dialogue", order = 1)]
public class DialogueScriptableObject : ScriptableObject, ISerializationCallbackReceiver
{
    public bool canExit;
    public string dialogueTitle;
    /*public string playerName = "You";
    public string characterName = "Them";*/
    [SerializeField]
    public List<EventPackage> eventsToSendOnExit;
    [SerializeField]    
    public List<TokenOperation> tokensToModifyOnExit;
    public Color titleFontColor = Color.white;
    public Color backgroundColor = Color.white;
    public bool isFullScreen = false;
    [Tooltip("Blocking means that the player is locked in this conversation with an NPC.Non-blocking means it happens in the world.")]
    public bool isBlocking = true;
    [HideInInspector]
    public GameObject containingObject;
    
        
    [HideInInspector]
    [SerializeField]
    List<DialogueNodeBase> dialogueEntries = new List<DialogueNodeBase>();

    DialogueNodeBase root;
    
    //[SerializeField]
    Dictionary<string, DialogueNodeBase> nodeLookup = new Dictionary<string, DialogueNodeBase>();

    [SerializeField]
    [HideInInspector]
    GameObject _hostObject;

    [SerializeField]
    [HideInInspector]
    List<CharacterProfile> _speakingCharacters;

    [SerializeField]
    public List<CharacterProfile> speakingCharacters
    {
        get
        {
            return _speakingCharacters;
        }
    }
    public GameObject hostObject
    {
        get
        {
            return _hostObject;
        }
    }
    public void SetHostObject(GameObject newHost)
    {
        _hostObject = newHost;
    }


    public DialogueNodeBase GetEntryByAddress(string searchAddress)
    {
        if((nodeLookup == null)||(nodeLookup.Count == 0))
        {
            buildNodeLookup();
        }
        if (nodeLookup.ContainsKey(searchAddress))
            return nodeLookup[searchAddress];
        else return null;
    }

    public DialogueNodeBase GetRoot()
    {
        return root;
    }
#if UNITY_EDITOR
    private void Awake()
    {
        _speakingCharacters = new List<CharacterProfile>();
        if (dialogueEntries.Count == 0)
        {
            CreateChildNode(null,typeof(DialogueNode_NPCEntry));
        }
        root = dialogueEntries[0];
        OnValidate();
    }

    private void OnValidate()
    {
        buildNodeLookup();
        
    }
#endif
    void buildNodeLookup()
    {
        nodeLookup.Clear();
        foreach (DialogueNodeBase node in GetAllNodes())
        {
            nodeLookup[node.entryAddress] = node;
            //nodeLookup.Add(node.entryAddress, node);
        }
    }
    public IEnumerable<DialogueNodeBase> GetAllNodes()
    {
        return dialogueEntries;
    }

    public IEnumerable<DialogueNodeBase> GetAllChildren(DialogueNodeBase parentNode)
    {
        foreach(string s in parentNode.GetChildren())
        {
            if(GetEntryByAddress(s)!=null)
                yield return GetEntryByAddress(s);
        }
    }
#if UNITY_EDITOR
    public void CreateChildNode(DialogueNodeBase creatingNode,Type nodeType, Vector2 positionOffset = new Vector2()) //, EntryComponentType componentType = EntryComponentType.none)
    {
        DialogueNodeBase newNode = null;
        
        if (nodeType == typeof(DialogueNode_NPCEntry))
        {
            newNode = new DialogueNode_NPCEntry();
        }
        if (nodeType == typeof(DialogueNode_PlayerEntry))
        {
            newNode = new DialogueNode_PlayerEntry();
        }
        if (nodeType == typeof(DialogueNode_Line))
        {
            newNode = new DialogueNode_Line();
        }
        if (nodeType == typeof(DialogueNode_Reply))
        {
            newNode = new DialogueNode_Reply();
        }
        if (nodeType == typeof(DialogueNode_LogicToken))
        {
            newNode = new DialogueNode_LogicToken();
        }
        if (nodeType == typeof(DialogueNode_LogicInventory))
        {
            newNode = new DialogueNode_LogicInventory();
        }
        if (nodeType == typeof(DialogueNode_LogicQuest))
        {
            newNode = new DialogueNode_LogicQuest();
        }

        if (newNode == null)
            return;
            //DialogueNode_Entry newNode = new DialogueNode_Entry(); // new DialogueNodeBase();

        newNode.SetAddress(System.Guid.NewGuid().ToString());
        Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");
        if (creatingNode != null)
        {
            newNode.SetPosition(creatingNode.GUIRect.position + new Vector2(creatingNode.GUIRect.width * 2,creatingNode.GUIRect.yMin) + positionOffset);
            
            //if the new node is a logic node, we need to set its "mode" so we make sure the player can't back door add player lines to NPC
            //entries and vice versa by using logic nodes as a buffer
            if(newNode as DialogueNode_Logic != null)
            {
                if(creatingNode as DialogueNode_NPCEntry != null)
                {
                    (newNode as DialogueNode_Logic).SetParentEntryMode(DialogueEntryMode.NPCEntry);
                }
                if (creatingNode as DialogueNode_PlayerEntry != null)
                {
                    (newNode as DialogueNode_Logic).SetParentEntryMode(DialogueEntryMode.playerEntry);
                }
                if (creatingNode as DialogueNode_Logic != null)
                {
                    (newNode as DialogueNode_Logic).SetParentEntryMode((creatingNode as DialogueNode_Logic).parentEntryMode);
                }
                
            }
            newNode.SetParent(creatingNode);
            creatingNode.AddChild(newNode.entryAddress);
            
            //if we're dealing with a logic node that has no parent, give it a "parent mode type" that matches the line we just childed to it
            //that way linkage will respect the children down the chain
            //!!TODO:make this a loop and set all of the parents in a chain of logic nodes!!
            if (creatingNode as DialogueNode_Logic != null)
            {
                if ((creatingNode as DialogueNode_Logic).parentEntryMode == DialogueEntryMode.none)
                {
                    if (nodeType == typeof(DialogueNode_Line))
                    {
                        (creatingNode as DialogueNode_Logic).SetParentEntryMode(DialogueEntryMode.NPCEntry);
                    }
                    if (nodeType == typeof(DialogueNode_Reply))
                    {
                        (creatingNode as DialogueNode_Logic).SetParentEntryMode(DialogueEntryMode.playerEntry);
                    }
                }
            }
        }
        else
        {
            if (newNode as DialogueNode_Logic != null)
                (newNode as DialogueNode_Logic).SetParentEntryMode(DialogueEntryMode.none);
            newNode.SetPosition( positionOffset);
        }
        Undo.RecordObject(this, "Created dialogue node");
        dialogueEntries.Add(newNode);        
        OnValidate();
    }

    public void DeleteNode(DialogueNodeBase deletingNode)
    {
        Undo.RecordObject(this, "Deleted dialogue node");
        dialogueEntries.Remove(deletingNode);
        OnValidate();
        
        //TODO: insert custom logic per node type to remove this from 
        foreach (DialogueNodeBase node in dialogueEntries)
        {
            //if (node.GetChildren().Contains(deletingNode.entryAddress))
            //TODO: figure this inheritance puzzle out  
            //node.RemoveChild(deletingNode.entryAddress);
        }        
        Undo.DestroyObjectImmediate(deletingNode);
    }
#endif
    public void OnBeforeSerialize()
    {
#if UNITY_EDITOR
        if(AssetDatabase.GetAssetPath(this) != "")
        {
            foreach(DialogueNodeBase node in GetAllNodes())
            {
                if(AssetDatabase.GetAssetPath(node) == "")
                    AssetDatabase.AddObjectToAsset(node, this);
            }
        }
#endif
    }

    public void OnAfterDeserialize()
    {
        
    }

    public void AddSpeakingCharacter(CharacterProfile newCharacter)
    {
        _speakingCharacters.Add(newCharacter);
    }
}
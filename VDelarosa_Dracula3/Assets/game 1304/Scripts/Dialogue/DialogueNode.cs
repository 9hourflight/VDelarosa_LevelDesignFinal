using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class DialogueNode : ScriptableObject
{
    [SerializeField]
    string _entryAddress;
    
    [HideInInspector]
    public string entryAddress
    {
        get { return _entryAddress; }
    }
    public string entryPlayerLine;
    public Color textColor = Color.white;
    public List<characterReply> possibleCharacterReplies;
    public AudioClip soundClip;
    public List<dialogueLineCouplet> playerReplies;
    public List<EventPackage> eventsToSend;

    
    [SerializeField]
    Rect _GUIRect = new Rect(100,100,200,200);
    public Rect GUIRect
    {
        get
        {
            return _GUIRect;
        }
    }

    [SerializeField]
    private string _dialogueText;
    
    
    public string dialogueText
    {
        get
        {
            return _dialogueText;
        }        
    }
    
    [SerializeField]
    List<string> _children = new List<string>();
    
    public List<string> GetChildren()
    {        
            return _children;        
    }

    public DialogueNode()
    {
        
    }

#if UNITY_EDITOR
    public void SetPosition(Vector2 newPos)
    {
        Undo.RecordObject(this, "Update node position");
        _GUIRect.position = newPos;
        EditorUtility.SetDirty(this);
    }

    public void SetText(string newText)
    {
        if (_dialogueText != newText)
        {
            Undo.RecordObject(this, "Update dialogue text");
            _dialogueText = newText;
            EditorUtility.SetDirty(this);
        }
    }

    public void AddChild(string entryAddress)
    {
        Undo.RecordObject(this, "Adding child link");
        _children.Add(entryAddress);
        EditorUtility.SetDirty(this);
    }

    public void RemoveChild(string entryAddress)
    {
        Undo.RecordObject(this, "Removing child link");
        _children.Remove(entryAddress);
        EditorUtility.SetDirty(this);
    }

    internal void SetAddress(string v)
    {
        _entryAddress = v;
    }
#endif
}
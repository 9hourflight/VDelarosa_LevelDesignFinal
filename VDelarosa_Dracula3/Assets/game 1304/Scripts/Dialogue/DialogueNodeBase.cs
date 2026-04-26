using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class DialogueNodeBase : ScriptableObject
{
    [HideInInspector]
    [SerializeField]
    string _entryAddress;
    
    [HideInInspector]
    public string entryAddress
    {
        get { return _entryAddress; }
    }

    [HideInInspector]
    [SerializeField]
    protected Rect _GUIRect = new Rect(100,100,200,200);
    public Rect GUIRect
    {
        get
        {
            return _GUIRect;
        }
    }

#if UNITY_EDITOR
    [HideInInspector]
    [SerializeField]
    public GUIStyle nodeStyle = GUIStyle.none;
#endif

    [HideInInspector]
    [SerializeField]
    List<string> _children = new List<string>();
    
    public List<string> GetChildren()
    {        
            return _children;        
    }

    [HideInInspector]
    [SerializeField]
    DialogueNodeBase _parent = null;

    public DialogueNodeBase parent
    {
        get
        {
            return _parent;
        }
    }

    public void SetParent(DialogueNodeBase creatingNode)
    {
        _parent = creatingNode;
    }
    public void AddChild(string nodeAddress)
    {
#if UNITY_EDITOR
        Undo.RecordObject(this, "Adding child link");
        _children.Add(nodeAddress);
        EditorUtility.SetDirty(this);
#endif
    }

    public void RemoveChild(string nodeAddress)
    {
#if UNITY_EDITOR
        Undo.RecordObject(this, "Removing child link");
        _children.Remove(nodeAddress);
        EditorUtility.SetDirty(this);
#endif
    }

#if UNITY_EDITOR
    public void SetPosition(Vector2 newPos)
    {
#if UNITY_EDITOR
        Undo.RecordObject(this, "Update node position");
        _GUIRect.position = newPos;
        EditorUtility.SetDirty(this);
#endif
    }
    
    public void SetAddress(string v)
    {
        _entryAddress = v;
    }

    
#endif
}
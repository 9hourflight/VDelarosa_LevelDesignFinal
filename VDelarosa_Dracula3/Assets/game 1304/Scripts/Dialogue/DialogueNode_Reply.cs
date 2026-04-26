using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class DialogueNode_Reply : DialogueNodeBase
{
    [SerializeField]
    public List<EventPackage> eventsToSend;

    [SerializeField]
    public List<TokenValuePair> tokensToSet;
    [SerializeField]
    public List<TokenOperation> tokensToModify;
    [SerializeField]
    string _reply;

    [SerializeField]
    [TextArea(15, 20)]
    string _fullReply;

    public string fullReply
    {
        get
        {
            return _fullReply;
        }
    }

    [SerializeField]
    [Tooltip("If this is true, the dialogue will show the short reply as the option and then expand to the full reply after this reply has been selected.")]
    bool _useFullReply;
    public bool useFullReply
    {
        get { return _useFullReply; }
    }

    [SerializeField]
    bool _exitsDialogue;
    public bool exitsDialogue
    {
        get { return _exitsDialogue; }
    }


    public void setExitsDialogue(bool newED)
    {
#if UNITY_EDITOR
        if (_exitsDialogue != newED)
        {
            Undo.RecordObject(this, "Update dialogue text");
            _exitsDialogue = newED;
            EditorUtility.SetDirty(this);
        }
#endif
    }

    public void setFullReply(string newFP)
    {
#if UNITY_EDITOR
        if (_fullReply != newFP)
        {
            Undo.RecordObject(this, "Update dialogue text");
            _fullReply = newFP;
            EditorUtility.SetDirty(this);
        }
#endif
    }


    public void setUseFullReply(bool newUFR)
    {
#if UNITY_EDITOR
        if (_useFullReply != newUFR)
        {
            Undo.RecordObject(this, "Update dialogue text");
            _useFullReply = newUFR;
            EditorUtility.SetDirty(this);
        }
#endif
    }


    public string reply
    {
        get
        {
            return _reply;
        }
    }
    public void setReply(string newR)
    {
#if UNITY_EDITOR
        if (_reply != newR)
        {
            Undo.RecordObject(this, "Update dialogue text");
            _reply = newR;
            EditorUtility.SetDirty(this);
        }
#endif
    }
    public DialogueNode_Reply()
    {
        _GUIRect.width = 250;
        _GUIRect.height = 150;
    }
    private void OnEnable()
    {

#if UNITY_EDITOR
        nodeStyle = new GUIStyle();
        nodeStyle.normal.background = EditorGUIUtility.Load("Assets/game 1304/Editor/Resources/nodeBackBlue.png") as Texture2D;
        nodeStyle.normal.textColor = Color.white;
        nodeStyle.padding = new RectOffset(10, 10, 10, 10);
        nodeStyle.border = new RectOffset(12, 12, 12, 12);
#endif
    }

}
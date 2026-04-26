using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class DialogueNode_Line : DialogueNodeBase
{
    [SerializeField]
    [TextArea(15, 20)]
    string _lineText;    

    [SerializeField]
    public List<EventPackage> eventsToSend;

    [SerializeField]
    public List<TokenValuePair> tokensToSet;

    public AudioClip audioLine;
    public string lineText
    {
        get
        {
            return _lineText;
        }
    }
    
    public void setLineText(string newLT)
    {
#if UNITY_EDITOR
        if (_lineText != newLT)
        {
            Undo.RecordObject(this, "Update dialogue text");
            _lineText = newLT;
            EditorUtility.SetDirty(this);
        }
#endif
    }

    public DialogueNode_Line()
    {
        _GUIRect.width = 220;
        _GUIRect.height = 100;
    }
    private void OnEnable()
    {
#if UNITY_EDITOR
        nodeStyle = new GUIStyle();
        nodeStyle.normal.background = EditorGUIUtility.Load("Assets/game 1304/Editor/Resources/nodeBackGreen.png") as Texture2D;
        nodeStyle.normal.textColor = Color.white;
        nodeStyle.padding = new RectOffset(20, 20, 20, 20);
        nodeStyle.border = new RectOffset(12, 12, 12, 12);
#endif
    }
}
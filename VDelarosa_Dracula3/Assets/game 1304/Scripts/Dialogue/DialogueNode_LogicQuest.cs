using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class DialogueNode_LogicQuest : DialogueNode_Logic
{
    

    public DialogueNode_LogicQuest()
    {
        _GUIRect.width = 150;
        _GUIRect.height = 150;
    }
    private void OnEnable()
    {
#if UNITY_EDITOR
        nodeStyle = new GUIStyle();
        nodeStyle.normal.background = EditorGUIUtility.Load("Assets/game 1304/Editor/Resources/nodeBackOrange.png") as Texture2D;
        nodeStyle.normal.textColor = Color.white;
        nodeStyle.padding = new RectOffset(10, 10, 10, 10);
        nodeStyle.border = new RectOffset(12, 12, 12, 12);
#endif
    }
}
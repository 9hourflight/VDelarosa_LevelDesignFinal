using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class DialogueNode_LogicToken : DialogueNode_Logic
{
    [SerializeField]
    string _tokenName;

    

    public string tokenName
    {
        get
        {
            return _tokenName;
        }
    }
    public void setTokenName(string newTN)
    {
#if UNITY_EDITOR
        if (_tokenName != newTN)
        {
            Undo.RecordObject(this, "Update token name");
            _tokenName = newTN;
            EditorUtility.SetDirty(this);
        }
#endif
    }

    [SerializeField]
    comparisonOperator _comparison;

    public comparisonOperator comparison
    {
        get
        {
            return _comparison;
        }
    }
    [SerializeField]
    int _valueToCompare;

    public int valueToCompare
    {
        get 
        { 
            return _valueToCompare; 
        }
    }

    public void setValue(int newValue)
    {
#if UNITY_EDITOR
        if (_valueToCompare != newValue)
        {
            Undo.RecordObject(this, "Update token test value");
            _valueToCompare = newValue;
            EditorUtility.SetDirty(this);
        }
#endif
    }

    public void setComparison(comparisonOperator newCO)
    {
#if UNITY_EDITOR
        if (_comparison != newCO)
        {
            Undo.RecordObject(this, "Update token comparison");
            _comparison = newCO;
            EditorUtility.SetDirty(this);
        }
#endif
    }
    public DialogueNode_LogicToken()
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
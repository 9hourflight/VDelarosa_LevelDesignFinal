using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class DialogueNode_LogicInventory : DialogueNode_Logic
{
    [SerializeField]
    string _inventoryItemName;
    [SerializeField]
    comparisonOperator _comparison;
    [SerializeField]
    int _quantity;
    [SerializeField]
    InventoryDef _inventoryItem;
    public string inventoryItemName
    {
        get
        {
            return _inventoryItemName;
        }
    }

    public InventoryDef inventoryItem
    {
        get
        {
            return _inventoryItem;
        }
    }
    public comparisonOperator comparison
    {
        get
        {
            return _comparison;
        }
    }

    public int quantity
    {
        get
        {
            return _quantity;
        }
    }

    public void setValue(int newValue)
    {
#if UNITY_EDITOR
        if (_quantity != newValue)
        {
            Undo.RecordObject(this, "Update token test value");
            _quantity = newValue;
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

    public DialogueNode_LogicInventory()
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
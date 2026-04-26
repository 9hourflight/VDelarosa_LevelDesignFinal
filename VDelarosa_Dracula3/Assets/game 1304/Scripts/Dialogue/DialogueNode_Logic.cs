using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class DialogueNode_Logic : DialogueNodeBase
{
    protected DialogueEntryMode _parentEntryMode;
    public DialogueEntryMode parentEntryMode
    {
        get
        {
            return _parentEntryMode;
        }
    }

    public void SetParentEntryMode(DialogueEntryMode entryMode)
    {
        _parentEntryMode = entryMode;
        if (parent as DialogueNode_Logic != null)
            (parent as DialogueNode_Logic).SetParentEntryMode(entryMode);
    }

    
}
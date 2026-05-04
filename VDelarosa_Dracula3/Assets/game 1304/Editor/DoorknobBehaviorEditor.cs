
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DoorknobBehavior))]
public class DoorknobBehaviorEditor : Editor
{
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

    
    

    EditorGUILayout.PropertyField(serializedObject.FindProperty("doorRoot"));
    EditorGUILayout.PropertyField(serializedObject.FindProperty("keyNeeded"));
        
    EditorGUILayout.PropertyField(serializedObject.FindProperty("quantityRequired"));
    EditorGUILayout.PropertyField(serializedObject.FindProperty("consumeKeyOnUse"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("eventsToSend"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("eventToLock"));
    EditorGUILayout.PropertyField(serializedObject.FindProperty("eventToUnlock"));
    EditorGUILayout.PropertyField(serializedObject.FindProperty("eventToOpen"));
    EditorGUILayout.PropertyField(serializedObject.FindProperty("eventToClose"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("isLockedArtificially"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("canOpenWithScriptIfLocked"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("artificialLockedLabel"));
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("itemNeededOverrideLabel"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("ConsumeInventoryItem"));
        
        
        serializedObject.ApplyModifiedProperties();
        
    }
}

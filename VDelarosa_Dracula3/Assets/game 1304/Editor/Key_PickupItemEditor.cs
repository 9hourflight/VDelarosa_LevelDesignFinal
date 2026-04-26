
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Key_PickupItem))]
public class Key_PickupItemEditor : Editor
{
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();


        EditorGUILayout.PropertyField(serializedObject.FindProperty("keyName"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("itemAmount"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("pickUpOnCollision"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("EventsSentOnPickup"));
        /*EditorGUILayout.PropertyField(serializedObject.FindProperty("oneWay"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("teleporterFX"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("teleporterLight"));*/

        serializedObject.ApplyModifiedProperties();
        
    }
}

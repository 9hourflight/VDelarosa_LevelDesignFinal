
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TeleporterVolume))]
public class TeleporterVolumeEditor : Editor
{
    bool showAdvanced = false;
    bool showSounds;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();


        EditorGUILayout.PropertyField(serializedObject.FindProperty("destinationTeleporter"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("startEnabled"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("onlyTriggerOnPlayer"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("oneWay"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("teleporterFX"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("teleporterLight"));



        showAdvanced = EditorGUILayout.Foldout(showAdvanced, "Advanced Properties");
        if (showAdvanced)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("EventsToSendOnEnter"));   
            EditorGUILayout.PropertyField(serializedObject.FindProperty("EventsToSendOnExit"));        
            EditorGUILayout.PropertyField(serializedObject.FindProperty("eventToEnableThis"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("eventToDisableThis"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onlyTriggerOnTheseObjects"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ignoreTheseObjects"));
        }
        serializedObject.ApplyModifiedProperties();
        
    }
}

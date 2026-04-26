
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CheckpointBehavior))]
public class CheckpointBehaviorEditor : Editor
{
    bool showEvents = false;
    bool showAdvanced = false;
    bool showControlled = false;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("consumeOnUse"));

        showControlled = EditorGUILayout.Foldout(showControlled, "Controlled Objects");
        
        if (showControlled)
        {
            EditorGUILayout.LabelField("These things will be activated when a player respawns at this checkpoint.");
            //EditorGUILayout.TextArea("These things will be activated when a player respawns at this checkpoint.");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("lights"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("movingPlatforms"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("conveyors"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("teleporters"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("doors"));
        }
        //EditorGUILayout.EndFoldoutHeaderGroup();

        showEvents = EditorGUILayout.Foldout(showEvents, "Event Sending and Listening");
        if (showEvents)
        {
            EditorGUILayout.LabelField("Events sent by other objects that can trigger this checkpoint.");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("eventsToTriggerThis"));
            EditorGUILayout.LabelField("Events that this object will send when the player tags this checkpoint.");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("EventsToSendOnUse"));
            EditorGUILayout.LabelField("Events that this object will send when the player is respawned here.");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("EventsToSendOnRespawnHere"));
        }
        //EditorGUILayout.EndFoldoutHeaderGroup();

        showAdvanced = EditorGUILayout.Foldout(showAdvanced, "Advanced");
        if (showAdvanced)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dormantMaterial"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("activeMaterial"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("respawnTransform"));

        }
        //EditorGUILayout.EndFoldoutHeaderGroup();
        serializedObject.ApplyModifiedProperties();
        
    }
}

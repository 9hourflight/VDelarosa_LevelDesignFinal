
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MoverBehavior))]
public class MoverBehaviorEditor : Editor
{
    bool showAdvanced = false;
    bool showSounds;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("startOn"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("movementOffset"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("travelDurationAtoB"));        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("travelDurationBtoA"));        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("pauseDurationAtA"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("pauseDurationAtB"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("startTimeOffset"));
        showSounds = EditorGUILayout.BeginFoldoutHeaderGroup(showSounds, "Sounds");
        if (showSounds)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("StopSound"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("MovingSound"));
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        showAdvanced = EditorGUILayout.Foldout(showAdvanced, "Advanced Properties");
        if (showAdvanced)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("AtoBCurve"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("BtoACurve"));

            //[Header("Event Sending")]
            EditorGUILayout.PropertyField(serializedObject.FindProperty("eventsToSendAtA"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("eventsToSendLeavingA"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("eventsToSendAtB"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("eventsToSendLeavingB"));

            //[Header("Event Listening")]

            EditorGUILayout.PropertyField(serializedObject.FindProperty("pauseEvent"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("resumeEvent"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("toggleActiveEvent"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("goToAEvent"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("goToBEvent"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("toggleLocationEvent"));
        }
        serializedObject.ApplyModifiedProperties();
        
    }
}

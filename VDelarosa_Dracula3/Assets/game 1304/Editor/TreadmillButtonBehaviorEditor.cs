using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TreadmillButtonBehavior),true)]
public class TreadmillButtonBehaviorEditor : Editor
{
    [SerializeField] bool showAdvanced = false;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        //base.OnInspectorGUI();
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("treadmills"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("interactionMode"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("interactLabel"));
             

        showAdvanced = EditorGUILayout.Foldout(showAdvanced, "Advanced Properties");
        if (showAdvanced)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(InteractiveObject.interactVerb)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(InteractiveObject.UseOnce)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(InteractiveObject.startEnabled)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(InteractiveObject.eventsToSend)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(InteractiveObject.delay)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(InteractiveObject.activeMaterial)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(InteractiveObject.usingMaterial)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(InteractiveObject.inactiveMaterial)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(InteractiveObject.highlightMaterial)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(InteractiveObject.useOffset)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(InteractiveObject.useSound)));            
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(InteractiveObject.enableThisEvent)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(InteractiveObject.disableThisEvent)));
        }

        serializedObject.ApplyModifiedProperties();
    }
}

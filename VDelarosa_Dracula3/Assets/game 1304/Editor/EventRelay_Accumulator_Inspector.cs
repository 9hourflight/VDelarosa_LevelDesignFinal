using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[CustomEditor(typeof(EventListener_Accumulator))]
public class EventRelay_Accumulator_Inspector : Editor
{

    SerializedProperty m_eventToListenProp;
    SerializedProperty m_thresholdProp;
    SerializedProperty m_resetProp;
    SerializedProperty m_EventsToFireProp;

    public void OnEnable()
    {
        m_eventToListenProp = serializedObject.FindProperty("eventToListenFor");
        m_thresholdProp = serializedObject.FindProperty("accumulationThreshold");
        m_resetProp = serializedObject.FindProperty("resetOnAccumulation");
        m_EventsToFireProp = serializedObject.FindProperty("eventsToFire");
        
    }


    public override void OnInspectorGUI()
    {
        EventRelay_Accumulator accumulator = (EventRelay_Accumulator)target;
        EditorStyles.label.wordWrap = true;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("This behavior listens for an event to fire a certain number of times. When that threshold is reached, it then fires a list of events.");
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(m_eventToListenProp, new GUIContent("Event to Listen For"));
        EditorGUILayout.PropertyField(m_thresholdProp, new GUIContent("How many times"));
        EditorGUILayout.PropertyField(m_resetProp, new GUIContent("Reset this once it has fired?"));
        EditorGUILayout.PropertyField(m_EventsToFireProp, new GUIContent("Fire these events when the count has been reached"));
        /*
        dialogue.eventToListenFor = EditorGUILayout.TextField("Event to listen for", dialogue.eventToListenFor);        
        EditorGUILayout.PropertyField(m_dialogueProp, new GUIContent("Dialogue Entry"));*/

        serializedObject.ApplyModifiedProperties();
    }

    }
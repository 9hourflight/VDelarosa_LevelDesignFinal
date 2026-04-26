using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class SearchForEvent : EditorWindow
{
    private string inputText;

    [MenuItem("Tools/GAME1304/Search For Event")]   
    static void searchForEvent()
    {
        SearchForEvent window = ScriptableObject.CreateInstance<SearchForEvent>();
        //window.position = new Rect(Screen.width / 2, Screen.height / 2, 250, 150);
        //window.ShowPopup();
        window.ShowUtility();
    }

    private void OnGUI()
    {
        inputText = EditorGUILayout.TextField("Event name:", inputText);

        if (GUILayout.Button("Search"))
        {
            search(inputText);
        }

        if (GUILayout.Button("Abort"))
            Close();

    }

    private void search(string desiredSubstring)
    {
        Debug.Log("Searching for "+desiredSubstring);
        MonoBehaviour[] allSceneObjects = MonoBehaviour.FindObjectsOfType<MonoBehaviour>();
        MonoBehaviour[] sceneObjectsWithProperty = allSceneObjects.Where(sceneObject =>
                                                    sceneObject.GetType().GetProperties().Any(objectProperty =>
                                                    objectProperty.PropertyType == typeof(string) && (objectProperty.GetValue(sceneObject, null) as string).Contains(desiredSubstring))).ToArray();

        for (int i = 0;i<sceneObjectsWithProperty.Length; i++)
        {
            Debug.Log(sceneObjectsWithProperty[i]);
        }
    }

}

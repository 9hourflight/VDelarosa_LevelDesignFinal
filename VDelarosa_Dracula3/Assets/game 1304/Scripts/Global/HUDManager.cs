using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class objectiveTaskEntry
{
    public TaskEntry taskEntry;
	/*public string taskID;
	public string taskText;
	public taskState state;*/
    public GameObject markerSprite;
    //public GameObject markerLocation;
    public string taskUIText;
}

public enum UtilityMenuTabType { objectives,inventory,character,archive,map};
/*
[Serializable]
public class UITaskEntry
{
	string taskID;
	Text UIText;
	taskState state;
}*/

public static class HUDManager
{
    public static Canvas UtilityMenuCanvas;

    //public static Canvas objectiveCanvas;
    /*	public static Text objectiveDescriptionText;
        public static Text objectiveTitleText;
        public static Text objectiveTasksText;
        public static string ObjectiveDescription;
        public static List<objectiveTaskEntry> taskEntries;*/

    public static Canvas readingCanvas;
    public static Canvas HUDCanvas;

    public static Text HUDNotificationText;
    //TODO: protect this better
    public static Queue<string> notificationQueue;
    private static float notificationCooldown = 0;

    //public static Dictionary<string, Text> UITaskEntries;
    //public static List<UITaskEntry> UITaskEntries;

    private static bool isInitialized = false;
    public static Text readingTitleUIText;
    public static Text readingBodyUIText;

    public static Camera playerCam;
    public static GameObject taskMarkerPrefab;

    public static DialogueScriptableObject currentDialogue; 

    public static void reinit()
    {
        isInitialized = false;
        init();
    }
    public static void init()
    {
        if (isInitialized)
            return;

        isInitialized = true;       
        notificationQueue = new Queue<string>();
    }

    public static void openUtilityMenu(UtilityMenuTabType tab = UtilityMenuTabType.objectives)
    {
        GameManager.isPaused = true;
        if (UtilityMenuCanvas != null)
        {
            UtilityMenuCanvas.gameObject.SetActive(true);
            UtilityScreenManager.instance.setTab(tab);
        }
    }

    public static void closeUtilityMenu()
    {
        GameManager.isPaused = false;
        if (UtilityMenuCanvas != null)
            UtilityMenuCanvas.gameObject.SetActive(false);
    }


    public static GameObject makeMarkerSprite()
     {
        
         return GameObject.Instantiate(taskMarkerPrefab, HUDCanvas.transform);
            //tempOTE.markerLocation = markerLocation;
        
    }
    public static void updateTaskMarkerPositions(float deltaTime)
    {
        if (notificationCooldown > 0)
        {
            notificationCooldown -= deltaTime;
            if (notificationCooldown <= 0)
            {
                notificationCooldown = 0;
                if (notificationQueue.Count == 0)
                    HUDNotificationText.text = "";
            }
        }

        if((notificationCooldown<=0)&& (notificationQueue.Count > 0))
        {
            HUDManager.HUDNotificationText.enabled = true;
            HUDManager.HUDNotificationText.text = notificationQueue.Dequeue();

            notificationCooldown = 5.0f;
        }             

        Vector3 markerPosition;
        //TODO: pretty gross naked reference of another class' junk. Fix this
        if (ObjectivesTabManager.taskEntries != null)
        {
            foreach (objectiveTaskEntry ote in ObjectivesTabManager.taskEntries)
            {
                if (ote.markerSprite != null)
                {
                    if (ote.taskEntry.markerLocation != null)
                    {
                        markerPosition = playerCam.WorldToScreenPoint(ote.taskEntry.markerLocation.transform.position);
                        //markerPosition = playerCam.WorldToViewportPoint(ote.markerLocation.transform.position);                
                        //ote.markerSprite.transform.localPosition =  new Vector3(markerPosition.x*HUDCanvas.pixelRect.xMax, markerPosition.y * HUDCanvas.pixelRect.yMax,0);
                        if ((markerPosition.z >= 0) && (ote.taskEntry.initialState == taskState.active))
                        {
                            ote.markerSprite.SetActive(true);
                            //ote.markerSprite.transform.localPosition = new Vector3(markerPosition.x - HUDCanvas.pixelRect.xMax / 2, markerPosition.y - HUDCanvas.pixelRect.yMax / 2, 0);
                            //ote.markerSprite.transform.localPosition = new Vector3(markerPosition.x - HUDCanvas.pixelRect.center.x, markerPosition.y - HUDCanvas.pixelRect.center.y, 0);
                            ote.markerSprite.transform.position = new Vector3(markerPosition.x , markerPosition.y, 0);
                        }
                        else
                            ote.markerSprite.SetActive(false);
                    }
                    else
                        ote.markerSprite.SetActive(false);
                }
            }
        }
    }
}

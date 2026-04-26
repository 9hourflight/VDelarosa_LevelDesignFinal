using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectivesTabManager : UtilityTabBehavior
{
    public  Text objectiveDescriptionText;
    public Text objectiveTitleText;
    public Text objectiveTasksText;
    public Text objectiveCluesText;

    public static string objectiveTitle;
    public static string objectiveDescription;

    public static List<objectiveTaskEntry> taskEntries;
    public static List<ClueEntry> clueEntries;

    public static ObjectivesTabManager instance;
    public ScrollRect taskTextScroller;
    public ScrollRect objectiveCluesScroller;

    private static bool isInitialized = false;
    void Awake()
    {
        instance = this;
        
    }

    public override void display()
    {
        objectiveTitleText.text = objectiveTitle;
        objectiveDescriptionText.text = objectiveDescription;
        repositionObjectiveUIElements();
    }
    public static void init()
    {
        if (isInitialized)
            return;
        isInitialized = true;        
        taskEntries = new List<objectiveTaskEntry>();
        clueEntries = new List<ClueEntry>();
        
    }

    // Update is called once per frame
    void Update()
    {
        float wheel = Input.GetAxisRaw("Mouse ScrollWheel");
        if (wheel < 0)
        {
            taskTextScroller.verticalNormalizedPosition -= 0.125f;
        }
        if (wheel > 0)
        {
            taskTextScroller.verticalNormalizedPosition += 0.125f;
        }
        if (Input.GetKey(KeyCode.PageUp))
        {
            taskTextScroller.verticalNormalizedPosition += 0.5f * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.PageDown))
        {
            taskTextScroller.verticalNormalizedPosition -= 0.5f * Time.deltaTime;
        }
    }

    public static void setObjectiveTitle(string newTitle)
    {
        init();
        objectiveTitle = newTitle;        
    }

    public static void addTask(TaskEntry te)
    {
        init();

        for (int i = 0; i < taskEntries.Count; i++)
        {
            if (taskEntries[i].taskEntry == te)
                return;
        }


        GameObject taskMarker;
        objectiveTaskEntry tempOTE = new objectiveTaskEntry();
        Text textComponent;
        string tempText;

        tempOTE.taskEntry = te;
        if (te.markerLocation != null)
        {
            tempOTE.markerSprite = HUDManager.makeMarkerSprite();
        }



        tempText = te.taskText;
        if (te.isOptional)
            tempText += "(Optional)";
        switch (te.taskType)
        {
            case TaskType.standard:
                tempOTE.taskUIText = tempText;
                break;
            case TaskType.XofY:
                tempOTE.taskUIText = tempText + ": " + te.currentCount + "/" + te.count;
                break;
            case TaskType.XRemaining:
                tempOTE.taskUIText = tempText + ": " + te.currentCount + " remaining";
                break;

        }


        //tempOTE.UIText = textComponent;
        taskEntries.Add(tempOTE);
        //UITaskEntries.Add(taskID, UIText.GetComponent<Text>());
        repositionObjectiveUIElements();

        //GameObject UISprite = Instantiate(InventoryImageUIPrefab, inventoryCanvas.transform);
        //notificationQueue.Enqueue("Added: "+taskText);


    }

    public static void addClue(ClueEntry ce)
    {
        init();

        if(clueEntries.Contains(ce))
        {
            return;
        }        

        clueEntries.Add(ce);
        
        repositionObjectiveUIElements();        


    }

    public static string StrikeThrough(string s)
    {
        string strikethrough = "";
        foreach (char c in s)
        {
            strikethrough = strikethrough + c + '\u0336';
        }
        return strikethrough;
    }

    private static void repositionObjectiveUIElements()
    {
        if (instance == null)
            return;
        string tempTaskString = "";
        string tempClueString = "";

        instance.objectiveTasksText.text = "";
        if (taskEntries != null)
        {
            for (int i = 0; i < taskEntries.Count; i++)
            {
                tempTaskString = taskEntries[i].taskUIText;// .taskEntry.taskText ;

                //taskEntries[i].UIText.transform.localPosition = new Vector3(taskEntries[i].UIText.transform.localPosition.x, 225 - i * 50 - deadEntryOffset, taskEntries[i].UIText.transform.localPosition.z);
                switch (taskEntries[i].taskEntry.initialState)
                {
                    case taskState.active:
                        if (taskEntries[i].taskEntry.isOptional)
                            instance.objectiveTasksText.text += "• " + "<b><color=#ffa500ff>" + tempTaskString + "</color></b>\n";
                        else
                            instance.objectiveTasksText.text += "• " + "<b><color=#ffffffff>" + tempTaskString + "</color></b>\n";
                        break;
                    case taskState.complete:
                        instance.objectiveTasksText.text += "• " + "<b><color=#506550ff>" + StrikeThrough(tempTaskString) + "</color></b>\n";
                        break;
                    case taskState.failed:
                        instance.objectiveTasksText.text += "• " + "<b><color=#655050ff>" + StrikeThrough(tempTaskString) + "</color></b>\n";
                        break;
                    case taskState.hidden:
                        //deadEntryOffset -= 50;
                        //taskEntries[i].UIText.enabled = false;
                        break;
                    case taskState.unassigned:
                        //deadEntryOffset -= 50;
                        //taskEntries[i].UIText.enabled = false;
                        break;
                }

            }
        }
        instance.objectiveCluesText.text = "";
        if (clueEntries !=null)
        {
            for (int i = 0; i < clueEntries.Count; i++)
            {
                tempClueString = clueEntries[i].ClueText;// .taskEntry.taskText ;
                
                switch (clueEntries[i].getCurrentState())
                {
                    case ClueState.visible:
                        instance.objectiveCluesText.text += "• " + "<b><color=#ffa500ff>" + tempClueString + "</color></b>\n";
                        break;
                    case ClueState.hidden:                        
                        break;
                }
            }
        }
    }
    

    public static void setObjectiveDescription(string description)
    {        
        objectiveDescription = description;
    }

    public static void updateTaskCount(TaskEntry te) //, taskState newState)
    {
        foreach (objectiveTaskEntry ote in taskEntries)
        {
            if (ote.taskEntry == te)
            {
                switch (te.taskType)
                {
                    case TaskType.standard:
                        ote.taskUIText = te.taskText;
                        break;
                    case TaskType.XofY:
                        ote.taskUIText = te.taskText + ": " + te.currentCount + "/" + te.count;
                        break;
                    case TaskType.XRemaining:
                        ote.taskUIText = te.taskText + ": " + te.currentCount + " remaining";
                        break;

                }

            }
        }
    }

    public static void updateClueState(ClueEntry ce) //, taskState newState)
    {
        foreach (ClueEntry c in clueEntries)
        {
            if (c == ce)
            {
                c.changeState(ce.getCurrentState());
            }
            repositionObjectiveUIElements();
        }
    }

    public static void updateTaskState(TaskEntry te) //, taskState newState)
    {
        foreach (objectiveTaskEntry ote in taskEntries)
        {
            if (ote.taskEntry == te)
            {
                //ote.taskEntry.state = newState;			
                repositionObjectiveUIElements();
                switch (ote.taskEntry.initialState)
                {
                    case taskState.active:
                        if (ote.markerSprite != null)
                        {
                            ote.markerSprite.SetActive(true);
                            if (ote.taskEntry.isOptional)
                            {
                                if (ote.markerSprite.GetComponent<Image>() != null)
                                    ote.markerSprite.GetComponent<Image>().color = Color.yellow;
                            }
                        }
                        break;
                    case taskState.complete:
                        if (ote.markerSprite != null)
                            ote.markerSprite.SetActive(false);
                        HUDManager.notificationQueue.Enqueue("Completed: " + ote.taskEntry.taskText);
                        break;
                    case taskState.failed:
                        if (ote.markerSprite != null)
                            ote.markerSprite.SetActive(false);
                        HUDManager.notificationQueue.Enqueue("Failed: " + ote.taskEntry.taskText);
                        break;
                    case taskState.hidden:
                        if (ote.markerSprite != null)
                            ote.markerSprite.SetActive(false);
                        break;
                    case taskState.unassigned:
                        if (ote.markerSprite != null)
                            ote.markerSprite.SetActive(false);
                        break;

                }
            }
        }
    }
}

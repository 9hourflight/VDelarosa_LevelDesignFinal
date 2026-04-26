using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum taskState {unassigned, active, complete, failed, hidden};
public enum TaskType { standard, XofY, XRemaining};
public enum objectiveState {inactive, active, complete, failed};

public enum ClueState { hidden, visible};

[Serializable]
public class TaskEntry
{
    [Header("Display")]
    public string taskText;
    public GameObject markerLocation;
    [FormerlySerializedAs("state")]
    public taskState initialState = taskState.unassigned;
    public bool isOptional = false;

    public TaskType taskType = TaskType.standard;
    [Tooltip("If this is a counter type of task, this will determine how many need to be counted up to or how many need to be counted down from")]
    public int count = 0;
    [HideInInspector]
    public int currentCount;
    public bool autoCompleteOnCountReached = true;
    [Header("Event Listening")]
    public List<TaskCounterChangeEntry> counterChangeEvents;
    public List<eventSetTaskStateEntry> taskStateEvents;
    public List<eventSetTaskMarkerEntry> taskMarkerChangeEvents;

    

    [Header("Event Sending")]
    public List<EventPackage> eventsToSendOnComplete;
    public List<EventPackage> eventsToSendOnFail;

}

[Serializable]
public class ClueEntry
{
    public string ClueText = "";
    public ClueState initialState = ClueState.hidden;
    [Header("Event Listening")]
    public List<ClueStateChangeEntry> clueStateChangeEvents;
    private ClueState currentState;

    public void Init()
    {
        currentState = initialState;
    }
    //TODO: migrate to setter and getter
    public void changeState(ClueState newState)
    {
        currentState = newState;
    }

    public ClueState getCurrentState()
    {
        return currentState;
    }
}

[Serializable]
public class TaskCounterChangeEntry
{
    public string eventName;
    public operationType counterOperation;
    public int value;
}

[Serializable]
public class eventAddTaskEntry
{
	public string eventName;
	public string taskID;
	public string taskText;
	public taskState state = taskState.active;
	public GameObject markerLocation;
}

[Serializable]
public class eventSetTaskStateEntry
{
	public string eventName;	
	public taskState updatedState;
}



[Serializable]
public class eventSetObjectiveStateEntry
{
    public string eventName;
    public objectiveState updatedState;
}

[Serializable]
public class eventSetTaskMarkerEntry
{
    public string eventName;
    public GameObject updatedMarkerLocation;
}

[Serializable]
public class ClueStateChangeEntry
{
    public string eventName;
    public ClueState newState;
}

[Serializable]
public class eventChangeObjectiveDescriptionEntry
{
	public string eventName;
    [TextArea(15,20)]
	public string updatedObjectiveDescription;
}



public class ObjectivesEventManager : MonoBehaviour 
{
    public string objectiveTitle = "Objectives";
    //public objectiveState initialState = objectiveState.inactive;
    public List<TaskEntry> taskEntries;
    public List<ClueEntry> ObjectiveClues;
    [TextArea(15, 20)]
    public string initialDescription;
    public List<eventChangeObjectiveDescriptionEntry> descriptionChangeEvents;



    private void Start()
    {
        Invoke("init", 0.1f);
    }

    void init() 
	{
        ObjectivesTabManager.setObjectiveTitle(objectiveTitle);
        foreach(TaskEntry te in taskEntries)
        {
            if (te.taskType == TaskType.XofY)
                te.currentCount = 0;
            if (te.taskType == TaskType.XRemaining)
                te.currentCount = te.count;
            ObjectivesTabManager.addTask(te);            
        }
        foreach (ClueEntry ce in ObjectiveClues)
        {
            ce.Init();
            ObjectivesTabManager.addClue(ce);
        }

        ObjectivesTabManager.setObjectiveDescription(initialDescription);
        //EventRegistry.Init();
        foreach (eventChangeObjectiveDescriptionEntry ode in descriptionChangeEvents)
        {            
                EventRegistry.AddEvent(ode.eventName, changeDescription, gameObject);            
        }

        foreach(TaskEntry te in taskEntries)
        {
            foreach(TaskCounterChangeEntry tcce in te.counterChangeEvents)
            {
                EventRegistry.AddEvent(tcce.eventName, changeTaskCounter, gameObject);

            }
            foreach (eventSetTaskStateEntry estse in te.taskStateEvents)
            {
                EventRegistry.AddEvent(estse.eventName, setTaskState, gameObject);

            }

            foreach (eventSetTaskMarkerEntry stme in te.taskMarkerChangeEvents)
            {                
                EventRegistry.AddEvent(stme.eventName, setTaskMarker, gameObject);             
            }
            
        }

        foreach (ClueEntry ce in ObjectiveClues)
        {
            foreach (ClueStateChangeEntry csce in ce.clueStateChangeEvents)
            {
                EventRegistry.AddEvent(csce.eventName, changeClueState, gameObject);

            }            

        }

    }	

    void changeTaskCounter(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        
        foreach(TaskEntry te in taskEntries)
        {
            foreach (TaskCounterChangeEntry tcce in te.counterChangeEvents)
            {
                if (tcce.eventName == eventName)
                {
                    switch(tcce.counterOperation)
                    {
                        case operationType.add:
                            te.currentCount += tcce.value;
                            break;
                        case operationType.divide:
                            te.currentCount /= tcce.value;
                            break;
                        case operationType.multiply:
                            te.currentCount *= tcce.value;
                            break;
                        case operationType.set:
                            te.currentCount = tcce.value;
                            break;
                        case operationType.subtract:
                            te.currentCount -= tcce.value;
                            break;
                    }
                    ObjectivesTabManager.updateTaskCount(te);
                    if(te.autoCompleteOnCountReached)
                    {
                        if((te.taskType == TaskType.XofY)&&(te.currentCount >= te.count))
                        {
                            setStateDirect(te, taskState.complete);
                        }
                        if ((te.taskType == TaskType.XRemaining) && (te.currentCount <=0))
                        {
                            setStateDirect(te, taskState.complete);
                            
                        }
                    }
                }
            }
        }
    }

	void setTaskState(string eventName, GameObject obj)
	{
        if ((obj != null) && (obj != this.gameObject))
            return;
        foreach (TaskEntry te in taskEntries)
        {
            foreach (eventSetTaskStateEntry ests in te.taskStateEvents)
            {
                if (ests.eventName == eventName)
                {
                    setStateDirect(te, ests.updatedState);                    
                }
            }
        }
    }

    void changeClueState(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        foreach (ClueEntry ce in ObjectiveClues)
        {
            foreach (ClueStateChangeEntry csce in ce.clueStateChangeEvents)
            {
                if (csce.eventName == eventName)
                {

                    setClueStateDirect(ce, csce.newState);
                }
            }
        }
    }

    void setClueStateDirect(ClueEntry ce, ClueState newState)
    {
        ce.changeState(newState);
        ObjectivesTabManager.updateClueState(ce);        
    }

    void setStateDirect(TaskEntry te, taskState newState)
    {
        te.initialState = newState;
        ObjectivesTabManager.updateTaskState(te);
        if(newState == taskState.complete)
        {
            foreach(EventPackage ep in te.eventsToSendOnComplete)
            {
                EventRegistry.SendEvent(ep, this.gameObject);
            }
        }
        if (newState == taskState.failed)
        {
            foreach (EventPackage ep in te.eventsToSendOnFail)
            {                
                EventRegistry.SendEvent(ep, this.gameObject);
            }
        }
    }

	void removeTask(string eventName, GameObject obj)
	{
        if ((obj != null) && (obj != this.gameObject))
            return;
        //TODO: implement this
    }

	void changeDescription(string eventName, GameObject obj)
	{
        if ((obj != null) && (obj != this.gameObject))
            return;
        foreach (eventChangeObjectiveDescriptionEntry entry in descriptionChangeEvents)
		{
			if(entry.eventName == eventName)
			{
                ObjectivesTabManager.setObjectiveDescription(entry.updatedObjectiveDescription);
			}
		}

	}

    void setTaskMarker(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        foreach (TaskEntry te in taskEntries)
        {
            foreach (eventSetTaskMarkerEntry tee in te.taskMarkerChangeEvents)
            {
                if (tee.eventName == eventName)
                {
                    te.markerLocation = tee.updatedMarkerLocation;
                    // HUDManager.updateTaskMarker(te);
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeLockBehavior : MonoBehaviour
{
    public int targetNumber = 451;
    public List<CodeWheelBehavior> codeWheels;
    private List<int> targetNumbers;
    [Header("Event Sending")]
    public List<EventPackage> eventsToSend;

    private void Update()
    {
        int x = 5;
    }
    private void Start()
    {
        
        foreach(CodeWheelBehavior cwb in codeWheels)
        {
            cwb.setParentLock(this);
        }
        targetNumbers = new List<int>();
        targetNumbers.Add( ((targetNumber % (1000 * 10)) - (targetNumber % 1000)) / 1000);
        targetNumbers.Add(((targetNumber % (100 * 10)) - (targetNumber % 100)) / 100);
        targetNumbers.Add(((targetNumber % (10 * 10)) - (targetNumber % 10)) / 10);
        targetNumbers.Add(((targetNumber % (1 * 10)) - (targetNumber % 1)) / 1);
    }
    public void checkCode()
    {
        bool checksOut = true;
        if (codeWheels.Count != targetNumbers.Count)
            return;
        for (int i = 0; i < codeWheels.Count; i++)
        {
            if(codeWheels[i].getNumber() != targetNumbers[i])
            {
                checksOut = false;
            }
        }
        if(checksOut)
        {
            foreach (EventPackage ep in eventsToSend)
            {
                if ((ep.scope == eventScope.instigator) || (ep.scope == eventScope.visualScriptingInstigator))
                    EventRegistry.SendEvent(ep, GameManager.player);
                else
                    EventRegistry.SendEvent(ep, this.gameObject);
            }
        }
    }

}

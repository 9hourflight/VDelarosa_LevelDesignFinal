using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeWheelBehavior : MonoBehaviour
{
    private int currentNum;
    private CodeLockBehavior parentLock;

    public void setParentLock(CodeLockBehavior clb)
    {
        parentLock = clb;
    }
    public void setNumber(int num)
    {
        currentNum = num % 10;        
        transform.localRotation = Quaternion.Euler(0, 0, -180+num * 36);
        if(parentLock!=null)
            parentLock.checkCode();
    }
    private void Start()
    {
        EventRegistry.AddEvent("SpinCodeWheel", spinWheel, gameObject);
        //add random start later
        setNumber(0);
    }

    private void spinWheel(string eventName, GameObject obj)
    {
        if ((obj != null) && (obj != this.gameObject))
            return;
        setNumber(currentNum + 1);
        
    }

    public int getNumber()
    { 
        return currentNum; 
    }
}

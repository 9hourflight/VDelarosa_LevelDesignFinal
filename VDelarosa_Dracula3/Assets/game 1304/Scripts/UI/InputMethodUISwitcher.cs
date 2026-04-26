using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputModeObjectList
{
    public InputMode mode;
    public List<GameObject> objectList;
}
public class InputMethodUISwitcher : MonoBehaviour
{
    public List<InputModeObjectList> inputModeObjects;
    private InputMode currentMode = InputMode.keyboard;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void switchMode(InputMode newMode)
    {
        if (newMode == currentMode)
            return;
        foreach(InputModeObjectList imol in inputModeObjects)
        {
            if (imol.mode == newMode)
            {
                foreach (GameObject go in imol.objectList)
                {
                    go.SetActive(true);
                }
            }
            else
            {
                foreach (GameObject go in imol.objectList)
                {
                    go.SetActive(false);
                }
            }
        }
    }
}

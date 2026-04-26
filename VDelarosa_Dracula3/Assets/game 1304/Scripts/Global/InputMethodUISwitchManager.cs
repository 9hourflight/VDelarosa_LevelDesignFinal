using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputMode { keyboard, controller};
public static class InputMethodUISwitchManager
{
    public static bool isInitialized = false;
    public static List<InputMethodUISwitcher> switchers;
    public static void init()
    {
        if (isInitialized)
            return;
        isInitialized = true;
        switchers = new List<InputMethodUISwitcher>();
    }

    public static void registerMe(InputMethodUISwitcher switcher)
    {
        init();
        switchers.Add(switcher);
    }

    public static void changeInputMode(InputMode newMode)
    {

    }
}

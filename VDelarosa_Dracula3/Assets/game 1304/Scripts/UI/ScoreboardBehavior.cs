using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using System;

[Serializable]
public class ScoreboardEntry
{
    public string tokenName;
    public string label;
    public int maxCountPossible;
    public bool useMaxCountPossible;
    public bool displayAsTime;
}

[RequireComponent(typeof(Text))]
public class ScoreboardBehavior : MonoBehaviour
{
    public string scoreboardTitle;    
    public List<ScoreboardEntry> scoreboardEntries;
    Text thisText;

    void Start()
    {
        thisText = GetComponent<Text>();
        thisText.text = "";
        if(scoreboardEntries != null)
        {
            if(scoreboardEntries.Count >0)
            {
                thisText.text = "<size=32>" + scoreboardTitle + "\n</size><size=22>";
                foreach(ScoreboardEntry sbe in scoreboardEntries)
                {
                    thisText.text += sbe.label + ": ";
                    if(sbe.displayAsTime)
                    {
                        thisText.text += EventListener_Stopwatch.MakeHoursMinutesSeconds(TokenRegistry.getToken(sbe.tokenName));
                    }
                    else                        
                        thisText.text += TokenRegistry.getToken(sbe.tokenName);
                    if(sbe.useMaxCountPossible)
                    {
                        thisText.text += " of " + sbe.maxCountPossible;
                    }
                    thisText.text += "\n";
                }
                thisText.text += "</size>";
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

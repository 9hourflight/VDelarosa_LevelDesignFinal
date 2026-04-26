using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using System;


[Serializable]
public class CanvasEntry
{
    public UtilityMenuTabType tabType;
    public Canvas tabCanvas;
}
public class UtilityScreenManager : MonoBehaviour
{
    //public List<Canvas> canvasList;
    public List<CanvasEntry> tabCanvases;
    public static UtilityScreenManager instance;
    private UtilityMenuTabType currentTabType;
    private Canvas currentTab;
    private int currentTabIndex;
    
    void Awake()
    {
        instance = this;    
    }

    public void setTab(UtilityMenuTabType newTab)
    {        
        for (int x = 0; x < tabCanvases.Count; x++)
        {
            if (instance.tabCanvases[x].tabType == newTab)
            {
                tabCanvases[x].tabCanvas.enabled = true;
                UtilityTabBehavior utb;
                if(tabCanvases[x].tabCanvas.gameObject.TryGetComponent<UtilityTabBehavior>(out utb))
                {
                    utb.display();
                }
                currentTabIndex = x;
                currentTabType = newTab;
                currentTab = instance.tabCanvases[x].tabCanvas;
            }
            else
                tabCanvases[x].tabCanvas.enabled = false;
        }
    }

    public void setTab(int index)
    {
        UtilityTabBehavior utb;
        //take care of index overflow
        if (index >= tabCanvases.Count)
            index = index % tabCanvases.Count;

        //take care of index underflow
        if((tabCanvases.Count != 0)&&(index <0))
        {
            while (index < 0)
            {
                index += tabCanvases.Count;
            }
        }
        currentTabIndex = index;
        currentTabType = tabCanvases[index].tabType;
        currentTab = tabCanvases[index].tabCanvas;

        for (int x=0;x< tabCanvases.Count;x++)
        {
            if (x == index)
            {
                tabCanvases[x].tabCanvas.enabled = true;
                if (tabCanvases[x].tabCanvas.gameObject.TryGetComponent<UtilityTabBehavior>(out utb))
                {
                    utb.display();
                }
            }
            else
                tabCanvases[x].tabCanvas.enabled = false;
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) // || Input.GetButtonDown("backButton"))
        {
            HUDManager.closeUtilityMenu();
        }
        //Next tab
        if(Input.GetKeyDown(KeyCode.E))
        {
            setTab(currentTabIndex + 1);
        }
        //previous tab
        if (Input.GetKeyDown(KeyCode.Q))
        {
            setTab(currentTabIndex - 1);
        }        

        if (Input.GetButtonDown("Inventory"))
        {
            if(currentTabType == UtilityMenuTabType.inventory)
                HUDManager.closeUtilityMenu();
            else
                setTab(UtilityMenuTabType.inventory);
        }

        if (Input.GetButtonDown("Objectives"))
        {
            if (currentTabType == UtilityMenuTabType.objectives)
                HUDManager.closeUtilityMenu();
            else
                setTab(UtilityMenuTabType.objectives);
        }
    }
}

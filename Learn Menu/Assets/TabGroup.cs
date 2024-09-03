using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    public List<TabButton> tabButtons;
    public Sprite tabIdle;
    public Sprite tabHover;
    public Sprite tabActive;
    

    public void Subscribe(TabButton button)
    {
        if (tabButtons == null)
        {
            tabButtons = new List<TabButton>();
        }
        
        tabButtons.Add(button);
    }

    public void OnTabEnter(TabButton button)
    {
        ResetTabs();
        button.background.sprite = tabHover;

    }
    
    public void OnTabExit(TabButton button)
    {
        ResetTabs();
        button.background.sprite = tabIdle;

    }
    
    public void OnTabSelected (TabButton button)
    {
        ResetTabs();
        button.background.sprite = tabActive;

    }

    public void ResetTabs()
    {
        foreach (TabButton button in tabButtons)
        {
            button.background.sprite = tabIdle;
        }
    }

}

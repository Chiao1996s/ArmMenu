using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerClickHandler, IPointerExitHandler, IPointerEnterHandler
{
    public TabGroup TabGroup;
    public Image background;
    
    
    // Start is called before the first frame update
    void Start()
    {
        background = GetComponent<Image>();
        TabGroup.Subscribe(this);
        

    }


    public void OnPointerClick(PointerEventData eventData)
    {
        TabGroup.OnTabSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TabGroup.OnTabEnter(this);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        TabGroup.OnTabExit(this);
    }


}

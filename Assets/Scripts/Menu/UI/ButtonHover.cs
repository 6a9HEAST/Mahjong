using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHover: MonoBehaviour,IPointerEnterHandler,
    IPointerExitHandler
{
    //[SerializeField] private Button buttonComponent;
    public GameObject Light;


    public void Awake()
    {
        Light.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Light.SetActive(true);        
    }

    // Вызывается, когда курсор уходит из области кнопки
    public void OnPointerExit(PointerEventData eventData)
    {
        Light.SetActive(false);
    }
}


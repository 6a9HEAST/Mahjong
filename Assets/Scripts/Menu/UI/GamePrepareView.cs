using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePrepareView : MonoBehaviour
{
    public GameObject Itself;
    public Transform OkButton;
    public Transform CancelButton;
    public List<Transform> gameLenghts;
    
    public MenuManager MenuManager { get; private set; }
    public void Start()
    {
        Itself.SetActive(false);
        if (OkButton!=null&&CancelButton!=null&&gameLenghts.Count>0)
        CreateButtons();
    }
    public void Show(MenuManager menuManager)
    {
        Itself.SetActive(true);
        if (MenuManager == null)
        {
            MenuManager = menuManager;
        }
    }

    public void CreateButtons()
    {
        Button buttonComponent = OkButton.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.RemoveAllListeners();

            buttonComponent.onClick.AddListener(() => OnOkButtonClicked());
        }

        //buttonComponent = CancelButton.GetComponent<Button>();
        //if (buttonComponent != null)
        //{
        //    buttonComponent.onClick.RemoveAllListeners();

        //    buttonComponent.onClick.AddListener(() => OnCancelButtonClicked());
        //}

        buttonComponent = gameLenghts[0].GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.RemoveAllListeners();

            buttonComponent.onClick.AddListener(() => On1LenghtClecked());
        }

        buttonComponent = gameLenghts[1].GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.RemoveAllListeners();

            buttonComponent.onClick.AddListener(() => On2LenghtClecked());
        }

        buttonComponent = gameLenghts[2].GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.RemoveAllListeners();

            buttonComponent.onClick.AddListener(() => On3LenghtClecked());
        }
    }

    public void OnOkButtonClicked()
    {

    }

    public void On1LenghtClecked()
    {

    }
    public void On2LenghtClecked()
    {

    }
    public void On3LenghtClecked()
    {

    }



    // Update is called once per frame
    void Update()
    {
        
    }
}

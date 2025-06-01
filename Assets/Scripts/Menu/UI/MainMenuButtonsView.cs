using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuButtons
{
    MenuManager MenuManager;

    public MainMenuButtons(MenuManager menuManager)
    {
        MenuManager = menuManager;
    }

    public void CreateButtons(List<GameObject> buttons)
    {
        if (buttons.Count == 0) return;

        CreatePlayButton(buttons[0]);
        CreateHelpButton(buttons[1]);
        CreateSettingsButton(buttons[2]);
        CreateExitButton(buttons[3]);

    }

    public void CreatePlayButton(GameObject button)
    {
        Button buttonComponent = button.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.RemoveAllListeners();

            buttonComponent.onClick.AddListener(() => MenuManager.StartCoroutine(MenuManager.OnPlayButtonClicked(button)));
        }
    }

    public void CreateHelpButton(GameObject button)
    {
        Button buttonComponent = button.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.RemoveAllListeners();

            buttonComponent.onClick.AddListener(() => MenuManager.StartCoroutine(MenuManager.OnHelpButtonClicked(button)));
            
        }
    }

    public void CreateSettingsButton(GameObject button)
    {
        Button buttonComponent = button.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.RemoveAllListeners();

            buttonComponent.onClick.AddListener(() => MenuManager.StartCoroutine(MenuManager.OnSettingsButtonClicked(button)));
        }
    }

    public void CreateExitButton(GameObject button)
    {
        Button buttonComponent = button.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.RemoveAllListeners();

            buttonComponent.onClick.AddListener(() => MenuManager.StartCoroutine(MenuManager.OnExitButtonClicked(button)));
        }
    }
}

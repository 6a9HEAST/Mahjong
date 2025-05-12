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

    public void CreateButtons(List<Transform> buttons)
    {
        if (buttons.Count == 0) return;

        CreatePlayButton(buttons[0]);
        CreateHelpButton(buttons[1]);
        CreateSettingsButton(buttons[2]);
        CreateExitButton(buttons[3]);

    }

    public void CreatePlayButton(Transform button)
    {
        Button buttonComponent = button.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.RemoveAllListeners();

            buttonComponent.onClick.AddListener(() => MenuManager.OnPlayButtonClicked());
        }
    }

    public void CreateHelpButton(Transform button)
    {
        Button buttonComponent = button.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.RemoveAllListeners();

            buttonComponent.onClick.AddListener(() => MenuManager.OnHelpButtonClicked());
        }
    }

    public void CreateSettingsButton(Transform button)
    {
        Button buttonComponent = button.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.RemoveAllListeners();

            buttonComponent.onClick.AddListener(() => MenuManager.OnSettingsButtonClicked());
        }
    }

    public void CreateExitButton(Transform button)
    {
        Button buttonComponent = button.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.RemoveAllListeners();

            buttonComponent.onClick.AddListener(() => MenuManager.OnExitButtonClicked());
        }
    }
}

using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePrepareView : MonoBehaviour
{
    public GameObject Itself;
    public Transform OkButton;
    public Transform CancelButton;
    public List<Transform> DifficultyButtons;
    public MenuManager MenuManager { get; set; }

    public void Start()
    {
        if (OkButton != null && CancelButton != null && CheckButtons())
            CreateButtons();
        Itself.SetActive(false);
       
    }

    private bool CheckButtons()
    {
        bool result = true;
        if (DifficultyButtons.Count == 0) result = false;

        return result;
    }
    public void Show(MenuManager menuManager)
    {
        Itself.SetActive(true);
        if (MenuManager == null)
        {
            MenuManager = menuManager;
        }
    }

    public void Hide()
    {
        Itself.SetActive(false);
    }

    public void CreateButtons()
    {
        Button buttonComponent = OkButton.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.RemoveAllListeners();

            buttonComponent.onClick.AddListener(() => OnOkButtonClicked());
        }

        buttonComponent = CancelButton.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.RemoveAllListeners();

            buttonComponent.onClick.AddListener(() => OnCancelButtonClicked());
        }

        buttonComponent = DifficultyButtons[0].GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.RemoveAllListeners();

            buttonComponent.onClick.AddListener(() => OnEasyDifficultyClicked());
        }

        buttonComponent = DifficultyButtons[1].GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.RemoveAllListeners();

            buttonComponent.onClick.AddListener(() => OnMediumDifficultyClicked());
        }
    }

    public void OnOkButtonClicked()
    {
        MenuManager.StartGame();
    }

    public void OnCancelButtonClicked()
    {
        MenuManager.ShowButtons();
        Itself.SetActive(false);
    }

    public void OnEasyDifficultyClicked()
    {
        GameSettings.MediumAiDifficulty = false;
    }
    public void OnMediumDifficultyClicked()
    {
        GameSettings.MediumAiDifficulty = true;
    }
}

using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject ButtonsBox;
    public List<Transform> Buttons;
    public GamePrepareView GamePrepareView;

    void Start()
    {
        MainMenuButtons mainMenuButtons = new(this);
        mainMenuButtons.CreateButtons(Buttons);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void HideButtonBox()
    {
        ButtonsBox.SetActive(false);
    }

    public void ShowButtonBox()
    {
        ButtonsBox.SetActive(true);
    }

    public void OnPlayButtonClicked()
    {
        //GamePrepareView.Show(this);
        //HideButtonBox();

        StartGame();
    }

    public void OnSettingsButtonClicked()
    {
        Debug.Log("Settings button clicked");
    }
    public void OnHelpButtonClicked()
    {
        Debug.Log("Help button clicked");
    }

    public void OnExitButtonClicked()
    {
        Application.Quit();
    }

    void Update()
    {
        
    }
}

using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public GameObject Itself;
    public GameManager GameManager;
    public LeftButtonsView LeftButtonsView;
    public List<GameObject> Screens;
    public Button ExitButton;
    public bool InGameMenu = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ExitButton.onClick.AddListener(OnExitButtonClicked);
        foreach (GameObject screen in Screens) { screen.SetActive(false); }
        ShowYakuScreen();
    }

    public void Open()
    {
        Itself.SetActive(true);
    }

    public void Close()
    {
        Itself.SetActive(false);
        GameManager.TutorialOpened = false;
    }

    public void ShowYakuScreen()
    {
        foreach (GameObject screen in Screens) { screen.SetActive(false); }
        Screens[0].SetActive(true);
        var scr = Screens[0].GetComponent<YakuListView>();
        scr.DrawFirst();
    }
    public void ShowTutorialScreen()
    {
        foreach (GameObject screen in Screens) { screen.SetActive(false); }
        Screens[1].SetActive(true);
    }
    public void ShowPointsCalculationScreen()
    {
        foreach (GameObject screen in Screens) { screen.SetActive(false); }
        Screens[2].SetActive(true);
    }
    public void OnExitButtonClicked()
    {
        if (InGameMenu) Close();
        else SceneManager.LoadScene("MainMenu");
    }
}

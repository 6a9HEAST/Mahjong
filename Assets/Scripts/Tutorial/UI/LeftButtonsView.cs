using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftButtonsView : MonoBehaviour
{
    public TutorialManager TutorialManager;
    public List<Button> Buttons;
    public Sprite DefaultSprite;
    public Sprite SelectedSprite;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Buttons[0].onClick.AddListener(OnYakuListButtonClick);
        Buttons[1].onClick.AddListener(OnTutorialButtonClick);
        Buttons[2].onClick.AddListener(OnPointsCalculationButtonClick);
        //OnYakuListButtonClick();
    }

    private void OnYakuListButtonClick()
    {
        SetSelectedSprite(Buttons[0]);
        TutorialManager.ShowYakuScreen();
    }
    private void OnTutorialButtonClick()
    {
        SetSelectedSprite(Buttons[1]);
        TutorialManager.ShowTutorialScreen();
    }
    private void OnPointsCalculationButtonClick()
    {
        SetSelectedSprite(Buttons[2]);
        TutorialManager.ShowPointsCalculationScreen();
    }
    private void SetSelectedSprite(Button _button)
    {
        foreach (Button button in Buttons)
        {
            button.image.sprite = DefaultSprite;
        }
        _button.image.sprite = SelectedSprite;
    }
}

using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BasicTutorialView : MonoBehaviour
{
    public List<Sprite> Pages;

    public SpriteRenderer SpriteRenderer;

    public GameObject LeftButton;
    public GameObject RightButton;

    public int pageIndex = 0;
    public TextMeshProUGUI PageIndicator;
  
    void Start()
    {
        LeftButton.GetComponent<Button>().onClick.AddListener(OnLeftButtonClicked);
        RightButton.GetComponent<Button>().onClick.AddListener(OnRightButtonClicked);
        DrawPage();
        DrawPageButtons();
    }

    private void OnLeftButtonClicked()
    {
        pageIndex--;
        DrawPage();
        DrawPageButtons();
    }

    private void OnRightButtonClicked()
    {
        pageIndex++;
        DrawPage();
        DrawPageButtons();
    }

    private void DrawPageButtons()
    {
        if (pageIndex == 0) LeftButton.SetActive(false);
        else LeftButton.SetActive(true);

        if (pageIndex == Pages.Count - 1) RightButton.SetActive(false);
        else RightButton.SetActive(true);

        PageIndicator.text = (pageIndex + 1).ToString() + "/" + Pages.Count.ToString();
    }

    private void DrawPage()
    {
        SpriteRenderer.sprite = Pages[pageIndex];
    }
}

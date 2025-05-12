using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameEndScreenView : MonoBehaviour
{
    public GameObject Itself;
    public List<Transform> PlaceBoxes;
    private GameManager GameManager;
    public Transform ContinueButton;

    public void Draw(GameManager gameManager)
    {
        GameManager = gameManager;
        Show();
        GameManager.Players.Sort((x, y) => y.Score.CompareTo(x.Score));
        DrawPlaceBoxes();
        DrawContinueButton();
    }

    public void DrawPlaceBoxes()
    {
        for (int i = 0; i < GameManager.Players.Count; i++)
        {
            var name = PlaceBoxes[i].transform.Find("Name").GetComponent<TextMeshProUGUI>();
            var score = PlaceBoxes[i].transform.Find("Score").GetComponent<TextMeshProUGUI>();

            name.text = GameManager.Players[i].Name;

            int rawScore = GameManager.Players[i].Score;
            bool isNegative = rawScore < 0;
            int absScore = isNegative ? -rawScore : rawScore;

            string score_str;
            if (absScore >= 1000)
            {
                int thousands = absScore / 1000;
                int remainder = absScore % 1000;
                // "D3" Ч форматирование с ведущими нул€ми до трЄх цифр
                score_str = (isNegative ? "-" : "") + $"{thousands},{remainder:D3}";
            }
            else
            {
                score_str = rawScore.ToString();
            }

            score.text = score_str;
        }
    }

    public void DrawContinueButton()
    {
        Button buttonComponent = ContinueButton.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.RemoveAllListeners();

            buttonComponent.onClick.AddListener(() => OnContinueButtonClicked());
        }
    }

    public void OnContinueButtonClicked()
    {

    }

    public void Show()
    {
        Itself.SetActive(true);
    }
}

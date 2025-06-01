using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameMenuView : MonoBehaviour
{
    public GameObject Itself;
    public Button HelpButton;
    public Button ExitButton;
    public TextMeshProUGUI ExitButtonText;
    public bool IsExitButtonClicked=false;
    public GameManager GameManager;
    void Start()
    {
        HelpButton.onClick.AddListener(OnHelpButtonClicked);
        ExitButton.onClick.AddListener(OnExitButtonClicked);
    }
    public void OnHelpButtonClicked()
    {
        GameManager.AudioPlayer.PlayButtonClick();
        GameManager.OpenTutorial();
        Close();
    }

    public void OnExitButtonClicked()
    {
        GameManager.AudioPlayer.PlayButtonClick();
        if (!IsExitButtonClicked)
        {
            IsExitButtonClicked = true;
            ExitButtonText.text = "Вы уверены?";
        }
        else
        {
            GameManager.StopAllCoroutines();
            //SceneManager.UnloadSceneAsync("Game");
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void Open()
    {
        Itself.SetActive(true);
    }

    public void Close()
    {
        IsExitButtonClicked = false;
        ExitButtonText.text = "В меню";
        Itself.SetActive(false);
    }
}

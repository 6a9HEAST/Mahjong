using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public List<GameObject> Buttons;
    public GamePrepareView GamePrepareView;
    public MenuAudioPlayer AudioPlayer;

    public float LightScale = 1.0f;
    public float AnimationTime = 0.5f;

    void Start()
    {
        MainMenuButtons mainMenuButtons = new(this);
        mainMenuButtons.CreateButtons(Buttons);
        AudioPlayer.PlayMusic();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    //public void ShowButtonBox()
    //{
    //    ButtonsBox.SetActive(true);
    //}

    public IEnumerator OnPlayButtonClicked(GameObject button)
    {
        HideButtons(button);
        AudioPlayer.StopMusic();
        AudioPlayer.PlayButtonClick();
        var light=button.GetComponent<ButtonHover>().Light;
        yield return StartCoroutine(ScaleToTarget(light));
        StartGame();
    }

    public IEnumerator OnSettingsButtonClicked(GameObject button)
    {
        HideButtons(button);
        AudioPlayer.PlayButtonClick();
        var light = button.GetComponent<ButtonHover>().Light;
        yield return StartCoroutine(ScaleToTarget(light));

    }
    public IEnumerator OnHelpButtonClicked(GameObject button)
    {
        HideButtons(button);
        AudioPlayer.PlayButtonClick();
        var light = button.GetComponent<ButtonHover>().Light;
        yield return StartCoroutine(ScaleToTarget(light));
        SceneManager.LoadScene("Tutorial");
        
    }

    public IEnumerator OnExitButtonClicked(GameObject button)
    {
        HideButtons(button);
        AudioPlayer.PlayButtonClick();
        var light = button.GetComponent<ButtonHover>().Light;
        yield return StartCoroutine(ScaleToTarget(light));
        Application.Quit();
    }

    public void HideButtons(GameObject _buton)
    {
        foreach (var button in Buttons)
            if (button!=_buton)
                button.SetActive(false);
    }

    private IEnumerator ScaleToTarget(GameObject Light)
    {
        var rect=Light.GetComponent<RectTransform>();

        Vector3 targetScale = new Vector3(LightScale, LightScale, 1f);
        Vector3 initialScale = transform.localScale;

        float elapsed = 0f;

        while (elapsed < AnimationTime)
        {
            // Нормализованное время от 0 до 1
            float t = elapsed / AnimationTime;
            // Интерполяция масштаба
            rect.localScale = Vector3.Lerp(initialScale, targetScale, t);
            // Увеличиваем прошедшее время
            elapsed += Time.deltaTime;
            yield return null;
        }

        // В конце принудительно ставим точный целевой масштаб
        rect.localScale = targetScale;
        yield return new WaitForSeconds(0.2f);
    }
}

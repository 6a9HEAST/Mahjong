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
    public GameObject ButtonsScreen;

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
        SetDefaultScale(light);
        OpenPrepareScreen();
        HideButtons();
        //StartGame();
    }

    public void OpenPrepareScreen()
    {
        GamePrepareView.Show(this);
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

    public void HideButtons(GameObject _button=null)
    {
        foreach (var button in Buttons)
            if (_button==null)
            {
                button.SetActive(false);
                ButtonsScreen.SetActive(false);
            }
            else 
            if (button == _button)
                button.SetActive(true);
            else button.SetActive(false);

    }

    public void ShowButtons()
    {
        foreach (var button in Buttons)
            button.SetActive(true);
        ButtonsScreen.SetActive(true);
    }

    private IEnumerator ScaleToTarget(GameObject Light)
    {
        var rect=Light.GetComponent<RectTransform>();

        Vector3 targetScale = new Vector3(LightScale, LightScale, 1f);
        Vector3 initialScale = transform.localScale;

        float elapsed = 0f;

        while (elapsed < AnimationTime)
        {
            // ��������������� ����� �� 0 �� 1
            float t = elapsed / AnimationTime;
            // ������������ ��������
            rect.localScale = Vector3.Lerp(initialScale, targetScale, t);
            // ����������� ��������� �����
            elapsed += Time.deltaTime;
            yield return null;
        }

        // � ����� ������������� ������ ������ ������� �������
        rect.localScale = targetScale;
        yield return new WaitForSeconds(0.2f);
    }
    
    private void SetDefaultScale(GameObject Light)
    {
        var rect = Light.GetComponent<RectTransform>();
        rect.localScale = Vector3.one;
    }
}

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CallsButtonsView : MonoBehaviour
{
    public GameObject buttonPrefab; // Префаб кнопки с компонентом Button и Text
    public Transform ButtonsRow1;
    public Transform ButtonsRow2;
    public IPlayer player;
    public int buttonsCreated = 0;

    public void CreateChiButton()
    {
        GameObject btn;
        if (buttonsCreated < 3)
        {
            btn = Instantiate(buttonPrefab, ButtonsRow1);
        }
        else
        {
            btn = Instantiate(buttonPrefab, ButtonsRow2);
        }

        // Назначаем текст кнопки
        var btnText = btn.GetComponentInChildren<TextMeshProUGUI>();
        if (btnText != null)
            btnText.text = "Чи";

        // Получаем компонент Button и добавляем обработчик события onClick
        Button buttonComponent = btn.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.AddListener(() => OnChiButtonClicked());
        }
        buttonsCreated++;
    }

    private void OnChiButtonClicked()
    {
        player.CallChi(player.chi.Item1[0]);
        Clear();
    }


    public void CreatePonButton()
    {
        GameObject btn;
        if (buttonsCreated < 3)
        {
            btn = Instantiate(buttonPrefab, ButtonsRow1);
        }
        else
        {
            btn = Instantiate(buttonPrefab, ButtonsRow2);
        }

        var btnText = btn.GetComponentInChildren<TextMeshProUGUI>();
        if (btnText != null)
            btnText.text = "Пон";

        Button buttonComponent = btn.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.AddListener(() => OnPonButtonClicked());
        }
        buttonsCreated++;
    }

    private void OnPonButtonClicked()
    {
        player.CallPon();
        Clear();
    }

    public void CreateKanButton()
    {
        GameObject btn;
        if (buttonsCreated < 3)
        {
            btn = Instantiate(buttonPrefab, ButtonsRow1);
        }
        else
        {
            btn = Instantiate(buttonPrefab, ButtonsRow2);
        }

        var btnText = btn.GetComponentInChildren<TextMeshProUGUI>();
        if (btnText != null)
            btnText.text = "Кан";

        Button buttonComponent = btn.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.AddListener(() => OnKanButtonClicked());
        }
        buttonsCreated++;
    }

    private void OnKanButtonClicked()
    {
        player.CallKan();
        Clear();
    }

    public void CreatePassButton()
    {
        GameObject btn;
        if (buttonsCreated < 3)
        {
            btn = Instantiate(buttonPrefab, ButtonsRow1);
        }
        else
        {
            btn = Instantiate(buttonPrefab, ButtonsRow2);
        }

        var btnText = btn.GetComponentInChildren<TextMeshProUGUI>();
        if (btnText != null)
            btnText.text = "Пропуск";

        Button buttonComponent = btn.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.AddListener(() => OnPassButtonClicked());
        }
        buttonsCreated++;
    }

    private void OnPassButtonClicked()
    {
        player.CallPass();
        Clear();
    }

    public void Clear()
    {
        foreach (Transform child in ButtonsRow1)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in ButtonsRow2)
        {
            Destroy(child.gameObject);
        }
        buttonsCreated = 0;
        
    }
}

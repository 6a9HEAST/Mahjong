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
        player.CallChi();
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
        Clear();
        player.GameManager.StartCoroutine(player.ExecutePass());
        
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

    public void CreateRiichiButton()
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
            btnText.text = "Риичи";

        Button buttonComponent = btn.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.AddListener(() => OnRiichiButtonClicked());
        }
        buttonsCreated++;
    }

    public void OnRiichiButtonClicked()
    {
        player.Riichi = true;
        CreateCancelRiichiButton();
    }

    public void CreateCancelRiichiButton() // при создании кнопки отмены удаляем все остальные кнопки и оставляем только эту
    {
        Clear();

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
            btnText.text = "Назад";

        Button buttonComponent = btn.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.AddListener(() => OnCancelButtonClicked());
        }
        buttonsCreated++;

    }
    public void OnCancelButtonClicked() // при нажатии кнопки отммены удаляем ее и возвращаем все остальые кнопки
    {
        player.Riichi = false;
        Clear();
        player.ProceedCalls();
    }

    public void CreateTsumoButton()
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
            btnText.text = "Цумо";

        Button buttonComponent = btn.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.AddListener(() => OnTsumoButtonClicked());
        }
        buttonsCreated++;
    }

    public void OnTsumoButtonClicked()
    {
        player.ExecuteTsumo();
    }

    public void CreateRonButton()
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
            btnText.text = "Рон";

        Button buttonComponent = btn.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.AddListener(() => OnRonButtonClicked());
        }
        buttonsCreated++;
    }
    public void OnRonButtonClicked()
    {
    player.CallRon();
    }
}

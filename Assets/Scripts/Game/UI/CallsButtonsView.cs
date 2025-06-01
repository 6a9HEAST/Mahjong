using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CallsButtonsView : MonoBehaviour
{
    public GameObject buttonPrefab; // Префаб кнопки с компонентом Button и Text
    public GameObject IndicatorPrefab;

    public Transform ButtonsRow1;
    public Transform ButtonsRow2;
    public GameObject CallIndicator { get; private set; }

    public IPlayer player;
    public int buttonsCreated = 0;
    public bool indicatorCreated=false;

    private static Dictionary<string, Sprite> spriteDictionary = new Dictionary<string, Sprite>();

    private void Start()
    {
        foreach (var sprite in Resources.LoadAll<Sprite>("Buttons"))
        {

            spriteDictionary[sprite.name] = sprite;
        }
    }
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

        var rect= btn.GetComponentInChildren<RectTransform>();
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2.8454f);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0.84f);

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

        var rect = btn.GetComponentInChildren<RectTransform>();
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2.8454f);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0.84f);

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

        var rect = btn.GetComponentInChildren<RectTransform>();
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2.8454f);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0.84f);

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

        var rect = btn.GetComponentInChildren<RectTransform>();
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2.8454f);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0.84f);

        var img = btn.GetComponent<Image>();
        if (img != null)
            if (spriteDictionary.TryGetValue("Red", out var sprite))
            {
                img.sprite = sprite;
            }

        Button buttonComponent = btn.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.AddListener(() => OnPassButtonClicked());
            buttonComponent.transition = Selectable.Transition.SpriteSwap;
            var ss = buttonComponent.spriteState;
            if (spriteDictionary.TryGetValue("RedHighlighted", out var sprite))
            {
                ss.highlightedSprite = sprite;
            }
            buttonComponent.spriteState = ss;
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
        indicatorCreated = false;
        if (CallIndicator!=null)
            Destroy(CallIndicator);
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

        var rect = btn.GetComponentInChildren<RectTransform>();
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2.8454f);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0.84f);

        var img = btn.GetComponent<Image>();
        if (img != null)
            if (spriteDictionary.TryGetValue("Green", out var sprite))
            {
                img.sprite = sprite;
            }

        Button buttonComponent = btn.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.AddListener(() => OnRiichiButtonClicked());
            buttonComponent.transition = Selectable.Transition.SpriteSwap;
            var ss = buttonComponent.spriteState;
            if (spriteDictionary.TryGetValue("GreenHighlighted", out var sprite))
            {
                ss.highlightedSprite = sprite;
            }
            buttonComponent.spriteState = ss;
        }
        buttonsCreated++;
    }
    public void OnRiichiButtonClicked()
    {
        player.Riichi = true;
        player.PlayerHandView.DrawRiichiHand(player.Hand);
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

        var rect = btn.GetComponentInChildren<RectTransform>();
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2.8454f);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0.84f);

        var img = btn.GetComponent<Image>();
        if (img != null)
            if (spriteDictionary.TryGetValue("Red", out var sprite))
            {
                img.sprite = sprite;
            }

        Button buttonComponent = btn.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.AddListener(() => OnCancelButtonClicked());
            buttonComponent.transition = Selectable.Transition.SpriteSwap;
            var ss = buttonComponent.spriteState;
            if (spriteDictionary.TryGetValue("RedHighlighted", out var sprite))
            {
                ss.highlightedSprite = sprite;
            }
            buttonComponent.spriteState = ss;
        }
        buttonsCreated++;

    }
    public void OnCancelButtonClicked() // при нажатии кнопки отммены удаляем ее и возвращаем все остальые кнопки
    {
        player.Riichi = false;
        player.DrawHand();
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

        var rect = btn.GetComponentInChildren<RectTransform>();
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2.8454f);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0.84f);

        var img = btn.GetComponent<Image>();
        if (img != null)
            if (spriteDictionary.TryGetValue("Yellow", out var sprite))
            {
                img.sprite = sprite;
            }

        Button buttonComponent = btn.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.AddListener(() => OnTsumoButtonClicked());
            buttonComponent.transition = Selectable.Transition.SpriteSwap;
            var ss = buttonComponent.spriteState;
            if (spriteDictionary.TryGetValue("YellowHighlighted", out var sprite))
            {
                ss.highlightedSprite = sprite;
            }
            buttonComponent.spriteState = ss;
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

        var rect = btn.GetComponentInChildren<RectTransform>();
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2.8454f);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0.84f);

        var img = btn.GetComponent<Image>();
        if (img != null)
            if (spriteDictionary.TryGetValue("Yrllow", out var sprite))
            {
                img.sprite = sprite;
            }

        Button buttonComponent = btn.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.AddListener(() => OnRonButtonClicked());
            buttonComponent.transition = Selectable.Transition.SpriteSwap;
            var ss = buttonComponent.spriteState;
            if (spriteDictionary.TryGetValue("YellowHighlighted", out var sprite))
            {
                ss.highlightedSprite = sprite;
            }
            buttonComponent.spriteState = ss;
        }
        buttonsCreated++;
    }
    public void OnRonButtonClicked()
    {
    player.CallRon();
    }

    public void CreateCallIndicator(IPlayer player)
    {
        if (indicatorCreated) return;

        var spawnPosition = player.PlayerDiscardView.GetLastTilePosition(player.Discard);

        if (IndicatorPrefab != null)
        {
            GameObject instance = Instantiate(
                IndicatorPrefab,
                spawnPosition,
                Quaternion.identity
            );

            instance.name = "CallIndicator";

            // Добавляем вращение
            Rotator rotator = instance.AddComponent<Rotator>();

            //rotator.rotationSpeed = 180f;
            //rotator.rotationAxis = Vector3.up; Веселая ось

            rotator.rotationSpeed = 51f;
            rotator.rotationAxis = new Vector3(0,0,1); 

            CallIndicator = instance;
            indicatorCreated = true;
        }
        else
        {
            Debug.LogError("Prefab reference is missing!");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class PointsCalculationView:MonoBehaviour
{
    public List<Button> Buttons;

    public List<Sprite> Sprites;

    public SpriteRenderer spriteRenderer;

    public Sprite DefaultButton;
    public Sprite ClickedButton;

    public void Start()
    {
        Buttons[0].onClick.AddListener(OnNonDealerButtonClicked);
        Buttons[1].onClick.AddListener(OnDealerButtonClicked);
        Buttons[2].onClick.AddListener(OnFuButtonClicked);
    }

    private void OnNonDealerButtonClicked()
    {
        SetButtonSprite(Buttons[0]);
        SetImageSprite(Sprites[0]);
    }

    private void OnDealerButtonClicked()
    {
        SetButtonSprite(Buttons[1]);
        SetImageSprite(Sprites[1]);
    }

    private void OnFuButtonClicked()
    {
        SetButtonSprite(Buttons[2]);
        SetImageSprite(Sprites[2]);
    }

    private void SetButtonSprite(Button button)
    {
        foreach (var item in Buttons)
        {
            item.GetComponent<Image>().sprite = DefaultButton;
        }
        button.GetComponent<Image>().sprite = ClickedButton;
    }

    private void SetImageSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }
}

